namespace JobTracker.SharedKernel.Results;

/// <summary>
/// Aggregates every FluentValidation failure for a single request into one
/// <see cref="Error"/> so <see cref="Behaviors.ValidationBehavior{TRequest,TResponse}"/>
/// can short-circuit the pipeline with a single Result failure.
/// </summary>
public sealed record ValidationError : Error
{
    public ValidationError(Error[] errors)
        : base("Validation.General", "One or more validation errors occurred.", ErrorType.Validation)
    {
        Errors = errors;
    }

    public Error[] Errors { get; }

    public static ValidationError FromErrors(IEnumerable<Error> errors) => new(errors.ToArray());
}
