using Telegram.Bot.Ext.Features.Messages;
using Telegram.Bot.Ext.Features.Users.Models;
using Telegram.Bot.Ext.Handlers.Base;
using Telegram.Bot.Ext.Utils;
using Telegram.Bot.Ext.Utils.Formatting;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Ext.Handlers;

public class LogAdminUnhandledTelegramHandler : TelegramHandlerBase
{
    private readonly string? _sendMessageCommand;

    public LogAdminUnhandledTelegramHandler(string? sendMessageCommand = null)
    {
        _sendMessageCommand = sendMessageCommand;
    }

    protected override async Task<bool> HandleAsync(Update request, IHandleContext ctx, CancellationToken token)
    {
        if (await ctx.Bot.GetUsersProvider().CheckIfAsync(ctx.UserId, Group.Administrator))
            return false;

        if (request.Message is { Chat: var chat, MessageId: var messageId })
        {
            await ctx.Bot.ForwardMessageAsync(Group.Administrator,
                chat.Id, messageId, cancellationToken: token);

            var adminText = new MarkdownV2Formatting()
                .WithText("Received message from ").WithUserLinkOrName(chat).WithText(".");
            if (_sendMessageCommand != null)
                adminText.WithNewLine().WithText($"Respond /{_sendMessageCommand}_{chat.Id}");

            var sentAdminMsgs = await ctx.Bot.SendTextMessageAsync(Group.Administrator,
                adminText.Build(), parseMode: adminText.TelegramParseMode, cancellationToken: token);
            ctx.Bot.GetMessageRemover().ScheduleForRemoval(sentAdminMsgs, TimeSpan.FromHours(1));
        }
        else
        {
            await ctx.Bot.SendTextMessageAsync(Group.Administrator,
                TryGetInfo(request) ?? $"Unknown update '{request.Type}' received",
                cancellationToken: token);
        }
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
