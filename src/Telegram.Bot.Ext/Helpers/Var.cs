using Telegram.Bot.Ext.States;

namespace Telegram.Bot.Ext.Helpers;

public class Var
{
    protected readonly string Id = GenerateId();

    public static implicit operator string(Var var)
        => var.Id;

    public virtual bool IsRelated(string callbackDataRaw)
        => Id == callbackDataRaw;

    public virtual bool IsRelated(IState state, string callbackDataRaw)
        => IsRelated(callbackDataRaw) && state.Has<object>(Id);

    protected static string GenerateId() => Guid.NewGuid().ToString("N");
}

public sealed class Var<T> : Var
{
    private const string DefaultKey = "default";
    private readonly Dictionary<string, T> _staticValues = new();

    public string AddStaticValue(T value)
    {
        var linkId = GenerateId();
        _staticValues[linkId] = value;
        return linkId;
    }

    public void SetDefaultValue(IState state, T value)
        => state.Group(Id).Set(DefaultKey, value);

    public string AddValue(IState state, T value)
    {
        var linkId = GenerateId();
        state.Group(Id).Set(linkId, value);
        return linkId;
    }

    public T? GetStaticValue(string callbackDataRaw)
        => _staticValues.TryGetValue(callbackDataRaw, out var value) ? value : default;

    public T? GetDefaultValue(IState state)
        => state.Group(Id).Get<T>(DefaultKey);

    public T? GetValue(IState state, string callbackDataRaw)
        => state.Group(Id).Get<T>(callbackDataRaw);

    public override bool IsRelated(string callbackDataRaw)
        => base.IsRelated(callbackDataRaw) || _staticValues.ContainsKey(callbackDataRaw);

    public override bool IsRelated(IState state, string callbackDataRaw)
        => base.IsRelated(state, callbackDataRaw) || state.Group(Id).Has<T>(callbackDataRaw);
}