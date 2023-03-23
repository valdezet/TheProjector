namespace TheProjector.Data.DTO;

public class CommandResult
{
    public bool IsSuccessful { get; private set; }

    public string? ErrorMessage { get; private set; }

    public Dictionary<string, string>? Errors { get; set; }

    public static CommandResult Success()
    {
        return new CommandResult
        {
            IsSuccessful = true
        };
    }

    public static CommandResult Fail(string errorMessage)
    {
        return new CommandResult
        {
            IsSuccessful = false,
            ErrorMessage = errorMessage
        };
    }

    public static CommandResult Fail(string errorMessage, Dictionary<string, string> errors)
    {
        return new CommandResult
        {
            IsSuccessful = false,
            ErrorMessage = errorMessage,
            Errors = errors
        };
    }

}