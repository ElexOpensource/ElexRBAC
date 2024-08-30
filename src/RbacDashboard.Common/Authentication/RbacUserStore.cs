using Microsoft.AspNetCore.Identity;
using RbacDashboard.DAL.Models.Domain;
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.Common.Authentication;

#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
#pragma warning disable CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.

[ExcludeFromCodeCoverage(Justification = "The 'RbacUserStore' class was specifically designed to handle 'SignInManager' with our custom model. Therefore, it is not necessary to create test cases for this class.")]
public class RbacUserStore : IUserStore<RbacApplicationUser>
{
    private readonly List<RbacApplicationUser> _users = [];

    public void Dispose() { }

    public Task<IdentityResult> CreateAsync(RbacApplicationUser user, CancellationToken cancellationToken)
    {
        user.CustomerId = Guid.NewGuid().ToString();

        _users.Add(user);

        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> UpdateAsync(RbacApplicationUser user, CancellationToken cancellationToken)
    {
        var match = _users.FirstOrDefault(u => u.CustomerId == user.CustomerId);
        if (match != null)
        {
            match.CustomerName = user.CustomerName;
            return Task.FromResult(IdentityResult.Success);
        }
        else
        {
            return Task.FromResult(IdentityResult.Failed());
        }
    }

    public Task<IdentityResult> DeleteAsync(RbacApplicationUser user, CancellationToken cancellationToken)
    {
        var match = _users.FirstOrDefault(u => u.CustomerId == user.CustomerId);
        if (match != null)
        {
            _users.Remove(match);

            return Task.FromResult(IdentityResult.Success);
        }
        else
        {
            return Task.FromResult(IdentityResult.Failed());
        }
    }

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
    public Task<RbacApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var user = _users.FirstOrDefault(u => u.CustomerId == userId);

        return Task.FromResult(user);
    }

    public Task<RbacApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        var user = _users.FirstOrDefault(user => user.CustomerId == normalizedUserName);
        return Task.FromResult(user);
    }
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.

    public Task<string> GetUserIdAsync(RbacApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.CustomerId);
    }

    public Task<string> GetUserNameAsync(RbacApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.CustomerName);
    }

    public Task<string> GetNormalizedUserNameAsync(RbacApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.CustomerName);
    }

    public Task<string> GetEmailAsync(RbacApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.CustomerName);
    }

    public Task SetUserNameAsync(RbacApplicationUser user, string userName, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.CustomerName);
    }

    public Task SetNormalizedUserNameAsync(RbacApplicationUser user, string normalizedName, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.CustomerName);
    }
}
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
#pragma warning restore CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
