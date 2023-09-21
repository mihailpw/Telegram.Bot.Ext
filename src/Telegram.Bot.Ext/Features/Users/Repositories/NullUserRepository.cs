using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Features.Users.Repositories;

public class NullUserRepository<TUser> : IUserRepository<TUser> where TUser : IUser
{
    public static readonly NullUserRepository<TUser> Instance = new();

    private NullUserRepository()
    {
    }

    public Task<bool> AddUserAsync(TUser user)
        => Task.FromResult(false);

    public IAsyncEnumerable<TUser> GetAllAwait()
        => AsyncEnumerable.Empty<TUser>();

    public Task<IReadOnlyCollection<TUser>> GetAllByRoleAsync(Role role)
        => Task.FromResult<IReadOnlyCollection<TUser>>(Array.Empty<TUser>());

    public IAsyncEnumerable<TUser> GetAllByRoleAwait(Role role)
        => AsyncEnumerable.Empty<TUser>();

    public Task<TUser?> GetByIdAsync(long id)
        => Task.FromResult<TUser?>(default);

    public Task<IReadOnlyCollection<TUser>> GetAllAsync()
        => Task.FromResult<IReadOnlyCollection<TUser>>(Array.Empty<TUser>());

    public Task<bool> DeleteUserAsync(long id)
        => Task.FromResult(false);
}
