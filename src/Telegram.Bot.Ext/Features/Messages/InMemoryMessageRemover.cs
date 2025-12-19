using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Telegram.Bot.Ext.Features.Messages;

public class InMemoryMessageRemover : MessageRemoverBase
{
    private readonly ConcurrentDictionary<ChatMessageId, DateTime> _scheduledToRemove = new();

    public InMemoryMessageRemover(ILoggerFactory loggerFactory, ITelegramBotClient bot)
        : base(loggerFactory, bot, TimeSpan.FromSeconds(1))
    {
    }

    protected override Task InitializeInternalAsync(CancellationToken token) => Task.CompletedTask;

    protected override Task<bool> CheckScheduledInternalAsync(ChatMessageId chatMessageId, CancellationToken token)
        => Task.FromResult(_scheduledToRemove.ContainsKey(chatMessageId));

    protected override Task<bool> CancelMessageRemovalInternalAsync(ChatMessageId chatMessageId,
        CancellationToken token)
        => Task.FromResult(_scheduledToRemove.TryRemove(chatMessageId, out _));

    protected override Task ScheduleForRemovalInternalAsync(IReadOnlyCollection<ChatMessageId> ids,
        string groupKey, TimeSpan deleteIn, CancellationToken token)
    {
        var deleteAt = DateTime.UtcNow + deleteIn;
        foreach (var id in ids)
            _scheduledToRemove[id] = deleteAt;
        return Task.CompletedTask;
    }

    protected override Task<IEnumerable<ChatMessageId>> GetMessagesToRemoveAsync(DateTime time, CancellationToken token)
    {
        var toRemove = _scheduledToRemove
            .Where(kvp => kvp.Value <= time)
            .Select(kvp => kvp.Key)
            .ToList();

        return Task.FromResult<IEnumerable<ChatMessageId>>(toRemove);
    }

    protected override Task MarkMessagesRemovedAsync(DateTime time, CancellationToken token)
    {
        foreach (var kvp in _scheduledToRemove)
            if (kvp.Value <= time)
                _scheduledToRemove.TryRemove(kvp.Key, out _);

        return Task.CompletedTask;
    }
}