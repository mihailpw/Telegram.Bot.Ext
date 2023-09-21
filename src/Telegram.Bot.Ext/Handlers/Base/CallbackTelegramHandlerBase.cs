using Telegram.Bot.Ext.Helpers;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Handlers.Base;

public abstract class CallbackTelegramHandlerBase : TelegramHandlerBase
{
    protected delegate Task HandleVar(CallbackQuery query, IHandleContext ctx, CancellationToken token);

    private readonly Dictionary<Var, HandleVar> _handlers = new();

    protected sealed override async Task<bool> HandleAsync(Update request, IHandleContext ctx, CancellationToken token)
    {
        if (request is not { CallbackQuery: { } callbackQuery })
            return false;

        if (callbackQuery is { Data: { } callbackData })
        {
            foreach (var handleVar in _handlers)
            {
                if (handleVar.Key.IsRelated(ctx.State, callbackData))
                {
                    await handleVar.Value(callbackQuery, ctx, token);
                    return true;
                }
            }
        }

        return false;
    }

    protected void RegisterVar(Var var, HandleVar handler)
    {
        _handlers[var] = handler;
    }
}