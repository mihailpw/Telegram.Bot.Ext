using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Features.Users;

public static class UsersProviderExt
{
    public static Task ForAllAdminsAsync(this IUsersProvider target, Func<long, Task> execute)
        => ForAllAsync(target, Role.Administrator, execute);
    public static Task ForAllModersAsync(this IUsersProvider target, Func<long, Task> execute)
        => ForAllAsync(target, Role.Moderator, execute);
    public static Task ForAllUsersAsync(this IUsersProvider target, Func<long, Task> execute)
        => ForAllAsync(target, Role.User, execute);

    public static async Task ForAllAsync(this IUsersProvider target, Role role, Func<long, Task> execute)
        => await Task.WhenAll(await target.GetAllAwait(role).Select(execute).ToListAsync());
    public static async Task<IReadOnlyList<T>> ForAllAsync<T>(this IUsersProvider target, Role role, Func<long, Task<T>> execute)
        => await Task.WhenAll(await target.GetAllAwait(role).Select(execute).ToListAsync());
}
