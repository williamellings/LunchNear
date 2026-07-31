namespace LunchNear.Application.Common.Behaviours;

using FluentValidation;
using global::Mediator;
using ValidationException = LunchNear.Application.Common.Exceptions.ValidationException;

/// <summary>
/// Runs all registered FluentValidation validators for a request before it reaches its handler.
/// A single, uniform enforcement point - this is what closes the gap from the previous review
/// where restaurant ratings were validated but dish ratings were not.
/// </summary>
public sealed class ValidationBehaviour<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    private readonly IEnumerable<IValidator<TMessage>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TMessage>> validators)
    {
        _validators = validators;
    }

    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TMessage>(message);

            var results = await Task.WhenAll(
                _validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

            var failures = results
                .SelectMany(result => result.Errors)
                .Where(failure => failure is not null)
                .ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }

        return await next(message, cancellationToken);
    }
}
