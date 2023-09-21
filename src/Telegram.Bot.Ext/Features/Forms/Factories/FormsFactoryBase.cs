using Telegram.Bot.Ext.Utils;
using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.Forms.Factories;

public abstract class FormsFactoryBase : IFormsFactory
{
    protected delegate IForm FormFactory(IHandleContext ctx, Message request);
    protected delegate Task<bool> Gate(IHandleContext ctx, Message request);

    private readonly Dictionary<string, (FormFactory formFactory, Gate? gate)> _factories = new();

    public async Task<IForm?> TryCreateAsync(IHandleContext ctx, Update update)
    {
        if (update.Message is not { Text: { } text } message)
            return null;

        if (!StringUtils.IsCommand(text))
            return null;

        var command = StringUtils.PrepareCommand(text);

        if (!_factories.TryGetValue(command, out var factoryWithGate))
            return null;

        if (factoryWithGate.gate is not null)
            if (!await factoryWithGate.gate(ctx, message))
                return null;

        return factoryWithGate.formFactory(ctx, message);
    }

    protected void RegisterFactory(string command, FormFactory factory, Gate? gate = default)
    {
        var preparedCommand = StringUtils.PrepareCommand(command);
        _factories[preparedCommand] = (factory, gate);
    }
}