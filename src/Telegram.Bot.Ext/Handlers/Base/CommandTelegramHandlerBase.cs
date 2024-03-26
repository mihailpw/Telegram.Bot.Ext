using Telegram.Bot.Ext.Utils;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Handlers.Base;

public abstract class CommandTelegramHandlerBase : TelegramHandlerBase
{
    private readonly string _command;

    protected CommandTelegramHandlerBase(string command)
    {
        _command = StringUtils.PrepareCommand(command);
    }

    protected bool ContinueIfNotSuitable { get; set; }
    protected bool AutoRemoveCommandMessage { get; set; }

    protected sealed override async Task<bool> HandleAsync(Update request, IHandleContext ctx, CancellationToken token)
    {
        if (request is not { Message: { Text: { } text } message })
            return false;

        if (!StringUtils.IsCommand(text) || _command != StringUtils.PrepareCommand(text))
            return false;

        if (AutoRemoveCommandMessage)
            await ctx.Bot.DeleteMessageAsync(ctx.ChatId, message.MessageId, token);

        if (!await CheckIfSuitableAsync(message, ctx, token))
            return !ContinueIfNotSuitable;

        await HandleCommandAsync(message, ctx, token);

        return true;
    }

    protected virtual Task<bool> CheckIfSuitableAsync(Message message, IHandleContext ctx, CancellationToken token)
        => Task.FromResult(true);
    protected abstract Task HandleCommandAsync(Message message, IHandleContext ctx, CancellationToken token);
}