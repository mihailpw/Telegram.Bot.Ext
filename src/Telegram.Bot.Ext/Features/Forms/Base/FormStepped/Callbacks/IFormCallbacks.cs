using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.Forms.Base.FormStepped.Callbacks;

public interface IFormCallbacks
{
    Task OnStartingAsync(Update request, IFormContext ctx, CancellationToken token);
    Task OnStepExecutedAsync(Update request, IFormContext ctx, CancellationToken token);
    Task OnCompletedAsync(Update request, IFormContext ctx, CancellationToken token);
}