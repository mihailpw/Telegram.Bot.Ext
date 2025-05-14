using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Features.Users;

public interface IUsersProvider
{
    Task<bool> CheckIfAsync(long id, Group group);
    Task<bool> CheckIfAsync(long id, Group[] oneOfGroups);
    Task<Group?> GetGroupAsync(long id);
    Task<IReadOnlyCollection<Group>> GetGroupsAsync(long id);
    IAsyncEnumerable<long> GetAllAwait(Group group);
    Task<IReadOnlyCollection<long>> GetAllAsync(Group group);
}

public class InMemoryUsersProvider : IUsersProvider
{
    private readonly Dictionary<Group, HashSet<long>> _groups;
    private readonly Dictionary<long, HashSet<Group>> _users = new();

    public InMemoryUsersProvider(Dictionary<Group, HashSet<long>> groups)
    {
        _groups = groups;
        foreach (var (group, users) in groups)
        foreach (var user in users)
        {
            if (!_users.ContainsKey(user))
                _users.Add(user, new HashSet<Group>());
            _users[user].Add(group);
        }
    }

    public static InMemoryUsersProvider Create(Dictionary<string, List<long>> groups)
        => new(groups.ToDictionary(kvp => new Group(kvp.Key), kvp => new HashSet<long>(kvp.Value)));
    
    private HashSet<long> Get(Group group)
        => _groups.GetValueOrDefault(group, new HashSet<long>());
    
    private HashSet<Group> Get(long id)
        => _users.GetValueOrDefault(id, new HashSet<Group>());

    public Task<bool> CheckIfAsync(long id, Group group)
        => Task.FromResult(Get(group).Contains(id));

    public Task<bool> CheckIfAsync(long id, Group[] oneOfGroups)
    {
        var groups = Get(id);
        return Task.FromResult(oneOfGroups.Any(targetGroup => groups.Contains(targetGroup)));
    }

    public Task<Group?> GetGroupAsync(long id)
        => Task.FromResult<Group?>(Get(id).FirstOrDefault());

    public Task<IReadOnlyCollection<Group>> GetGroupsAsync(long id)
        => Task.FromResult<IReadOnlyCollection<Group>>(Get(id));

    public IAsyncEnumerable<long> GetAllAwait(Group group)
        => Get(group).ToAsyncEnumerable();

    public Task<IReadOnlyCollection<long>> GetAllAsync(Group group)
        => Task.FromResult<IReadOnlyCollection<long>>(Get(group));
}
