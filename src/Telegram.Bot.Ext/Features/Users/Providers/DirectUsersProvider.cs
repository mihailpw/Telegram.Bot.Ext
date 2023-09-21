using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Features.Users.Providers;

public class DirectUsersProvider : IUsersProvider
{
    private readonly Dictionary<Role, IReadOnlyCollection<long>> _all;

    public DirectUsersProvider(
        IReadOnlyCollection<long>? admins = default,
        IReadOnlyCollection<long>? moderators = default,
        IReadOnlyCollection<long>? users = default)
    {
        _all = new Dictionary<Role, IReadOnlyCollection<long>>
        {
            [Role.Administrator] = admins ?? Array.Empty<long>(),
            [Role.Moderator] = moderators ?? Array.Empty<long>(),
            [Role.User] = users ?? Array.Empty<long>(),
        };
    }

    public Task<bool> CheckIfAsync(long id, Role role)
        => Task.FromResult(_all[role].Contains(id));

    public Task<bool> CheckIfAsync(long id, Role[] oneOfRoles)
        => Task.FromResult(oneOfRoles.Any(r => _all[r].Contains(id)));

    public Task<Role?> GetRoleAsync(long id)
    {
        foreach (var kvp in _all)
            if (kvp.Value.Contains(id))
                return Task.FromResult<Role?>(kvp.Key);

        return Task.FromResult<Role?>(null);
    }

    public IAsyncEnumerable<long> GetAllAwait(Role role)
        => _all[role].ToAsyncEnumerable();

    public Task<IReadOnlyCollection<long>> GetAllAsync(Role role)
        => Task.FromResult(_all[role]);
}
