using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Features.Users.Repositories;

public class InMemoryUserRepository<TUser> : IUserRepository<TUser> where TUser : IUser
{
    private readonly HashSet<long> _savedUsers = new();
    private readonly List<TUser> _users = new();

    public async Task<bool> AddUserAsync(TUser user)
    {
        await Task.CompletedTask;
        if (!_savedUsers.Add(user.Id))
            return false;
        _users.Add(user);
        return true;
    }

    public IAsyncEnumerable<TUser> GetAllAwait()
        => _users.ToAsyncEnumerable();

    public Task<IReadOnlyCollection<TUser>> GetAllByRoleAsync(Role role)
        => Task.FromResult<IReadOnlyCollection<TUser>>(_users.Where(u => u.Role == role).ToList());

    public IAsyncEnumerable<TUser> GetAllByRoleAwait(Role role)
        => _users.Where(u => u.Role == role).ToAsyncEnumerable();

    public Task<TUser?> GetByIdAsync(long id)
        => Task.FromResult<TUser?>(_users.FirstOrDefault(u => u.Id == id));

    public Task<IReadOnlyCollection<TUser>> GetAllAsync()
        => Task.FromResult<IReadOnlyCollection<TUser>>(_users);

    public Task<bool> DeleteUserAsync(long id)
    {
        var index = _users.FindIndex(u => u.Id == id);
        if (index < 0)
            return Task.FromResult(false);

        _users.RemoveAt(index);
        _savedUsers.Remove(id);
        return Task.FromResult(true);
    }
}
