using Telegram.Bot.Ext.Features.Users.Models;
using Telegram.Bot.Ext.Handlers.Base;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.Users.Handlers;

public class SubscriptionTelegramHandler : MultiCommandTelegramHandlerBase
{
    private readonly IUserRepository<UserModel> _userRepository;

    public SubscriptionTelegramHandler(IUserRepository<UserModel> userRepository)
    {
        _userRepository = userRepository;

        RegisterCommand("start", HandleStartAsync);
        RegisterCommand("stop", HandleStopAsync);
    }

    public string? SubscribedMessage { private get; set; } = "Subscribed";
    public string? AlreadySubscribedMessage { private get; set; } = "Already subscribed";
    public string? WasNotSubscribedMessage { private get; set; } = "Was not subscribed";
    public string? UnsubscribedMessage { private get; set; } = "Removed";

    private async Task HandleStartAsync(Message message, IHandleContext ctx, CancellationToken token)
    {
        if (message is not { From.Id: var userId, Chat: { } chat })
            return;

        var user = UserModel.Create(userId, Role.User, chat.Username, $"{chat.FirstName} {chat.LastName}");
        var added = await _userRepository.AddUserAsync(user);

        var response = added ? SubscribedMessage : AlreadySubscribedMessage;
        if (response is not null)
            await ctx.Bot.SendTextMessageAsync(userId, response, cancellationToken: token);
    }

    private async Task HandleStopAsync(Message message, IHandleContext ctx, CancellationToken token)
    {
        if (message is not { From.Id: var userId })
            return;

        var removed = await _userRepository.DeleteUserAsync(userId);
        var response = removed ? UnsubscribedMessage : WasNotSubscribedMessage;
        if (response is not null)
            await ctx.Bot.SendTextMessageAsync(userId, response, cancellationToken: token);
    }
}
