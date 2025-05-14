using Telegram.Bot.Ext.Features.Users.Models;
using Telegram.Bot.Ext.Handlers.Base;
using Telegram.Bot.Ext.Utils;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Handlers;

public class LogAdminUnhandledTelegramHandler : TelegramHandlerBase
{
    protected override async Task<bool> HandleAsync(Update request, IHandleContext ctx, CancellationToken token)
    {
        if (request.Message is { Chat.Id: var chatId, MessageId: var messageId })
            await ctx.Bot.ForwardMessageAsync(Group.Administrator,
                chatId, messageId, cancellationToken: token);
        await ctx.Bot.SendTextMessageAsync(Group.Administrator,
            TryGetInfo(request) ?? $"Unknown update '{request.Type}' received",
            cancellationToken: token);
        return false;
    }

    private static string? TryGetInfo(Update update)
        => update switch
        {
            { Message: { } m } => $"Message received {GetInfo(m.Chat)}: {m.Text}",
            { EditedMessage: { } em } => $"Message edited {GetInfo(em.Chat)}: {em.Text}",
            { InlineQuery: { } iq } => $"Inline query {GetInfo(iq.From)}",
            { ChosenInlineResult: { } cir } => $"Chosen inline result {GetInfo(cir.From)}",
            { CallbackQuery: { } cq } => $"Callback query {GetInfo(cq.From)}",
            { ChannelPost: { } cp } => $"Channel post from {GetInfo(cp.Chat)}: {cp.Text}",
            { EditedChannelPost: { } ecp } => $"Edited channel post {GetInfo(ecp.From)}",
            { ShippingQuery: { } sq } => $"Shipping query {GetInfo(sq.From)}",
            { PreCheckoutQuery: { } pcq } => $"Pre checkout query {GetInfo(pcq.From)}",
            { Poll: not null } => "New poll",
            { PollAnswer: { } pa } => $"Poll answer {GetInfo(pa.User)}",
            { MyChatMember: { } mcm } => $"My chat member {GetInfo(mcm.Chat)}",
            { ChatMember: { } cm } => $"Chat member {GetInfo(cm.Chat)}",
            { ChatJoinRequest: { } cjr } => $"Chat join request query {GetInfo(cjr.From)}",
            _ => null
        };

    private static string GetInfo(User? user) => user != null ? user.ToNameString("by user") : "<null user>";
    private static string GetInfo(Chat chat) => chat.ToNameString("in chat");
}
