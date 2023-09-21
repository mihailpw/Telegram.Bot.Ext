namespace Telegram.Bot.Ext.Core;

internal static class TaskExt
{
    public static async void RunAfter(TimeSpan delay, Func<Task> taskFunc, CancellationToken token)
    {
        await Task.Delay(delay, token);
        await taskFunc();
    }

    public static async Task Safe(this Task target)
    {
        try
        {
            await target;
        }
        catch
        {
            // ignored
        }
    }

    public static async Task<T?> Safe<T>(this Task<T> target, T? defaultValue = default)
    {
        try
        {
            return await target;
        }
        catch
        {
            return defaultValue;
        }
    }

    public static async void Forget(this Task target)
    {
        await target.Safe();
    }
}
