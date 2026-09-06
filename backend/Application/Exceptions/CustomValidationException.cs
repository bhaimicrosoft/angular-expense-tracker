using FluentValidation.Results;

namespace Application.Exceptions;

public class CustomValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public CustomValidationException(IDictionary<string, string[]> errors) : base(
        "One or more validation failures have occurred.")
    {
        Errors = errors;
    }

    public CustomValidationException(IEnumerable<ValidationFailure> failures) : this(
        failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray())
    )
    {
    }
}