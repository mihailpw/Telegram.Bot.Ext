using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.Forms.Steps;

public abstract class FormStep
{
    public abstract Task RenderQuestionAsync(Update request, IFormContext ctx, CancellationToken token);
    public abstract Task<FormStep?> ExecuteAsync(Update request, IFormContext ctx, CancellationToken token);
    public virtual Task FinishAsync(Update request, IFormContext ctx, CancellationToken token) => Task.CompletedTask;
}