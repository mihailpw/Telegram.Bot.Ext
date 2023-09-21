using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.Forms.Steps;

public sealed class CancelableFormStepDecorator : FormStep
{
    private readonly FormStep _inner;
    private readonly string _cancelCommand;

    public CancelableFormStepDecorator(FormStep inner, string cancelCommand)
    {
        _inner = inner;
        _cancelCommand = cancelCommand;
    }

    public override async Task RenderQuestionAsync(Update request, IFormContext ctx, CancellationToken token)
    {
        await _inner.RenderQuestionAsync(request, ctx, token);;
    }

    public override async Task<FormStep?> ExecuteAsync(Update request, IFormContext ctx, CancellationToken token)
    {
        if (request is { Message.Text: { } text }
            && text == _cancelCommand)
            return null;

        return await _inner.ExecuteAsync(request, ctx, token);
    }
}