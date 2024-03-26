using Microsoft.Extensions.Logging;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Ext.Core;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.Messages;

public class MessageRemover : IMessageRemover
{
    private readonly record struct ChatMessageId(ChatId ChatId, int MessageId, string GroupKey);

    private readonly SemaphoreSlim _lock = new(1);

    private readonly ILogger<MessageRemover> _logger;
    private readonly ITelegramBotClient _bot;
    private readonly Dictionary<ChatMessageId, Timer> _scheduledToRemove = new();

    public MessageRemover(
        ILogger<MessageRemover> logger,
        ITelegramBotClient bot)
    {
        _logger = logger;
        _bot = bot;
    }

    public bool CheckScheduled(ChatId chatId, int messageId, string groupKey = "")
    {
        ArgumentNullException.ThrowIfNull(groupKey);

        using (_lock.Lock())
        {
            return _scheduledToRemove.ContainsKey(new ChatMessageId(chatId, messageId, groupKey));
        }
    }

    public bool CancelMessageRemoval(ChatId chatId, int messageId, string groupKey = "")
    {
        ArgumentNullException.ThrowIfNull(groupKey);

        using (_lock.Lock())
        {
            return _scheduledToRemove.Remove(new ChatMessageId(chatId, messageId, groupKey));
        }
    }

    public void ScheduleForRemoval(ChatId chatId, int messageId, TimeSpan deleteIn, string groupKey = "")
    {
        ArgumentNullException.ThrowIfNull(groupKey);

        using (_lock.Lock())
        {
            var chatMessageId = new ChatMessageId(chatId, messageId, groupKey);
            if (_scheduledToRemove.TryGetValue(chatMessageId, out var value))
            {
                _logger.LogInformation("Rescheduling message removal (chatId={ChatId}, messageId={MessageId}",
                    chatId, messageId);
                value.Dispose();
                _scheduledToRemove.Remove(chatMessageId);
            }

            if (deleteIn <= TimeSpan.FromSeconds(1))
            {
                RemoveMessage(new ChatMessageId(chatId, messageId, groupKey), force: true);
                return;
            }

            _scheduledToRemove[chatMessageId] = new Timer(
                state => RemoveMessage((ChatMessageId)state!),
                chatMessageId,
                deleteIn,
                TimeSpan.Zero);
        }
    }

    public void RemoveAllImmediately(ChatId? chatId = null, int? messageId = null, string? groupKey = null)
    {
        foreach (var key in _scheduledToRemove.Keys)
            if ((chatId == null || key.ChatId == chatId)
                && (messageId == null || key.MessageId == messageId)
                && (groupKey == null || key.GroupKey == groupKey))
                RemoveMessage(key);
    }

    private async void RemoveMessage(ChatMessageId chatMessageId, bool force = false)
    {
        try
        {
            using (await _lock.LockAsync())
            {
                if (!force && !_scheduledToRemove.ContainsKey(chatMessageId))
                    return;

                await _bot.DeleteMessageAsync(chatMessageId.ChatId, chatMessageId.MessageId);
                _scheduledToRemove.Remove(chatMessageId);
            }
        }
        catch (ApiRequestException ex)
        {
            _logger.LogTrace(ex, "Message was already deleted (probably)");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Message was not deleted");
        }
    }
}
