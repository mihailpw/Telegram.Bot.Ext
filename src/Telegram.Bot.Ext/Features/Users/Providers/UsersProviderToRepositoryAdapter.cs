using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Features.Users.Providers;

public class UsersProviderToRepositoryAdapter<TUser> : IUsersProvider where TUser : IUser
{
    private readonly IUserRepository<TUser> _userRepository;
    private readonly IReadOnlyCollection<long> _admins;

    public UsersProviderToRepositoryAdapter(
        IUserRepository<TUser> userRepository,
        IReadOnlyCollection<long> admins)
    {
        _userRepository = userRepository;
        _admins = admins;
    }

    public async Task<bool> CheckIfAsync(long id, Group group)
    {
        if (group == Group.Administrator)
            return _admins.Contains(id);

        var user = await _userRepository.GetByIdAsync(id);
        return user?.Group == group;
    }

    public async Task<bool> CheckIfAsync(long id, Group[] oneOfGroups)
    {
        if (oneOfGroups.Contains(Group.Administrator))
            return _admins.Contains(id);

        var user = await _userRepository.GetByIdAsync(id);
        return user is not null && oneOfGroups.Contains(user.Group);
    }

    public async Task<Group?> GetGroupAsync(long id)
    {
        if (_admins.Contains(id))
            return Group.Administrator;

        var user = await _userRepository.GetByIdAsync(id);
        return user?.Group;
    }

    public async Task<IReadOnlyCollection<Group>> GetGroupsAsync(long id)
    {
        if (_admins.Contains(id))
            return new[] { Group.Administrator };

        var user = await _userRepository.GetByIdAsync(id);
        return user is not null ? new[] { user.Group } : Array.Empty<Group>();
    }

    public async IAsyncEnumerable<long> GetAllAwait()
    {
        var seen = new HashSet<long>();
        foreach (var admin in _admins)
            if (seen.Add(admin))
                yield return admin;
        await foreach (var user in _userRepository.GetAllAwait())
            if (seen.Add(user.Id))
                yield return user.Id;
    }

    public IAsyncEnumerable<long> GetAllAwait(Group group)
    {
        return group == Group.Administrator
            ? _admins.ToAsyncEnumerable()
            : _userRepository.GetAllByGroupAwait(group).Select(u => u.Id);
    }

    public async Task<IReadOnlyCollection<long>> GetAllAsync(Group group)
    {
        if (group == Group.Administrator)
            return _admins;

        var users = await _userRepository.GetAllByGroupAsync(group);
        return users.Select(u => u.Id).ToList();
    }
}

public class UsersProviderToRepositoryAdapter2<TUser> : IUsersProvider where TUser : IUser
{
    private readonly IUserRepository<TUser> _userRepository;

    public UsersProviderToRepositoryAdapter2(IUserRepository<TUser> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> CheckIfAsync(long id, Group group)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user?.Group == group;
    }

    public async Task<bool> CheckIfAsync(long id, Group[] oneOfGroups)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user is not null && oneOfGroups.Contains(user.Group);
    }

    public async Task<Group?> GetGroupAsync(long id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user?.Group;
    }

    public async Task<IReadOnlyCollection<Group>> GetGroupsAsync(long id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user is not null ? new[] { user.Group } : Array.Empty<Group>();
    }

    public IAsyncEnumerable<long> GetAllAwait()
    {
        return _userRepository.GetAllAwait().Select(u => u.Id);
    }

    public IAsyncEnumerable<long> GetAllAwait(Group group)
    {
        return _userRepository.GetAllByGroupAwait(group).Select(u => u.Id);
    }

    public async Task<IReadOnlyCollection<long>> GetAllAsync(Group group)
    {
        var users = await _userRepository.GetAllByGroupAsync(group);
        return users.Select(u => u.Id).ToList();
    }
}
