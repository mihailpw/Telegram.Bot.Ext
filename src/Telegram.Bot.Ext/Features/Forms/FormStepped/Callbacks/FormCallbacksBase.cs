using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.Forms.FormStepped.Callbacks;

public class FormCallbacksBase : IFormCallbacks
{
    public virtual Task OnStartingAsync(Update request, IFormContext ctx, CancellationToken token)
        => Task.CompletedTask;

    public virtual Task OnStepExecutedAsync(Update request, IFormContext ctx, CancellationToken token)
        => Task.CompletedTask;

    public virtual Task OnCompletedAsync(Update request, IFormContext ctx, CancellationToken token)
        => Task.CompletedTask;
}