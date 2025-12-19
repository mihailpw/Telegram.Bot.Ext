using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.Messages;

public interface IMessageRemover
{
    Task InitializeAsync(CancellationToken token);
    Task<bool> CheckScheduledAsync(ChatMessageId id, CancellationToken token = default);
    Task CancelMessageRemovalAsync(ChatMessageId id, CancellationToken token = default);
    Task ScheduleForRemovalAsync(IReadOnlyCollection<ChatMessageId> ids, TimeSpan deleteIn, string groupKey = "default",
        CancellationToken token = default);
}

public readonly record struct ChatMessageId(ChatId ChatId, int MessageId);