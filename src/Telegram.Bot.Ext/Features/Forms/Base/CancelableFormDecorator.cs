using Telegram.Bot.Ext.Utils;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.Forms.Base;

public class CancelableFormDecorator : IForm
{
    private readonly IForm _inner;
    private readonly bool _deleteMessagesOnCancel;
    private readonly string _cancelCommand;

    public CancelableFormDecorator(IForm inner, bool deleteMessagesOnCancel = true, string cancelCommand = "cancel")
    {
        _inner = inner;
        _deleteMessagesOnCancel = deleteMessagesOnCancel;
        _cancelCommand = StringUtils.PrepareCommand(cancelCommand);
    }

    public async Task Initialize(Update request, IFormContext ctx, CancellationToken token)
    {
        await _inner.Initialize(request, ctx, token);
    }

    public async Task<bool> ProcessAsync(Update request, IFormContext ctx, CancellationToken token)
    {
        if (request is { Message.Text: { } text }
            && StringUtils.IsCommand(text)
            && StringUtils.PrepareCommand(text) == _cancelCommand)
        {
            if (_deleteMessagesOnCancel)
                await ctx.DeleteMessagesAsync(token);
            return true;
        }

        return await _inner.ProcessAsync(request, ctx, token);
    }
}