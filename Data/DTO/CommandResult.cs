namespace TheProjector.Data.DTO;

public class CommandResult
{
    public bool IsSuccessful { get; private set; }

    public List<string>? ErrorMessages { get; private set; }

    public static CommandResult Success()
    {
        return new CommandResult
        {
            IsSuccessful = true
        };
    }

    public static CommandResult Fail(List<string> errorMessages)
    {
        return new CommandResult
        {
            IsSuccessful = false,
            ErrorMessages = errorMessages
        };
    }

    public static CommandResult Fail(string errorMessage)
    {
        List<string> errors = new List<string>();
        errors.Add(errorMessage);
        return Fail(errors);
    }

}