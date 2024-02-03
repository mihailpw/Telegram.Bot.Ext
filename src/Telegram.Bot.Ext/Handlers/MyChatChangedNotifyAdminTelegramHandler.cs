using Telegram.Bot.Ext.Features.Users;
using Telegram.Bot.Ext.Features.Users.Models;
using Telegram.Bot.Ext.Handlers.Base;
using Telegram.Bot.Ext.Utils;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Handlers;

public class MyChatChangedNotifyAdminTelegramHandler : TelegramHandlerBase
{
    private readonly IUsersProvider _usersProvider;

    public MyChatChangedNotifyAdminTelegramHandler(IUsersProvider usersProvider)
    {
        _usersProvider = usersProvider;
    }

    protected override async Task<bool> HandleAsync(Update request, IHandleContext ctx, CancellationToken token)
    {
        if (request.MyChatMember is not { NewChatMember: not null, OldChatMember: not null } myChatMember)
            return false;

        await ctx.Bot.SendTextMessageAsync(_usersProvider, Role.Administrator,
            $"User update: {myChatMember.From.ToNameString()} (current: {myChatMember.NewChatMember.Status}; previous: {myChatMember.OldChatMember.Status})",
            cancellationToken: token);
        return true;
    }
}
