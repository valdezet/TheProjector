using TheProjector.Data.DTO;
using TheProjector.Data.Form;
using TheProjector.Data.Persistence;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace TheProjector.Services;

public class AuthService
{
    private TheProjectorDbContext _dbContext;
    private PasswordService _passwordService;
    public AuthService(
        TheProjectorDbContext dbContext,
        PasswordService passwordService)
    {
        _dbContext = dbContext;
        _passwordService = passwordService;
    }

    public async Task<CommandResult> AuthenticateUser(LoginForm form)
    {

        string emailNormalized = form.Email.ToUpper();
        User? user = await _dbContext.Users.Where(user => emailNormalized == user.EmailNormalized).FirstOrDefaultAsync();
        if (user == null)
        {
            return CommandResult.Fail("Invalid Login Credentials.");
        }
        bool isCorrect = _passwordService.VerifyPassword(form.Password, user.PasswordHash);
        if (isCorrect)
        {
            return CommandResult.Success();
        }
        else
        {
            return CommandResult.Fail("Invalid Login Credentials.");
        }
    }

    public async Task<List<Claim>?> BuildClaimsFor(string email)
    {
        string emailNormalized = email.ToUpper();
        User? user = await _dbContext.Users.Where(user => emailNormalized == user.EmailNormalized).FirstOrDefaultAsync();
        if (user == null)
        {
            return null;
        }
        Person person = await _dbContext.People
            .Include(person => person.Roles)
            .Where(user => user.UserId == user.Id)
            .FirstAsync();
        List<Claim> claims = new List<Claim>() {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, person.FullName),
        };
        foreach (var role in person.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));
        }
        return claims;


    }
}