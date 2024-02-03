using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Ext.Features.Users;
using Telegram.Bot.Ext.Features.Users.Models;
using Telegram.Bot.Requests;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Helpers;

public sealed class TelegramSendingFilterBotClientDecorator : ITelegramBotClient
{
    private readonly ITelegramBotClient _inner;
    private readonly IUsersProvider _usersProvider;
    private readonly Role _allowedRole;

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

    public TelegramSendingFilterBotClientDecorator(ITelegramBotClient inner, IUsersProvider usersProvider, Role allowedRole)
    {
        _inner = inner;
        _usersProvider = usersProvider;
        _allowedRole = allowedRole;
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
        if (TryGetChatId(request) is {} chatId
            && !await _usersProvider.CheckIfAsync(chatId, _allowedRole))
            return default!;
        return await _inner.MakeRequestAsync(request, cancellationToken);
    }

    public Task<bool> TestApiAsync(CancellationToken cancellationToken)
        => _inner.TestApiAsync(cancellationToken);

    public Task DownloadFileAsync(string filePath, Stream destination, CancellationToken cancellationToken)
        => _inner.DownloadFileAsync(filePath, destination, cancellationToken);

    private long? TryGetChatId<T>(IRequest<T> request)
        => request switch
        {
            RequestBase<Message> requestBase => request is IChatTargetable { ChatId.Identifier: var chatId}
                ? chatId : null,
            RequestBase<MessageId> requestBase => request is IChatTargetable { ChatId.Identifier: var chatId}
                ? chatId : null,
            _ => null,
        };
}
