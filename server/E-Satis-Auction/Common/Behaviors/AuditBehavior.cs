using System.Security.Claims;
using System.Text.Json;
using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Models.Common;
using MediatR;

namespace E_Satis_Auction.Common.Behaviors;

public class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IAuditableCommandMarker
{
    private readonly IAuditLogQueue _auditLogQueue;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuditBehavior<TRequest, TResponse>> _logger;

    private const string UNKNOW_IP = "UNKNOWN_IP";

    public AuditBehavior(
        IAuditLogQueue auditLogQueue,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuditBehavior<TRequest, TResponse>> logger)
    {
        _auditLogQueue = auditLogQueue;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        TResponse response = await next(cancellationToken);
        try
        {
            ClaimsPrincipal? userPrincipal = _httpContextAccessor.HttpContext?.User;
            string userId = userPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier) ?? SystemConstants.SystemUser;
            string userEmail = userPrincipal?.FindFirstValue(ClaimTypes.Email) ?? SystemConstants.SystemUserMail;
            string ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? UNKNOW_IP;
            string actionName = typeof(TRequest).Name;
            string details = JsonSerializer.Serialize(request);

            AuditLog log = AuditLog.Create(userId, userEmail, ipAddress, actionName, details);
            await _auditLogQueue.EnqueueAsync(log, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating the automatic audit log.");
        }

        return response;
    }
}