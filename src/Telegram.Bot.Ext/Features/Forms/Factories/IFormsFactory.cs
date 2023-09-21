using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Features.Forms.Factories;

public interface IFormsFactory
{
    Task<IForm?> TryCreateAsync(IHandleContext ctx, Update update);
}