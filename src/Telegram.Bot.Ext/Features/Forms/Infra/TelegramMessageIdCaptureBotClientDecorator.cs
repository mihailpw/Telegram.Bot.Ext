using System.IO;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.Forms.Infra;

internal sealed class TelegramMessageIdCaptureBotClientDecorator : ITelegramBotClient
{
    private readonly ITelegramBotClient _inner;
    private readonly Action<int> _onMessageIdReceived;

    public event AsyncEventHandler<ApiRequestEventArgs>? OnMakingApiRequest
    {
        add => _inner.OnMakingApiRequest += value;
        remove => _inner.OnMakingApiRequest -= value;
    }

    public event AsyncEventHandler<ApiResponseEventArgs>? OnApiResponseReceived
    {
        add => _inner.OnApiResponseReceived += value;
        remove => _inner.OnApiResponseReceived -= value;
    }

    public TelegramMessageIdCaptureBotClientDecorator(ITelegramBotClient inner, Action<int> onMessageIdReceived)
    {
        _inner = inner;
        _onMessageIdReceived = onMessageIdReceived;
    }

    public bool LocalBotServer => _inner.LocalBotServer;

    public long? BotId => _inner.BotId;

    public TimeSpan Timeout
    {
        get => _inner.Timeout;
        set => _inner.Timeout = value;
    }

    public IExceptionParser ExceptionsParser
    {
        get => _inner.ExceptionsParser;
        set => _inner.ExceptionsParser = value;
    }

    public async Task<TResponse> MakeRequestAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
    {
        var response = await _inner.MakeRequestAsync(request, cancellationToken);
        if (TryGetMessageId(response) is { } messageId)
            _onMessageIdReceived(messageId);
        return response;
    }

    public Task<bool> TestApiAsync(CancellationToken cancellationToken)
        => _inner.TestApiAsync(cancellationToken);

    public Task DownloadFileAsync(string filePath, Stream destination, CancellationToken cancellationToken)
        => _inner.DownloadFileAsync(filePath, destination, cancellationToken);

    private static int? TryGetMessageId<TResponse>(TResponse? response)
        => response switch
        {
            Message message => message.MessageId,
            // MessageId messageId => messageId.Id,
            // Message[] messages => messages.FirstOrDefault()?.MessageId, // SendMediaGroupRequest?
            _ => null
        };
}
