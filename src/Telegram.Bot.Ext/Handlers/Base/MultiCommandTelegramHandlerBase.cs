using Telegram.Bot.Ext.Utils;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Handlers.Base;

public abstract class MultiCommandTelegramHandlerBase : TelegramHandlerBase
{
    protected delegate Task<bool> CheckSuitable(Message message, IHandleContext ctx, CancellationToken token);
    protected delegate Task Handle(Message message, IHandleContext ctx, CancellationToken token);

    private readonly bool _continueIfNotSuitable;
    private readonly Dictionary<string, CheckSuitable> _gates = new();
    private readonly Dictionary<string, Handle> _handlers = new();

    protected MultiCommandTelegramHandlerBase(bool continueIfNotSuitable = false)
    {
        _continueIfNotSuitable = continueIfNotSuitable;
    }

    protected sealed override async Task<bool> HandleAsync(Update request, IHandleContext ctx, CancellationToken token)
    {
        if (request is not { Message: { Text: { } text } message })
            return false;

        if (!StringUtils.IsCommand(text))
            return false;

        var preparedCommand = StringUtils.PrepareCommand(text);
        if (!_handlers.TryGetValue(preparedCommand, out var handler))
            return false;

        if (_gates.TryGetValue(preparedCommand, out var gate))
            if (!await gate(message, ctx, token))
                return !_continueIfNotSuitable;

        await handler(message, ctx, token);

        return true;
    }

    protected void RegisterCommand(
        string command,
        Handle handler,
        CheckSuitable? gate = default)
    {
        var preparedCommand = StringUtils.PrepareCommand(command);
        if (gate is not null)
            _gates[preparedCommand] = gate;
        _handlers[preparedCommand] = handler;
    }
}