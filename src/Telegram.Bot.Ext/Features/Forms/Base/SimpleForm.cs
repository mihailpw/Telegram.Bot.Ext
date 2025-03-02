using Telegram.Bot.Ext.Features.Messages;
using Telegram.Bot.Types;
using Message = Telegram.Bot.Types.Message;

namespace Telegram.Bot.Ext.Features.Forms.Base;

public abstract class SimpleFormBase : IForm
{
    private readonly List<int> _messagesToRemove = new();
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
                isCompleted = ValidateForm();
                if (!isCompleted)
                    await RenderQuestionAsync(request, ctx, token);
                _isQuestionState = false;
            }
        }

        if (!isCompleted)
            return false;

        await OnCompletedAsync(ctx, token);
        if (_messagesToRemove.Count > 0)
        {
            var messageRemover = ctx.Bot.GetMessageRemover();
            foreach (var id in _messagesToRemove)
                messageRemover.RemoveImmediately(ctx.ChatId, id);
        }

        return true;
    }

    protected abstract bool ValidateForm();
    protected abstract Task RenderQuestionAsync(Update request, IFormContext ctx, CancellationToken token);
    protected abstract Task<bool> ProcessResponseAsync(Update request, IFormContext ctx, CancellationToken token);
    protected abstract Task OnCompletedAsync(IFormContext ctx, CancellationToken token);

    protected void CaptureMessageToRemove(Message message)
    {
        _messagesToRemove.Add(message.MessageId);
    }
}