using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Ext.Building.Internal;
using Telegram.Bot.Ext.States;

namespace Telegram.Bot.Ext.Building;

public class TelegramBotBuilder
{
    private readonly string _token;
    private Func<HttpClient>? _httpClientFactory;

    private readonly TelegramHandlersExecutorBuilder _telegramHandlersExecutorBuilder = new();
    private Func<IServiceProvider, IStatesRepository>? _statesRepositoryFactory;

    public TelegramBotBuilder(string token)
    {
        _token = token;
    }

    public TelegramBotBuilder WithHttpClient(Func<HttpClient> httpClientFactory)
        => With(() => _httpClientFactory = httpClientFactory);

    public TelegramBotBuilder WithStatesRepository(Func<IServiceProvider, IStatesRepository> statesRepositoryFactory)
        => With(() => _statesRepositoryFactory = statesRepositoryFactory);

    public TelegramBotBuilder WithHandlers(Action<ITelegramHandlersBuilder> buildAction)
        => With(() => buildAction(_telegramHandlersExecutorBuilder));

    public ITelegramBot Build(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var httpClient = _httpClientFactory?.Invoke() ?? serviceProvider.GetService<HttpClient>();
        var statesRepository = _statesRepositoryFactory?.Invoke(serviceProvider)
                               ?? serviceProvider.GetService<IStatesRepository>()
                               ?? new InMemoryStatesRepository();
        var bot = new TelegramBot(
            serviceProvider.GetRequiredService<ILogger<TelegramBot>>(),
            new TelegramBotClient(_token, httpClient),
            statesRepository,
            _telegramHandlersExecutorBuilder.Build(serviceProvider),
            serviceProvider);

        return bot;
    }

    private TelegramBotBuilder With(Action action)
    {
        action();
        return this;
    }
}