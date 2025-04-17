namespace Telegram.Bot.Ext.Features.CallbackQueries;

public class CallbackDataId
{
    private readonly Guid _guid;
    private readonly Func<Guid, string?, string> _callbackDataBuilder;
    private readonly Func<Guid, bool> _detach;

    internal CallbackDataId(Guid guid, Func<Guid, string?, string> callbackDataBuilder, Func<Guid, bool> detach)
    {
        _guid = guid;
        _callbackDataBuilder = callbackDataBuilder;
        _detach = detach;
    }

    public static implicit operator string(CallbackDataId callbackDataId) =>
        callbackDataId.Build();

    public string Build(string? data = null) => _callbackDataBuilder(_guid, data);

    public bool Detach() => _detach(_guid);
}