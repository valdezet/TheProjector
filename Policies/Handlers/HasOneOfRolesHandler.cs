using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TheProjector.Policies.Requirements;

namespace TheProjector.Policies.Handlers;


public class HasOneOfRolesHandler : AuthorizationHandler<HasOneOfRolesRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasOneOfRolesRequirement requirement)
    {
        var user = context.User;
        var claims = user.FindAll(claim => claim.Type == ClaimTypes.Role);
        bool hasRole = false;
        foreach (var claim in claims)
        {
            if (requirement.RoleNames.Contains(claim.Value))
            {
                hasRole = true;
                break;
            }
        }
        if (hasRole)
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}