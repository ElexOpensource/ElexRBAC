using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.Common.Authentication;

[ExcludeFromCodeCoverage(Justification = "The 'RbacRoleStore' class was specifically designed to handle 'SignInManager' with our custom model. Therefore, it is not necessary to create test cases for this class.")]
public class RbacRoleStore : IRoleStore<IdentityRole>
{
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
#pragma warning disable CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
    private readonly List<IdentityRole> _roles = [];

    public Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        _roles.Add(role);

        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        var match = _roles.FirstOrDefault(r => r.Id == role.Id);
        if (match != null)
        {
            match.Name = role.Name;

            return Task.FromResult(IdentityResult.Success);
        }
        else
        {
            return Task.FromResult(IdentityResult.Failed());
        }
    }

    public Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        var match = _roles.FirstOrDefault(r => r.Id == role.Id);
        if (match != null)
        {
            _roles.Remove(match);

            return Task.FromResult(IdentityResult.Success);
        }
        else
        {
            return Task.FromResult(IdentityResult.Failed());
        }
    }

    public Task<IdentityRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        var role = _roles.FirstOrDefault(r => r.Id == roleId);

        return Task.FromResult(role);
    }

    public Task<IdentityRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        var role = _roles.FirstOrDefault(r => string.Equals(r.NormalizedName, normalizedRoleName, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(role);
    }

    public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.Id);
    }

    public Task<string> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.NormalizedName);
    }

    public Task<string> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.NormalizedName);
    }

    public Task SetRoleNameAsync(IdentityRole role, string roleName, CancellationToken cancellationToken)
    {
        role.Name = roleName;

        return Task.FromResult(true);
    }

    public Task SetNormalizedRoleNameAsync(IdentityRole role, string normalizedName, CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning restore CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.

    public void Dispose() { }
}
