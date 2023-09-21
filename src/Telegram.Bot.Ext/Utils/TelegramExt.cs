using System.Text;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Utils;

public static class TelegramExt
{
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