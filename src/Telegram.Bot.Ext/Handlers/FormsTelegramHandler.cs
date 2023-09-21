using Telegram.Bot.Ext.Features.Forms;
using Telegram.Bot.Ext.Features.Forms.Factories;
using Telegram.Bot.Ext.Handlers.Base;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Handlers;

public class FormsTelegramHandler : TelegramHandlerBase
{
    private readonly FormExecutor _formExecutor;

    public FormsTelegramHandler(IFormsFactory formsFactory)
    {
        _formExecutor = new FormExecutor(formsFactory);
    }

    protected override async Task<bool> HandleAsync(Update request, IHandleContext ctx, CancellationToken token)
    {
        return await _formExecutor.TryExecuteAsync(ctx, request, token);
    }
}