namespace TheProjector.Data.DTO;

public class CommandResult
{
    public bool IsSuccessful { get; private set; }

    public string? ErrorMessage { get; private set; }

    public Object? Data { get; set; }

    public static CommandResult Success()
    {
        return new CommandResult
        {
            IsSuccessful = true
        };
    }

    public static CommandResult Success(object data)
    {
        return new CommandResult
        {
            IsSuccessful = true,
            Data = data
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

}