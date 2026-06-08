using E_Satis_Auction.Extensions;
using E_Satis_Auction.Common.Behaviors;
using E_Satis_Auction.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization();
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    string[] supportedCultures = ["en", "tr"];
    options.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        ValidationProblemDetails problemDetails = new(context.ModelState)
        {
            Status = StatusCodes.Status400BadRequest
        };

        return new BadRequestObjectResult(problemDetails);
    };
});

builder.Services.AddControllers(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
}).AddDataAnnotationsLocalization();
builder.Services.AddSignalR();
builder.Services.AddScalarOpenApi();

builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddRedisCaching(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCustomRateLimiting();

builder.Services.AddProxyAndCorsServices(builder.Configuration);
builder.Services.AddHttpContextAccessor();

builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(typeof(Program).Assembly);
    configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
    configuration.AddOpenBehavior(typeof(CachingBehavior<,>));
    configuration.AddOpenBehavior(typeof(AuditBehavior<,>));
});
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.ConfigureCustomOptions(builder.Configuration);

builder.Services.AddLogger();

WebApplication app = builder.Build();

app.UseForwardedHeaders();

app.UseRequestLocalization();

app.UseExceptionHandler();

await app.SeedDatabaseAsync();

//if (app.Environment.IsDevelopment())
//{
    app.MapOpenApi();
    app.MapScalarApiReference();
//}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors(SecurityServiceExtensions.CorsPolicyName);

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<E_Satis_Auction.Hubs.AuctionHub>("/hubs/auctions");

app.Run();
