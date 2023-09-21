using Microsoft.Extensions.Logging;
using Telegram.Bot.Ext.Building.Internal;
using Telegram.Bot.Ext.States;
using Telegram.Bot.Ext.Utils;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Building;

internal class TelegramUpdateHandler : IUpdateHandler
{
    private readonly ILogger<TelegramUpdateHandler> _logger;
    private readonly IStatesRepository _statesRepository;
    private readonly HandlersExecutor _handlersExecutor;

    public TelegramUpdateHandler(
        ILogger<TelegramUpdateHandler> logger,
        IStatesRepository statesRepository,
        HandlersExecutor handlersExecutor)
    {
        _logger = logger;
        _statesRepository = statesRepository;
        _handlersExecutor = handlersExecutor;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.GetChatId() is not {} chatId || update.GetUserId() is not { } userId)
        {
            _logger.LogWarning("Update {UpdateType} does not contain chatId or userId", update.Type);
            return;
        }

        try
        {
            var state = await _statesRepository.GetOrCreateAsync(chatId, userId);
            var ctx = new HandleContext(state, botClient);
            var reachedEnd = await _handlersExecutor.ExecuteAsync(update, ctx, cancellationToken);
            await _statesRepository.SaveAsync(state);

            if (reachedEnd)
            {
                _logger.LogInformation("Update {UpdateType} from chat {ChatId} was not handled or handled partially", update.Type, chatId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handling error");
        }
    }

    public Task HandlePollingErrorAsync(
        ITelegramBotClient botClient,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Polling updates error (bot_id={BotId})", botClient.BotId);
        return Task.CompletedTask;
    }
}
