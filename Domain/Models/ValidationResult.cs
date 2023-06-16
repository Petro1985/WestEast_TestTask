namespace Services.Models;

public struct ValidationResult
{
    public bool IsSuccess => !Errors.Any();
    public IEnumerable<string> Errors { get; }

    public ValidationResult()
    {
        Errors = Enumerable.Empty<string>();
    }

    private ValidationResult(IEnumerable<string> errors)
    {
        Errors = errors;
    }

    public static ValidationResult Success()
    {
        return new ValidationResult();
    }

    public static ValidationResult Fail(params string[] errors)
    {
        return new ValidationResult(errors);
    }
}