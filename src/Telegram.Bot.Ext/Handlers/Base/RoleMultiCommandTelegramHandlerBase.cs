using Telegram.Bot.Ext.Features.Users.Models;

namespace Telegram.Bot.Ext.Handlers.Base;

public abstract class RoleMultiCommandTelegramHandlerBase : MultiCommandTelegramHandlerBase
{
    protected void RegisterCommand(
        string command,
        Handle handler,
        Role role)
    {
        RegisterCommand(
            command,
            handler,
            (message, ctx, _) => ctx.Bot.GetUsersProvider().CheckIfAsync(message.From!.Id, role));
    }

    protected void RegisterCommand(
        string command,
        Handle handler,
        params Role[] roles)
    {
        RegisterCommand(
            command,
            handler,
            async (message, ctx, _) =>
            {
                var userRole = await ctx.Bot.GetUsersProvider().GetRoleAsync(message.From!.Id);
                return userRole.HasValue && roles.Contains(userRole.Value);
            });
    }
}