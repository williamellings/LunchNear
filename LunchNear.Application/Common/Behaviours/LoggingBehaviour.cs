namespace LunchNear.Application.Common.Behaviours;

using System.Diagnostics;
using global::Mediator;
using Microsoft.Extensions.Logging;

public sealed class LoggingBehaviour<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    private readonly ILogger<LoggingBehaviour<TMessage, TResponse>> _logger;

    public LoggingBehaviour(ILogger<LoggingBehaviour<TMessage, TResponse>> logger)
    {
        _logger = logger;
    }

    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        var messageName = typeof(TMessage).Name;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("Handling {MessageName}", messageName);

        var response = await next(message, cancellationToken);

        _logger.LogInformation("Handled {MessageName} in {ElapsedMs}ms", messageName, stopwatch.ElapsedMilliseconds);

        return response;
    }
}
