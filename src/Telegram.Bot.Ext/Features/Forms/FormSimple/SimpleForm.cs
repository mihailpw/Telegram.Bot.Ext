using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.Forms.FormSimple;

public abstract class SimpleFormBase : IForm
{
    private bool _isQuestionState = true;

    public virtual Task Initialize(Update request, IFormContext ctx, CancellationToken token) => Task.CompletedTask;

    public async Task<bool> ProcessAsync(Update request, IFormContext ctx, CancellationToken token)
    {
        var isCompleted = false;

        if (_isQuestionState)
        {
            await RenderQuestionAsync(request, ctx, token);
            _isQuestionState = false;
        }
        else
        {
            var isAnswered = await ProcessResponseAsync(request, ctx, token);
            if (isAnswered)
            {
                _isQuestionState = true;
                await RenderQuestionAsync(request, ctx, token);
                _isQuestionState = false;
                isCompleted = ValidateForm();
            }
        }

        if (isCompleted)
            await OnCompletedAsync(ctx, token);

        return isCompleted;
    }

    protected abstract bool ValidateForm();
    protected abstract Task RenderQuestionAsync(Update request, IFormContext ctx, CancellationToken token);
    protected abstract Task<bool> ProcessResponseAsync(Update request, IFormContext ctx, CancellationToken token);
    protected abstract Task OnCompletedAsync(IFormContext ctx, CancellationToken token);
}