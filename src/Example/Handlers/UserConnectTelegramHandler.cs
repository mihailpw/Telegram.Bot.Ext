using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Ext;
using Telegram.Bot.Ext.Handlers.Base;
using Telegram.Bot.Types;

namespace Example.Handlers;

public class UserConnectTelegramHandler : CommandTelegramHandlerBase
{
    private readonly ILogger<UserConnectTelegramHandler> _logger;

    public UserConnectTelegramHandler(ILogger<UserConnectTelegramHandler> logger)
        : base("start", continueIfNotSuitable: true)
    {
        _logger = logger;
    }

    protected override async Task HandleCommandAsync(Message message, IHandleContext ctx, CancellationToken token)
    {
        await ctx.Bot.DeleteMessageAsync(ctx.ChatId, message.MessageId, token);
        var name = string.Join(" ", new[] { message.Chat.FirstName, message.Chat.LastName }.Where(d => d is not null));
        _logger.LogInformation($"User connected: {name} ({message.Chat.Id})");
    }
}