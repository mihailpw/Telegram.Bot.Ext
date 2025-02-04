using System.Text;
using Telegram.Bot.Ext.Features.Users;
using Telegram.Bot.Ext.Features.Users.Models;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Utils;

public static class TelegramExt
{
    public static async Task ForwardSentFirstMessageAsync<TMessages>(
        this Task<TMessages> messages,
        ITelegramBotClient bot,
        IUsersProvider usersProvider,
        Role role,
        CancellationToken token)
        where TMessages : IEnumerable<Message>
        => await ForwardSentFirstMessageAsync(await messages, bot, usersProvider, role, token);

    public static async Task ForwardSentFirstMessageAsync(
        this IEnumerable<Message> messages,
        ITelegramBotClient bot,
        IUsersProvider usersProvider,
        Role role,
        CancellationToken token)
    {
        var message = messages.FirstOrDefault();
        if (message is null)
            return;

        var userRole = await usersProvider.GetRoleAsync(message.Chat.Id);
        if (userRole is not Role.User)
            return;

        await bot.ForwardMessageAsync(usersProvider, role, message.Chat.Id, message.MessageId,
            cancellationToken: token);
    }

    public static string ToNameString(this User user, string? predict = default)
        => ToNameString(predict, user.FirstName, user.LastName, user.Username, user.Id);

    public static string ToNameString(this Chat chat, string? predict = default)
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