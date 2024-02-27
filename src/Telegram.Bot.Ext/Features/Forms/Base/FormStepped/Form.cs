using Telegram.Bot.Ext.Features.Forms.Base.FormStepped.Callbacks;
using Telegram.Bot.Ext.Features.Forms.Steps;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.Forms.Base.FormStepped;

public sealed class Form : IForm
{
    private readonly FormStep _rootState;
    private readonly IFormBag _bag;
    private readonly IFormCallbacks? _formCallbacks;
    private FormStep? _currentStep;

    public Form(FormStep rootState, IFormBag bag, IFormCallbacks? formCallbacks = default)
    {
        _rootState = rootState;
        _bag = bag;
        _formCallbacks = formCallbacks;
    }

    public Task Initialize(Update request, IFormContext ctx, CancellationToken token)
    {
        ctx.Bag = _bag;
        return Task.CompletedTask;
    }

    public async Task<bool> ProcessAsync(Update request, IFormContext ctx, CancellationToken token)
    {
        if (_currentStep is null)
        {
            if (_formCallbacks is not null)
                await _formCallbacks.OnStartingAsync(request, ctx, token);

            await _rootState.RenderQuestionAsync(request, ctx, token);

            _currentStep = _rootState;
            return false;
        }

        var nextStep = await _currentStep.ExecuteAsync(request, ctx, token);
        if (_currentStep != nextStep)
        {
            if (_formCallbacks is not null)
                await _formCallbacks.OnStepExecutedAsync(request, ctx, token);
            await _currentStep.FinishAsync(request, ctx, token);

            if (nextStep is not null)
                await nextStep.RenderQuestionAsync(request, ctx, token);

            _currentStep = nextStep;
        }

        if (_currentStep is not null)
            return false;

        if (_formCallbacks is not null)
            await _formCallbacks.OnCompletedAsync(request, ctx, token);

        return true;
    }
}