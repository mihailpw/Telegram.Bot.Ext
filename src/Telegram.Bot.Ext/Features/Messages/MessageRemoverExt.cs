using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.Messages;

public static class MessageRemoverExt
{
    public static Task RemoveImmediately(this IMessageRemover target, ChatId chatId, int messageId, string groupKey = "default", CancellationToken token = default)
        => target.ScheduleForRemovalAsync(chatId, messageId, TimeSpan.Zero, groupKey, token);

    public static Task ScheduleForRemovalAsync(this IMessageRemover target, ChatId chatId, int messageId, DateTime deleteAt, string groupKey = "default", CancellationToken token = default)
        => target.ScheduleForRemovalAsync(chatId, messageId, deleteAt - DateTime.UtcNow, groupKey, token);

    public static Task ScheduleForRemovalAsync(this IMessageRemover target, Types.Message message, DateTime deleteAt, string groupKey = "default", CancellationToken token = default)
        => target.ScheduleForRemovalAsync(message.Chat.Id, message.MessageId, deleteAt - DateTime.UtcNow, groupKey, token);

    public static Task ScheduleForRemovalAsync(this IMessageRemover target, Types.Message message, TimeSpan deleteIn, string groupKey = "default", CancellationToken token = default)
        => target.ScheduleForRemovalAsync(message.Chat.Id, message.MessageId, deleteIn, groupKey, token);

    public static Task ScheduleForRemovalAsync(this IMessageRemover target, IEnumerable<Types.Message> messages, DateTime deleteAt, string groupKey = "default", CancellationToken token = default)
        => target.ScheduleForRemovalAsync(messages, deleteAt - DateTime.UtcNow, groupKey: groupKey, token: token);
    
    public static Task<bool> CheckScheduledAsync(this IMessageRemover target, ChatId chatId, int messageId, CancellationToken token = default)
        => target.CheckScheduledAsync(new ChatMessageId(chatId, messageId), token);

    public static Task CancelMessageRemovalAsync(this IMessageRemover target, ChatId chatId, int messageId, CancellationToken token = default)
        => target.CancelMessageRemovalAsync(new ChatMessageId(chatId, messageId), token);

    public static Task ScheduleForRemovalAsync(this IMessageRemover target, ChatId chatId, int messageId,
        TimeSpan deleteIn, string groupKey = "default", CancellationToken token = default)
        => target.ScheduleForRemovalAsync([new ChatMessageId(chatId, messageId)], deleteIn, groupKey, token);

    public static Task ScheduleForRemovalAsync(this IMessageRemover target, IEnumerable<Types.Message> messages,
        TimeSpan deleteIn, string groupKey = "default", CancellationToken token = default)
        => target.ScheduleForRemovalAsync(messages.Select(m => new ChatMessageId(m.Chat.Id, m.MessageId)).ToList(),
            deleteIn, groupKey, token);
}
