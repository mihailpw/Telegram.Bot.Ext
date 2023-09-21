namespace Telegram.Bot.Ext.Core;

internal static class SemaphoreExt
{
    private class Disposable(Action disposeAction) : IDisposable
    {
        public void Dispose() => disposeAction();
    }
    
    public static async Task<IDisposable> LockAsync(this SemaphoreSlim target)
    {
        await target.WaitAsync();
        return new Disposable(() => target.Release());
    }
}
