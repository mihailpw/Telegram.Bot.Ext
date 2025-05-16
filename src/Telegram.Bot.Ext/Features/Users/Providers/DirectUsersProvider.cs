using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Features.Users.Providers;

public class DirectUsersProvider : IUsersProvider
{
    private readonly Dictionary<Group, IReadOnlyCollection<long>> _all;
    private readonly HashSet<long> _allUsers = new();

    public DirectUsersProvider(
        IReadOnlyCollection<long>? admins = null,
        IReadOnlyCollection<long>? users = null)
    {
        _all = new Dictionary<Group, IReadOnlyCollection<long>>
        {
            [Group.Administrator] = admins ?? Array.Empty<long>(),
            [Group.User] = users ?? Array.Empty<long>(),
        };
        _allUsers.UnionWith(admins ?? Array.Empty<long>());
        _allUsers.UnionWith(users ?? Array.Empty<long>());
    }

    private IReadOnlyCollection<long> Get(Group group)
        => _all.GetValueOrDefault(group, Array.Empty<long>());

    public Task<bool> CheckIfAsync(long id, Group group)
        => Task.FromResult(Get(group).Contains(id));

    public Task<bool> CheckIfAsync(long id, Group[] oneOfGroups)
        => Task.FromResult(oneOfGroups.Any(r => _all[r].Contains(id)));

    public Task<Group?> GetGroupAsync(long id)
    {
        foreach (var kvp in _all)
            if (kvp.Value.Contains(id))
                return Task.FromResult<Group?>(kvp.Key);

        return Task.FromResult<Group?>(null);
    }

    public async Task<IReadOnlyCollection<Group>> GetGroupsAsync(long id)
    {
        var group = await GetGroupAsync(id);
        return group.HasValue ? new[] { group.Value } : Array.Empty<Group>();
    }

    public IAsyncEnumerable<long> GetAllAwait()
        => _allUsers.ToAsyncEnumerable();

    public IAsyncEnumerable<long> GetAllAwait(Group group)
        => Get(group).ToAsyncEnumerable();

    public Task<IReadOnlyCollection<long>> GetAllAsync(Group group)
        => Task.FromResult(Get(group));
}
