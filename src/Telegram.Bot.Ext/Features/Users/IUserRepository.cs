using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Features.Users;

public interface IUserRepository<TUser> where TUser : IUser
{
    Task<IReadOnlyCollection<TUser>> GetAllAsync();
    IAsyncEnumerable<TUser> GetAllAwait();
    Task<IReadOnlyCollection<TUser>> GetAllByRoleAsync(Role role);
    IAsyncEnumerable<TUser> GetAllByRoleAwait(Role role);
    Task<TUser?> GetByIdAsync(long id);

    Task<bool> AddUserAsync(TUser user);
    Task<bool> DeleteUserAsync(long id);
}