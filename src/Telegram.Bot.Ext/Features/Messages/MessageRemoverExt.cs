using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.Messages;

public static class MessageRemoverExt
{
    public static void RemoveImmediately(this IMessageRemover target, ChatId chatId, int messageId, string groupKey = "")
        => target.ScheduleForRemoval(chatId, messageId, TimeSpan.Zero, groupKey);

    public static void ScheduleForRemoval(this IMessageRemover target, ChatId chatId, int messageId, DateTime deleteAt, string groupKey = "")
        => target.ScheduleForRemoval(chatId, messageId, deleteAt - DateTime.UtcNow, groupKey);

    public static void ScheduleForRemoval(this IMessageRemover target, Types.Message message, DateTime deleteAt, string groupKey = "")
        => target.ScheduleForRemoval(message.Chat.Id, message.MessageId, deleteAt - DateTime.UtcNow, groupKey);

    public static void ScheduleForRemoval(this IMessageRemover target, Types.Message message, TimeSpan deleteIn, string groupKey = "")
        => target.ScheduleForRemoval(message.Chat.Id, message.MessageId, deleteIn, groupKey);

    public static void ScheduleForRemoval(this IMessageRemover target, IEnumerable<Types.Message> messages, DateTime deleteAt, string groupKey = "")
        => target.ScheduleForRemoval(messages, deleteAt - DateTime.UtcNow, groupKey);

    public static void ScheduleForRemoval(this IMessageRemover target, IEnumerable<Types.Message> messages, TimeSpan deleteIn, string groupKey = "")
    {
        foreach (var message in messages)
        {
            target.ScheduleForRemoval(message.Chat.Id, message.MessageId, deleteIn, groupKey);
        }
    }
}
