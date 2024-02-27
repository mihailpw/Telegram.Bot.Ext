using Telegram.Bot.Ext.Helpers;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.Forms.Base;

public abstract class VarForm : IForm
{
    protected delegate Task<bool> AsyncHandler(int editMessageId, string callbackQueryRaw, IFormContext ctx, CancellationToken token);

    private readonly Dictionary<Var, AsyncHandler> _handlers = new();
    private int? _formMessageId;
    private Var? _lastClickedVar;

    public async Task Initialize(Update request, IFormContext ctx, CancellationToken token)
    {
        var message = await RenderStartMessageAsync(request, ctx, token);
        _formMessageId = message.MessageId;
    }

    public async Task<bool> ProcessAsync(Update request, IFormContext ctx, CancellationToken token)
    {
        if (_formMessageId is null)
            throw new InvalidOperationException("Start message not rendered");

        bool finished;
        if (request is { CallbackQuery.Data: { } callbackQueryRaw }
            && _handlers.Keys.FirstOrDefault(v =>
                v.IsRelated(callbackQueryRaw) || v.IsRelated(ctx.State, callbackQueryRaw)) is { } relatedVar)
        {
            finished = await _handlers[relatedVar](_formMessageId.Value, callbackQueryRaw, ctx, token);
            _lastClickedVar = relatedVar;
        }
        else
        {
            finished = await ProcessCustomMessageAsync(_lastClickedVar, _formMessageId.Value, request, ctx, token);
        }

        if (finished)
            await OnFinishedAsync(token);

        return finished;
    }

    protected abstract Task<Message> RenderStartMessageAsync(Update request, IFormContext ctx, CancellationToken token);

    protected abstract Task<bool> ProcessCustomMessageAsync(Var? lastClickedVar, int editMessageId, Update request, IFormContext ctx,
        CancellationToken token);

    protected virtual Task OnFinishedAsync(CancellationToken token) => Task.CompletedTask;

    protected void RegisterAsyncHandler(Var var, AsyncHandler handler)
        => _handlers.Add(var, handler);
}