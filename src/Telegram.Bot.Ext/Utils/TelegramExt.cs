using System.Text;
using Telegram.Bot.Ext.Features.Users.Models;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Utils;

public static class TelegramExt
{
    public static async Task ForwardSentFirstMessageAsync<TMessages>(this Task<TMessages> messages, ITelegramBot bot,
        Group group, CancellationToken token)
        where TMessages : IEnumerable<Message>
        => await ForwardSentFirstMessageAsync(await messages, bot, group, token);

    public static Task ForwardSentFirstMessageAsync(this IEnumerable<Message> messages, ITelegramBot bot,
        Group group, CancellationToken token)
        => ForwardSentMessageAsync(messages.FirstOrDefault(), bot, group, token);

    public static async Task ForwardSentMessageAsync(this Task<Message> message, ITelegramBot bot,
        Group group, CancellationToken token)
        => await ForwardSentMessageAsync(await message, bot, group, token);

    public static async Task ForwardSentMessageAsync(this Message? message, ITelegramBot bot,
        Group group, CancellationToken token)
    {
        if (message == null)
            return;

        var usersProvider = bot.GetUsersProvider();
        var userGroup = await usersProvider.GetGroupAsync(message.Chat.Id);
        if (userGroup != Group.User)
            return;

        await bot.ForwardMessageAsync(group, message.Chat.Id, message.MessageId, cancellationToken: token);
    }

    public static string ToNameString(this User user, string? predict = null)
        => ToNameString(predict, user.FirstName, user.LastName, user.Username, user.Id);

    public static string ToNameString(this Chat chat, string? predict = null)
        => ToNameString(predict, chat.FirstName, chat.LastName, chat.Username, chat.Id);

    private static string ToNameString(string? predict, string? firstName, string? lastName, string? userName, long id)
    {
        var sb = new StringBuilder(predict);
        if (!string.IsNullOrEmpty(predict))
            sb.Append(' ');
        
        if (!string.IsNullOrEmpty(firstName))
        {
            sb.Append(firstName);
        }

        if (!string.IsNullOrEmpty(lastName))
        {
            if (!string.IsNullOrEmpty(firstName))
                sb.Append(' ');
            sb.Append(lastName);
        }

        if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
            sb.Append("<unnamed>");

        sb.Append(" (");
        if (!string.IsNullOrEmpty(userName))
        {
            sb.Append(userName);
            sb.Append('/');
        }

        sb.Append(id);
        sb.Append(")");
        return sb.ToString();
    }
}