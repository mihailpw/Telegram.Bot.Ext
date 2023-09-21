using Telegram.Bot.Ext.Utils;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Handlers.Base;

public abstract class CommandTelegramHandlerBase : TelegramHandlerBase
{
    private readonly string _command;
    private readonly bool _continueIfNotSuitable;

    protected CommandTelegramHandlerBase(string command, bool continueIfNotSuitable = false)
    {
        _continueIfNotSuitable = continueIfNotSuitable;
        _command = StringUtils.PrepareCommand(command);
    }

    protected sealed override async Task<bool> HandleAsync(Update request, IHandleContext ctx, CancellationToken token)
    {
        if (request is not { Message: { Text: { } text } message })
            return false;

        if (!StringUtils.IsCommand(text) || _command != StringUtils.PrepareCommand(text))
            return false;

        if (!await CheckIfSuitableAsync(message, ctx, token))
            return !_continueIfNotSuitable;

        await HandleCommandAsync(message, ctx, token);

        return true;
    }

    protected virtual Task<bool> CheckIfSuitableAsync(Message message, IHandleContext ctx, CancellationToken token)
        => Task.FromResult(true);
    protected abstract Task HandleCommandAsync(Message message, IHandleContext ctx, CancellationToken token);
}