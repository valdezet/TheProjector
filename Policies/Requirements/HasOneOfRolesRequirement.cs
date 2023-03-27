using Microsoft.AspNetCore.Authorization;

namespace TheProjector.Policies.Requirements;


public class HasOneOfRolesRequirement : IAuthorizationRequirement
{
    public string[] RoleNames { get; }
    public HasOneOfRolesRequirement(string[] roleNames)
    {
        RoleNames = roleNames;
    }
}