using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.Forms.Base;

public abstract class StateForm<T> : IForm
    where T : struct, Enum
{
    protected delegate T Handler(Update request, IFormContext ctx, CancellationToken token);
    protected delegate Task<T> AsyncHandler(Update request, IFormContext ctx, CancellationToken token);

    private readonly T _finishState;
    private readonly Dictionary<T, AsyncHandler> _handlers = new();

    private T _state;

    protected StateForm(T finishState)
    {
        _finishState = finishState;
    }

    public virtual Task Initialize(Update request, IFormContext ctx, CancellationToken token) => Task.CompletedTask;

    public async Task<bool> ProcessAsync(Update request, IFormContext ctx, CancellationToken token)
    {
        if (_state.Equals(_finishState))
            throw new InvalidOperationException("Form already done");

        var nextState = await _handlers[_state](request, ctx, token);
        _state = nextState;

        var finished = _state.Equals(_finishState);
        if (finished)
            await OnFinishedAsync(token);
        return finished;
    }

    protected virtual Task OnFinishedAsync(CancellationToken token) => Task.CompletedTask;

    protected void RegisterHandler(T state, Handler handler)
        => _handlers.Add(state, (r, c, t) => Task.FromResult(handler(r, c, t)));
    protected void RegisterAsyncHandler(T state, AsyncHandler handler)
        => _handlers.Add(state, handler);
}