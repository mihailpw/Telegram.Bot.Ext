namespace Telegram.Bot.Ext.Core;

internal static class SemaphoreExt
{
    private class Disposable : IDisposable
    {
        private readonly Action _disposeAction;

        public Disposable(Action disposeAction) => _disposeAction = disposeAction;

        public void Dispose() => _disposeAction();
    }
    
    public static async Task<IDisposable> LockAsync(this SemaphoreSlim target)
    {
        await target.WaitAsync();
        return new Disposable(() => target.Release());
    }
}
