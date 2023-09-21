using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Features.Users;

public interface IUsersProvider
{
    Task<bool> CheckIfAsync(long id, Role role);
    Task<bool> CheckIfAsync(long id, Role[] oneOfRoles);
    Task<Role?> GetRoleAsync(long id);
    IAsyncEnumerable<long> GetAllAwait(Role role);
    Task<IReadOnlyCollection<long>> GetAllAsync(Role role);
}
