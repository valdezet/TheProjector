using TheProjector.Data.DTO;
using TheProjector.Data.DTO.Form;
using TheProjector.Data.Request;
using TheProjector.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace TheProjector.Services;


public class PersonService
{

    private TheProjectorDbContext _dbContext;

    public PersonService(TheProjectorDbContext dbContext)
    {
        // avoiding manual resolution of Db Context for now.
        _dbContext = dbContext;

    }


    public async Task<CommandResult> CreatePerson(PersonForm form)
    {
        try
        {
            Person newPerson = new Person
            {
                FirstName = form.FirstName,
                LastName = form.LastName
            };
            _dbContext.People.Add(newPerson);
            await _dbContext.SaveChangesAsync();
            return CommandResult.Success();
        }
        catch (Exception)
        {
            return CommandResult.Fail("There was an error in inserting to database.");
        }
    }
}