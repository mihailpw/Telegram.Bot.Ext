using Telegram.Bot.Ext.Features.Forms.Factories;
using Telegram.Bot.Ext.States;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.Forms;

public class FormExecutor
{
    private readonly IFormsFactory _formsFactory;
    private readonly Dictionary<IState, LinkedList<(IForm form, FormContext ctx)>> _activeForms = new();

    public FormExecutor(IFormsFactory formsFactory)
    {
        _formsFactory = formsFactory;
    }

    public async Task<bool> TryExecuteAsync(IHandleContext ctx, Update request,
        CancellationToken token)
    {
        if (!TryResolveActiveForm(ctx.State, out var formData))
        {
            var form = await _formsFactory.TryCreateAsync(ctx, request);
            if (form is not null)
            {
                var formCtx = new FormContext(ctx);
                formData = (form, formCtx);
                await form.Initialize(request, formCtx, token);
                AddForm(ctx.State, formData);
            }
        }

        if (formData.form is not null)
        {
            formData.ctx.CaptureRequest(request);
            var isCompleted = await formData.form.ProcessAsync(request, formData.ctx, token);
            if (isCompleted)
                RemoveForm(ctx.State, formData);
            return true;
        }

        return false;
    }

    private bool TryResolveActiveForm(IState state, out (IForm form, FormContext ctx) formItem)
    {
        if (_activeForms.TryGetValue(state, out var stack)
            && stack.Last is not null)
        {
            formItem = stack.Last.Value;
            return true;
        }

        formItem = default;
        return false;
    }

    private void AddForm(IState state, (IForm form, FormContext ctx) formItem)
    {
        if (!_activeForms.TryGetValue(state, out var stack))
        {
            stack = new LinkedList<(IForm form, FormContext ctx)>();
            _activeForms.Add(state, stack);
        }

        stack.AddLast(formItem);
    }

    private void RemoveForm(IState state, (IForm form, FormContext ctx) formItem)
    {
        if (!_activeForms.TryGetValue(state, out var stack))
            return;

        stack.Remove(formItem);
    }
}