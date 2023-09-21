using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Features.Users.Providers;

public sealed class NullUsersProvider : IUsersProvider
{
    public static readonly NullUsersProvider Instance = new();

    private NullUsersProvider() { }

    public Task<bool> CheckIfAsync(long id, Role role)
        => Task.FromResult(false);

    public Task<bool> CheckIfAsync(long id, Role[] oneOfRoles)
        => Task.FromResult(false);

    public Task<Role?> GetRoleAsync(long id)
        => Task.FromResult<Role?>(null);

    public IAsyncEnumerable<long> GetAllAwait(Role role)
        => AsyncEnumerable.Empty<long>();

    public Task<IReadOnlyCollection<long>> GetAllAsync(Role role)
        => Task.FromResult<IReadOnlyCollection<long>>(Array.Empty<long>());
}
