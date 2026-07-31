namespace LunchNear.Application.Common.Behaviours;

using global::Mediator;
using LunchNear.Application.Common.Exceptions;
using LunchNear.Domain.Exceptions;
using Microsoft.Extensions.Logging;

/// <summary>
/// Logs truly unexpected exceptions. Expected, "business" outcomes (validation failures,
/// not-found, domain rule violations) are left to propagate untouched - they are translated
/// into proper ProblemDetails responses by the API layer, not swallowed or logged as errors here.
/// </summary>
public sealed class UnhandledExceptionBehaviour<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    private readonly ILogger<UnhandledExceptionBehaviour<TMessage, TResponse>> _logger;

    public UnhandledExceptionBehaviour(ILogger<UnhandledExceptionBehaviour<TMessage, TResponse>> logger)
    {
        _logger = logger;
    }

    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next(message, cancellationToken);
        }
        catch (Exception ex) when (ex is not ValidationException and not NotFoundException and not DomainException)
        {
            _logger.LogError(ex, "Unhandled exception for message {MessageName}", typeof(TMessage).Name);
            throw;
        }
    }
}
