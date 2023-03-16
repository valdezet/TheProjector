namespace TheProjector.Data.DTO;

public class CommandResult
{
    public bool IsSuccessful { get; private set; }

    public static CommandResult Success()
    {
        return new CommandResult
        {
            IsSuccessful = true
        };
    }

}