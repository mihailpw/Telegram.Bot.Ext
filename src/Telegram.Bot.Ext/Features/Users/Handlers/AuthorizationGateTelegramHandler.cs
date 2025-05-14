using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Ext.Features.Users.Handlers;

public class AuthorizationGateTelegramHandler : ITelegramHandler
{
    private readonly ILogger<AuthorizationGateTelegramHandler> _logger;
    private readonly string? _message;

    public AuthorizationGateTelegramHandler(ILogger<AuthorizationGateTelegramHandler> logger, string? message = null)
    {
        _logger = logger;
        _message = message;
    }

    public async Task HandleAsync(HandleNext next, Update request, IHandleContext ctx, CancellationToken token)
    {
        if (await ctx.Bot.GetUsersProvider().GetGroupAsync(ctx.UserId) is not null)
        {
            await next(ctx);
        }
        else
        {
            _logger.LogInformation("User {UserId} is not authorized", ctx.UserId);
            if (!string.IsNullOrWhiteSpace(_message))
                await ctx.Bot.SendTextMessageAsync(ctx.ChatId, _message, parseMode: ParseMode.MarkdownV2,
                    cancellationToken: token);
        }
    }
}