using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.Forms.FormStepped.Callbacks;

public class MessagesCleanerFormCallbacks : FormCallbacksBase
{
    private readonly List<int> _exceptions = new();

    public override async Task OnCompletedAsync(Update request, IFormContext ctx, CancellationToken token)
        => await ctx.DeleteMessagesAsync(token, _exceptions.ToArray());

    protected void ExceptMessageId(int messageId)
        => _exceptions.Add(messageId);
}