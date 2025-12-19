using Microsoft.Extensions.Logging;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Ext.Core;

namespace Telegram.Bot.Ext.Features.Messages;

public abstract class MessageRemoverBase : IMessageRemover, IAsyncDisposable
{
    private readonly SemaphoreSlim _lock = new(1, 1);

    private readonly ILogger _logger;
    private readonly ITelegramBotClient _bot;
    private readonly TimeSpan _checkInterval;

    private CancellationTokenSource? _cycleCts;
    private Task? _cycleTask;

    protected MessageRemoverBase(ILoggerFactory loggerFactory, ITelegramBotClient bot, TimeSpan checkInterval)
    {
        _logger = loggerFactory.CreateLogger(GetType());
        _bot = bot;
        _checkInterval = checkInterval;
    }

    protected bool IsInitialized => _cycleTask != null;

    public async Task InitializeAsync(CancellationToken token)
    {
        if (!IsInitialized)
        {
            using (await _lock.LockAsync(token))
            {
                if (!IsInitialized)
                {
                    await InitializeInternalAsync(token);
                    _cycleCts = CancellationTokenSource.CreateLinkedTokenSource(token);
                    _cycleTask = RunCycleAsync(_cycleCts.Token);
                    return;
                }
            }
        }

        _logger.LogWarning("The message remover is already initialized.");
    }

    public async Task<bool> CheckScheduledAsync(ChatMessageId id, CancellationToken token = default)
    {
        if (!IsInitialized)
            await InitializeAsync(token);

        using (await _lock.LockAsync(token))
            return await CheckScheduledInternalAsync(id, token);
    }

    public async Task CancelMessageRemovalAsync(ChatMessageId id, CancellationToken token = default)
    {
        if (!IsInitialized)
            await InitializeAsync(token);

        using (await _lock.LockAsync(token))
            await CancelMessageRemovalInternalAsync(id, token);
    }

    public async Task ScheduleForRemovalAsync(IReadOnlyCollection<ChatMessageId> ids, TimeSpan deleteIn,
        string groupKey = "default", CancellationToken token = default)
    {
        if (!IsInitialized)
            await InitializeAsync(CancellationToken.None);

        ArgumentNullException.ThrowIfNull(groupKey);

        if (deleteIn <= _checkInterval)
        {
            await Task.WhenAll(ids.Select(id => RemoveMessageAsync(id, CancellationToken.None)));
            return;
        }

        using (await _lock.LockAsync(token))
            await ScheduleForRemovalInternalAsync(ids, groupKey, deleteIn, token);
    }

    public async ValueTask DisposeAsync()
    {
        _lock.Dispose();
        if (_cycleCts != null)
        {
            await _cycleCts.CancelAsync();
            _cycleCts.Dispose();
            _cycleCts = null;
        }
        if (_cycleTask != null)
        {
            await _cycleTask;
            _cycleTask = null;
        }
    }

    protected abstract Task InitializeInternalAsync(CancellationToken token);
    protected abstract Task<bool> CheckScheduledInternalAsync(ChatMessageId chatMessageId, CancellationToken token);
    protected abstract Task<bool> CancelMessageRemovalInternalAsync(ChatMessageId chatMessageId, CancellationToken token);
    protected abstract Task ScheduleForRemovalInternalAsync(IReadOnlyCollection<ChatMessageId> ids,
        string groupKey, TimeSpan deleteIn, CancellationToken token);

    protected abstract Task<IEnumerable<ChatMessageId>> GetMessagesToRemoveAsync(DateTime time, CancellationToken token);
    protected abstract Task MarkMessagesRemovedAsync(DateTime time, CancellationToken token);

    private async Task RunCycleAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.UtcNow;
                var messagesToRemove = await GetMessagesToRemoveAsync(now, token);
                var count = 0;
                await Task.WhenAll(messagesToRemove.Select(cm =>
                {
                    count++;
                    return RemoveMessageAsync(cm, token);
                }));
                if (count > 0)
                    await MarkMessagesRemovedAsync(now, token);
                await Task.Delay(_checkInterval, token).Safe();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in message remover cycle");
            }
        }
    }

    private async Task RemoveMessageAsync(ChatMessageId chatMessageId, CancellationToken token)
    {
        try
        {
            await _bot.DeleteMessageAsync(chatMessageId.ChatId, chatMessageId.MessageId, cancellationToken: token);
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
