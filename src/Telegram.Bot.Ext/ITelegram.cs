using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Ext.Core;

namespace Telegram.Bot.Ext;

public interface ITelegramBot
{
    string Title { get; }
    string UserName { get; }
    ITelegramBotClient Client { get; }

    T GetFeature<T>() where T : notnull;
}

public class TelegramBot : ITelegramBot
{
    private readonly ILogger<TelegramBot> _logger;
    private readonly IServiceProvider _serviceProvider;

    private string? _title;
    private string? _userName;

    public TelegramBot(ILogger<TelegramBot> logger, ITelegramBotClient client, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        Client = client;

        //_title = title;
        SetupAsync().Forget();
    }

    public string Title => _title ?? throw new InvalidOperationException("Title not loaded");

    public string UserName => _userName ?? throw new InvalidOperationException("User name not loaded");

    public ITelegramBotClient Client { get; }

    public T GetFeature<T>() where T : notnull
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    private async Task SetupAsync()
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
}
