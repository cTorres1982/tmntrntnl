using JobTracker.SharedKernel.Results;

namespace JobTracker.Api.Common;

public static class ResultExtensions
{
    public static IResult ToHttpResult(this Result result) =>
        result.IsSuccess ? Results.NoContent() : ToProblem(result.Error);

    public static IResult ToHttpResult<TValue>(this Result<TValue> result, Func<TValue, IResult>? onSuccess = null) =>
        result.IsSuccess
            ? (onSuccess is not null ? onSuccess(result.Value) : Results.Ok(result.Value))
            : ToProblem(result.Error);

    private static IResult ToProblem(Error error)
    {
        int statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError,
        };

        if (error is ValidationError validationError)
        {
            var errors = validationError.Errors
                .GroupBy(e => e.Code)
                .ToDictionary(group => group.Key, group => group.Select(e => e.Message).ToArray());

            return Results.ValidationProblem(errors, statusCode: statusCode);
        }

        return Results.Problem(title: error.Code, detail: error.Message, statusCode: statusCode);
    }
}
