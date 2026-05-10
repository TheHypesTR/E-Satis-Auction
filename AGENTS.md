# AGENTS Guide - Emergency-Platform-HUB

## Scope and Source of Truth
- This guide is derived from discoverable repo state. It is the absolute source of truth for code generation, architecture decisions, and refactors.
- Treat `client/` and `server/Emergency-Platform-HUB/` as separate apps with independent run loops.

## Big-Picture Architecture
- Frontend: React + Vite SPA (`client/src/main.tsx` -> `App.tsx` -> `createAppRouter` in `client/src/routes.tsx`).
- Backend: ASP.NET Core Web API (`Program.cs`) using MediatR + pipeline behaviors + EF Core + Identity + JWT.
- Local Infra: PostgreSQL + Redis are expected locally via `server/docker-compose.yml`.
- API docs in development are served with OpenAPI + Scalar (`Program.cs`, `Extensions/ScalarOpenApiExtension.cs`).

## Core Backend Request Flow
- Controllers are thin and dispatch entirely to MediatR (`Controllers/AuthController.cs`, etc.).
- Shared controller bases: `BaseController` (lazy `Mediator`), `AuthorizedBaseController` (`CurrentUserId` from claims).
- MediatR behaviors run cross-cutting logic:
  - `ValidationBehavior` (FluentValidation fail-fast)
  - `CachingBehavior` (queries implementing cache marker hit Redis)
  - `AuditBehavior` (commands implementing audit marker enqueue logs)
- `AppDbContext` applies soft-delete query filters and auto audit timestamps on `SaveChangesAsync`.

---

## DOMAIN & PERSISTENCE ARCHITECTURE STANDARDS (NEW)

### Event-Driven Architecture & Domain Events (CRITICAL)
- **Side Effect Isolation:** Command Handlers MUST NOT manually orchestrate internal side effects like writing to Audit Logs or Inventory Ledgers (e.g., injecting `IInventoryTransactionRepository` into a Dispatch handler is FORBIDDEN).
- **Domain Event Triggering:** Aggregate Roots must encapsulate state changes and trigger Domain Events (implementing MediatR `INotification`) using `AddDomainEvent(...)` from the `IHasDomainEvents` interface.
- **EF Core Interceptors for Events (NO MEDIATR IN `DBCONTEXT`):** **NEVER** inject `IMediator` into `AppDbContext`. Domain Events MUST be dispatched using an EF Core `SaveChangesInterceptor` (e.g., `DispatchDomainEventsInterceptor`).
- **Pre-Commit Dispatching (While Loop):** The Interceptor MUST dispatch events *before* calling `SavingChangesAsync`. It must use a `while(true)` loop to continuously fetch, clear, and publish events until the `ChangeTracker` has no pending events left. This guarantees that cascading side effects (like logging stock movements) are implicitly included in the exact same database transaction as the original entity mutation.
- **Event Handler Purity & Fallbacks:** `INotificationHandler<T>` implementations must be fast and purely reactive. Avoid making external HTTP calls. If an event handler requires a UserId (e.g., via `ICurrentUserService`), it MUST implement a fallback (e.g., `SystemConstants.SystemUser`) to prevent crashes when triggered by background workers or cron jobs where the HTTP context is null.

### Master vs Transactional Model
- `Product` is **Master Data** (Catalog/Canonical): SKU, optional barcode, category, unit, base attributes.
- `Item` is **Transactional Batch/Inventory State**: Facility-level stock batch with status, quantity, optional expiration, and batch-level dynamic attributes.
- **Never mix responsibilities:** Do not put master catalog lifecycle/business rules into `Item`. Do not use `Product` as a stock movement or batch snapshot entity.

### Item Mode Invariants (MANDATORY)
- `ItemMode.Standardized`: `ProductId` MUST be present and non-empty.
- `ItemMode.AdHoc`: `ProductId` MUST be null. Ad-hoc naming/context fields MUST be validated by domain guards.
- Enforce these invariants strictly in domain factories/behaviors (`Item.CreateFromProduct`, `Item.CreateAdHoc`), not in controllers.

### Semantic Attribute Keys & JSONB Rules
- JSON attribute keys are **semantic codes**, not GUIDs. `CategoryAttribute.Code` is immutable, normalized (lowercase + underscore), and is the canonical key for JSON payloads.
- **Target Isolation:** CategoryAttribute utilizes the AttributeTarget enum (ProductLevel = 1, ItemLevel = 2). Command Handlers MUST explicitly filter attributes by their Target before validating and persisting JSON payloads to prevent cross-contamination between Master Data (Products) and Transactional Data (Items).
- **Encapsulation:** Domain exposure must be read-only (`IReadOnlyDictionary<string, string>`), backed by private mutable dictionary fields.
- **EF Core Mapping:** Persist backing fields as `jsonb` with explicit field access mode. Always configure a `ValueComparer` for dictionary-backed JSON properties.
- Add GIN indexes on frequently queried JSONB columns (e.g., `_dynamicAttributes`, `_baseAttributes`).

### Soft-Delete Hierarchy Expectations
- Global query filter excludes `IsDeleted=true` records centrally in `AppDbContext`.
- For hierarchy-like structures (e.g., `Category -> CategoryAttribute -> CategoryAttributeOption`), use soft-delete-safe constraints:
  - Filtered unique indexes MUST include `"IsDeleted" = false` in their configuration.
- Handlers must apply explicit soft-delete intent on dependents when aggregate semantics require it (Cascade soft-delete).

### Category Schema Mutation Safety (MANDATORY)
- Category metadata (`Name`, `Description`) can be updated independently.
- Attribute/option schema mutations (`add/update/delete`) are allowed **only when** `Category.IsActive == false`.
- Attribute/option schema mutations are blocked when any `Product` or `Item` exists for that category, because JSONB attribute keys depend on category schema.
- Enforce this in command handlers before mutating aggregate internals.

### Concurrency Rules & Migration Pre-Flight Checklist
- Use optimistic concurrency for high-contention aggregates (`Product`, `Item`).
- **PostgreSQL `xmin` Caveat:** We map PostgreSQL's hidden `xmin` system column to an explicit `Version` property (`.IsRowVersion().HasColumnName("xmin")`) to allow the frontend to participate in concurrency checks. Npgsql natively recognizes this configuration and will silently skip `AddColumn` operations for `xmin` during migrations. No manual editing of migration files is required.

---

## STRICT CQRS & MEDIATR RULES (CRITICAL)
- **NEVER** use standard `IRequest` or `IRequestHandler` directly in your feature handlers. Always use the custom wrapper interfaces (`ICommand`, `IQuery`, etc.).
- For **Queries** (Read operations): Always implement `IQuery<TResponse>`. Handler must be `IQueryHandler<TQuery, TResponse>`.
- For **Cached Queries**: Implement `ICacheableQuery<TResponse>`. Do NOT write caching logic in handlers.
- For **Commands** (Write operations returning data): Implement `ICommand<TResponse>`. Handler must be `ICommandHandler<TCommand, TResponse>`.
- For **Commands** (Write operations returning NOTHING): Implement `ICommand` (non-generic). Handler must be `ICommandHandler<TCommand>` and return `Task` (DO NOT return `Unit`).
- For **Audited Commands**: Implement `IAuditableCommand<T>` or `IAuditableCommand`. Mask passwords using `[JsonIgnore]`.
- **Pipeline Behavior Constraints:** Never use `where TRequest : IRequest<TResponse>` for behaviors like Audit or Validation if they need to handle non-generic `ICommand`. ALWAYS use specific marker interfaces as constraints (e.g., `where TRequest : IAuditableCommandMarker`).
- **Polymorphic Entities (Separate Write, Unified Read):** For inherited entities (TPH) like Facility, ALWAYS use specific Commands and separate endpoints for Creation/Mutation to ensure strict, type-safe validation. For reading, use a unified Query returning a generic/polymorphic DTO.
- **Context Separation:** MediatR Commands and Queries MUST remain pure DTOs. NEVER pass HTTP context data like `CurrentUserId` from the Controller into the Command. Handlers MUST independently resolve the user's context by injecting `ICurrentUserService`.
- **FluentValidation Payload Shaping:** When validating cross-property business rules (e.g., an XOR rule where either `TargetFacilityId` OR `TargetAddressId` must exist), ALWAYS use `.WithName("FieldName")`. This ensures the validation error maps to a specific, predictable JSON key for the frontend form, rather than defaulting to the generic parent object name (like "Payload").
- **Explicit Transactions for Multi-Aggregate Updates:** Commands that mutate multiple aggregates or require strict atomicity (e.g., Stock Dispatches, Conversions) MUST be wrapped in explicit `await _unitOfWork.BeginTransactionAsync()`, `CompleteAsync()`, and `CommitTransactionAsync()` blocks. Always include a `try-catch` block that triggers `RollbackTransactionAsync()` on failure.

## DATA ACCESS & REPOSITORY RULES
- **DO NOT** inject `AppDbContext` directly into Handlers or Services unless absolutely necessary for complex transactions. Use the generic repository pattern (`IGenericRepository<T>`) or specific repositories.
- **Identity Exception:** Do NOT use repositories for `AppUser`. Always use ASP.NET Core Identity managers (`UserManager<AppUser>`) directly in handlers.
- **Read vs. Write Fetch Separation:** Do NOT reuse the same repository fetch methods for both Queries (Read) and Commands (Write) if it causes over-fetching. Use `.AsNoTracking()` specifically for Read queries.
- **Case-Insensitive Search:** NEVER use `StringComparison` inside EF Core LINQ queries (it crashes at runtime). Use `.ToLower()` to leverage Npgsql's native `ILIKE`.
- **Cross-Aggregate Enrichment (In-Memory Hash Joins):** **NEVER** use `.Include()` or database-level joins across different Aggregate Roots (e.g., linking `Product` to `Category`, or `Item` to `Facility`). Repositories must only include navigation properties within their own aggregate boundaries.
- **Handler Orchestration for Joins:** To enrich data across aggregates, Handlers must fetch the primary aggregate, extract the necessary foreign keys (e.g., `CategoryId`s), and query the secondary repository for a `Dictionary<Guid, TEnrichmentDto>`. Map these together in memory within the Handler to guarantee **O(1) lookup performance** and prevent N+1 issues.
- **Enrichment DTOs over Tuples:** When secondary repositories return multiple fields for an in-memory join (e.g., Facility Name and Address), **ALWAYS** create and use a lightweight `record` DTO (e.g., `FacilityStockEnrichmentDto`). Do not use complex Tuples (`(string Name, ...)`), as they severely degrade code readability.
- **Feature-Based DTO Localization:** DTOs SHOULD be grouped by feature subfolders under the global `Dtos` directory (e.g., `Dtos/Facility`, `Dtos/Product`) to prevent massive root folder clutter. Ensure intent-revealing naming conventions are used.
- **Aggregate Decoupling & Shadow Foreign Keys:** When removing cross-aggregate navigation properties in C# models to enforce DDD boundaries, you MUST preserve database referential integrity. Always configure "Shadow Foreign Keys" in `IEntityTypeConfiguration<T>` using `builder.HasOne<TargetAggregate>().WithMany().HasForeignKey(...)`. Explicitly recreate indexes using `builder.HasIndex(...)` because EF Core drops them when navigation properties are removed. Use `OnDelete(DeleteBehavior.Restrict)` for related root aggregates to prevent orphan records.

## PAGINATION & LISTING STANDARDS
- Never return flat lists (`List<T>`) for bulk data. Always return `PaginatedList<T>`.
- Any query that requires pagination must implement the `IPaginatedQuery` interface.
- Validators for paginated queries MUST inherit from the base `PaginatedQueryValidator<T>` to automatically inherit `PageNumber` and `PageSize` rules. Do not rewrite pagination rules.
- **Anti-Cache for Search/Pagination:** **NEVER** implement `ICacheableQuery` on queries that involve dynamic user inputs such as `SearchTerm` or dynamic pagination parameters (`PageNumber`). Caching these permutations leads to severe cache poisoning and memory bloat. Reserve caching strictly for static or slowly changing Master Data lookups (e.g., `GetProductById`).
- **Pagination Hard-Caps:** `PaginatedQueryValidator<T>` MUST enforce a hard maximum on `PageSize` (e.g., maximum 100). This prevents attackers from requesting massive datasets (`PageSize = 999999`) to bypass rate limits and cause Out-Of-Memory exceptions.

## EXCEPTIONS, GUARD CLAUSES & LOCALIZATION
- **NEVER** use magic strings for validation or exception messages. Always use the constants defined in the `ErrorMessages` static class.
- ALWAYS use Guard Clauses: `NotFoundException.ThrowIfNull(entity, ErrorMessages.Organization.EntityName, id);` or `BusinessException.ThrowIfFalse(...)`.
- **Localized Error Pipeline:** Handlers and Validators must only return/throw constant keys from `ErrorMessages`. The `GlobalExceptionHandler` is strictly responsible for translating these keys into localized messages.
- **Resource-Based Authorization (BOLA/IDOR Prevention):** Role-based authorization (`[RequireRoles]`) is not enough for multi-tenant resources. Handlers MUST verify if the current user has explicit permission to access or mutate the specific resource using `ICurrentUserService`. **NEVER** implement `ICacheableQuery` on queries that contain Resource-Based Authorization.
- **Security & Authorization Pipeline:** For any resource-based authorization failures (e.g., a user trying to access a Facility they don't own), NEVER use `BusinessException`. You MUST use `ForbiddenAccessException.ThrowIfTrue(...)` to ensure the GlobalExceptionHandler returns an HTTP 403 Forbidden, not a 400 Bad Request.
- **Read-Model Failures (Queries):** Queries MUST return pure DTOs, not `Result<T>` envelopes. If a dependency is missing during a read operation, fail-fast using `NotFoundException.ThrowIfNull`.
- **Centralized Authorization (DRY):** NEVER inject `IFacilityManagerRepository` or `IOrganizationManagerRepository` into feature handlers purely to check user permissions. All role, hierarchy, and ownership-based access checks MUST be delegated to `ICurrentUserService` (e.g., `HasFacilityAccess, IsOrganizationManagerOrAdminAsync`). This keeps handlers strictly focused on domain logic and massively improves testability.

## CONTROLLER & INTEGRATION RULES
- Keep controllers orchestration-only and one-liner where possible.
- Always use strict Route Constraints (e.g., `[HttpGet("{id:guid}")]`).
- Route & Body Merging: When receiving an ID from the route and data from the body, DO NOT map them inside the controller. Pass them both into a custom constructor of the MediatR Command.
- Always return HTTP 200 with an empty `PaginatedList` for list queries that yield no results; DO NOT return HTTP 404.
- **Auth Rate-Limiting:** Apply strict rate-limiting policies to sensitive endpoints (Login, Register, OTP).
- **Strict REST Semantics:** Controllers MUST map Handler responses to the correct REST semantic HTTP codes. `[HttpPost]` endpoints that create resources MUST return `CreatedAtAction` (HTTP 201), NEVER `Ok()` (HTTP 200).

## VERIFIED DEV WORKFLOWS
- Frontend (`client/package.json`): `npm install`, `npm run dev`, `npm run build`.
- Backend (`server/Emergency-Platform-HUB`): `dotnet restore`, `dotnet run` (launches Scalar at `/scalar`).
- Infra (`server/docker-compose.yml`): `docker compose up -d` for Postgres/pgAdmin/Redis.