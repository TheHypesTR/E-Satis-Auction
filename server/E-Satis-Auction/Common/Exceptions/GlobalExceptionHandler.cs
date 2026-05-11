using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Resources;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace E_Satis_Auction.Common.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IStringLocalizer<SharedResource> localizer)
    {
        _logger = logger;
        _localizer = localizer;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is ValidationException or BusinessException or NotFoundException or ForbiddenAccessException)
        {
            _logger.LogWarning("Domain logic violation [{ExceptionType}]: {Message}", exception.GetType().Name, exception.Message);
        }
        else
        {
            _logger.LogError(exception, "Unhandled system exception occurred: {Message}", exception.Message);
        }

        switch (exception)
        {
            case ValidationException validationException:
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                Dictionary<string, string[]> localizedErrors = validationException.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => _localizer[x.ErrorMessage].Value).ToArray()
                    );

                ValidationProblemDetails validationProblemDetails = new(localizedErrors)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = _localizer[ErrorMessages.Exception.ValidationErrorTitle].Value,
                    Detail = _localizer[ErrorMessages.Exception.ValidationErrorDetail].Value
                };

                await httpContext.Response.WriteAsJsonAsync(validationProblemDetails, cancellationToken);
                return true;
            }
            case BusinessException businessException:
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                string localizedTitle = string.IsNullOrEmpty(businessException.Title)
                    ? _localizer[ErrorMessages.Exception.BusinessErrorTitle].Value
                    : _localizer[businessException.Title].Value;
                string localizedMessage = _localizer[businessException.Message].Value;

                ProblemDetails busProblemDetails = new()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = localizedTitle,
                    Detail = localizedMessage
                };

                await httpContext.Response.WriteAsJsonAsync(busProblemDetails, cancellationToken);
                return true;
            }
            case ForbiddenAccessException forbiddenException:
            {
                httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            
                string localizedTitle = string.IsNullOrEmpty(forbiddenException.Title)
                    ? _localizer[ErrorMessages.Exception.SecurityTitle].Value
                    : _localizer[forbiddenException.Title].Value;
                string localizedMessage = _localizer[forbiddenException.Message].Value;

                ProblemDetails secProblemDetails = new()
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = localizedTitle,
                    Detail = localizedMessage
                };

                await httpContext.Response.WriteAsJsonAsync(secProblemDetails, cancellationToken);
                return true;
            }
            case NotFoundException notFoundException:
            {
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

                string localizedTitle = _localizer[ErrorMessages.Exception.ResourceNotFoundTitle].Value;
                string localizedEntityName = _localizer[notFoundException.EntityName].Value;
                string localizedMessage = _localizer[
                    ErrorMessages.Exception.NotFoundMessage,
                    localizedEntityName,
                    notFoundException.Key].Value;

                ProblemDetails notFoundProblemDetails = new()
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = localizedTitle,
                    Detail = localizedMessage
                };

                await httpContext.Response.WriteAsJsonAsync(notFoundProblemDetails, cancellationToken);
                return true;
            }
            default:
                return false;
        }
    }
}