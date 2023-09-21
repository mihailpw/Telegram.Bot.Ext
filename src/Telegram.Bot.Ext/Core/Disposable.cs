namespace Telegram.Bot.Ext.Core;

internal class Disposable : IDisposable
{
    private readonly Action _disposeAction;

    public Disposable(Action disposeAction) => _disposeAction = disposeAction;

    public void Dispose() => _disposeAction();
}

internal class AsyncDisposable : IAsyncDisposable
{
    private readonly Func<Task> _disposeAction;

    public AsyncDisposable(Func<Task> disposeAction) => _disposeAction = disposeAction;

    public async ValueTask DisposeAsync() => await _disposeAction();
}
