using Telegram.Bot.Exceptions;

namespace Telegram.Bot.Ext.Features.Forms;

public static class FormContextExt
{
    public static async Task DeleteMessagesAsync(this IFormContext ctx, CancellationToken token, params int[] exceptions)
    {
        await Task.WhenAll(ctx.HistoryMessageIds
            .Except(exceptions)
            .Select(async id =>
            {
                try
                {
                    await ctx.Bot.DeleteMessageAsync(ctx.ChatId, id, token);
                }
                catch (ApiRequestException e) when (e.ErrorCode == 400)
                {
                    // ignore
                }
            }));
    }
}