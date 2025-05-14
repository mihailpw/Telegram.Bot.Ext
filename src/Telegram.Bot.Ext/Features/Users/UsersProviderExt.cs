using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Features.Users;

public static class UsersProviderExt
{
    public static Task ForAllAdminsAsync(this IUsersProvider target, Func<long, Task> execute)
        => ForAllAsync(target, Group.Administrator, execute);
    public static Task ForAllUsersAsync(this IUsersProvider target, Func<long, Task> execute)
        => ForAllAsync(target, Group.User, execute);

    public static async Task ForAllAsync(this IUsersProvider target, Group group, Func<long, Task> execute)
        => await Task.WhenAll(await target.GetAllAwait(group).Select(execute).ToListAsync());
    public static async Task<IReadOnlyList<T>> ForAllAsync<T>(this IUsersProvider target, Group group, Func<long, Task<T>> execute)
        => await Task.WhenAll(await target.GetAllAwait(group).Select(execute).ToListAsync());
}
