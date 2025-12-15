namespace Telegram.Bot.Ext.Features.CallbackQueries;

public class CallbackDataId
{
    private readonly string _id;
    private readonly Func<string, string?, string> _callbackDataBuilder;
    private readonly Func<string, bool> _detach;

    internal CallbackDataId(string id, Func<string, string?, string> callbackDataBuilder, Func<string, bool> detach)
    {
        _id = id;
        _callbackDataBuilder = callbackDataBuilder;
        _detach = detach;
    }

    public static implicit operator string(CallbackDataId callbackDataId) =>
        callbackDataId.Build();

    public string Build(string? data = null) => _callbackDataBuilder(_id, data);

    public bool Detach() => _detach(_id);
}