using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Ext.Building.Internal;
using Telegram.Bot.Ext.Core;
using Telegram.Bot.Ext.States;
using Telegram.Bot.Ext.Utils;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext;

public interface ITelegramBot
{
    string Title { get; }
    string UserName { get; }
    ITelegramBotClient Client { get; }

    T GetFeature<T>() where T : notnull;
    void StartReceiving(ReceiverOptions? receiverOptions, CancellationToken token);
}

internal class TelegramBot : ITelegramBot
{
    private readonly ILogger<TelegramBot> _logger;
    private readonly IStatesRepository _statesRepository;
    private readonly TelegramHandlersExecutor _telegramHandlersExecutor;
    private readonly IServiceProvider _serviceProvider;

    private string? _title;
    private string? _userName;

    public TelegramBot(
        ILogger<TelegramBot> logger,
        ITelegramBotClient client,
        IStatesRepository statesRepository,
        TelegramHandlersExecutor telegramHandlersExecutor,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _statesRepository = statesRepository;
        _telegramHandlersExecutor = telegramHandlersExecutor;
        _serviceProvider = serviceProvider;
        Client = client;

        InitializeAsync().Forget();
    }

    public string Title => _title ?? throw new InvalidOperationException("Title not loaded");

    public string UserName => _userName ?? throw new InvalidOperationException("User name not loaded");

    public ITelegramBotClient Client { get; }

    public void StartReceiving(ReceiverOptions? receiverOptions, CancellationToken token)
    {
        Client.StartReceiving(HandleUpdateAsync, HandlePollingErrorAsync, receiverOptions, token);
    }

    public T GetFeature<T>() where T : notnull
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    private async Task InitializeAsync()
    {
        try
        {
            var botInfo = await Client.GetMeAsync();
            _title ??= $"{botInfo.FirstName} {botInfo.LastName}".Trim();
            _userName = botInfo.Username ?? throw new ArgumentNullException(nameof(botInfo.Username));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get bot info");
        }
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.GetChatId() is not {} chatId || update.GetUserId() is not { } userId)
        {
            _logger.LogWarning("Update {UpdateType} does not contain chatId or userId", update.Type);
            return;
        }

        try
        {
            var state = await _statesRepository.GetOrCreateAsync(chatId, userId);
            var ctx = new HandleContext(state, this);
            var reachedEnd = await _telegramHandlersExecutor.ExecuteAsync(update, ctx, cancellationToken);
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

    private Task HandlePollingErrorAsync(
        ITelegramBotClient botClient,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Polling updates error (bot_id={BotId})", botClient.BotId);
        return Task.CompletedTask;
    }
}
