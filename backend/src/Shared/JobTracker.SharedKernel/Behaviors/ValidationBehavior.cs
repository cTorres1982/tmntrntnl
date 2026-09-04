using FluentValidation;
using JobTracker.SharedKernel.Results;
using MediatR;

namespace JobTracker.SharedKernel.Behaviors;

/// <summary>
/// MediatR pipeline behavior that runs every registered FluentValidation validator for
/// the incoming request before the handler executes, short-circuiting with a
/// <see cref="ValidationError"/> Result instead of throwing. Applies to every module —
/// registered once per module's DI setup, not duplicated per handler.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        Error[] errors = _validators
            .Select(validator => validator.Validate(request))
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .Select(failure => Error.Validation(failure.PropertyName, failure.ErrorMessage))
            .Distinct()
            .ToArray();

        if (errors.Length == 0)
        {
            return await next();
        }

        return CreateValidationFailure(errors);
    }

    private static TResponse CreateValidationFailure(Error[] errors)
    {
        var validationError = ValidationError.FromErrors(errors);

        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(validationError);
        }

        Type resultValueType = typeof(TResponse).GetGenericArguments()[0];
        var failureFactory = typeof(Result)
            .GetMethod(nameof(Result.Failure), 1, [typeof(Error)])!
            .MakeGenericMethod(resultValueType);

        return (TResponse)failureFactory.Invoke(null, [validationError])!;
    }
}
