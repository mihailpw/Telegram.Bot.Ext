using Telegram.Bot.Ext.Features.Forms.Base.FormStepped;
using Telegram.Bot.Ext.Features.Forms.Infra;
using Telegram.Bot.Ext.States;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.Forms;

public interface IForm
{
    Task Initialize(Update request, IFormContext ctx, CancellationToken token);
    Task<bool> ProcessAsync(Update request, IFormContext ctx, CancellationToken token);
}

public interface IFormContext : IHandleContext
{
    IFormBag Bag { get; set; }
    IReadOnlyCollection<int> HistoryMessageIds { get; }
}

internal sealed class FormContext : IFormContext
{
    private readonly IHandleContext _handleContext;
    private readonly HashSet<int> _historyMessageIds = new();
    private IFormBag? _bag;

    public FormContext(IHandleContext handleContext)
    {
        _handleContext = handleContext;
        Bot = new TelegramMessageIdCaptureBotClientDecorator(handleContext.Bot, id => _historyMessageIds.Add(id));
    }

    public ChatId ChatId => _handleContext.ChatId;
    public long UserId => _handleContext.UserId;
    public IState State => _handleContext.State;
    public ITelegramBotClient Bot { get; }

    public IFormBag Bag
    {
        get => _bag ?? throw new InvalidOperationException("Bad was not set");
        set => _bag = value;
    }

    public IReadOnlyCollection<int> HistoryMessageIds => _historyMessageIds;

    public void CaptureRequest(Update request)
    {
        if (request is { Message.MessageId: var messageId })
            _historyMessageIds.Add(messageId);
        // if (request.GetMessageId() is { } messageId)
        //     _historyMessageIds.Add(messageId);
    }
}