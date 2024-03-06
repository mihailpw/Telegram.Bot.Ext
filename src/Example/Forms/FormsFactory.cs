using Telegram.Bot.Ext;
using Telegram.Bot.Ext.Features.Forms;
using Telegram.Bot.Ext.Features.Forms.Factories;
using Telegram.Bot.Ext.Features.Users;
using Telegram.Bot.Ext.Features.Users.Models;
using Telegram.Bot.Types;

namespace Example.Forms;

public class FormsFactory : FormsFactoryBase
{
    private readonly IUsersProvider _usersProvider;

    public FormsFactory(IUsersProvider usersProvider)
    {
        _usersProvider = usersProvider;
        RegisterFactory(Commands.Hello, Hello);
        RegisterFactory(Commands.HelloAdmin, HelloAdmin, AdminGate);
    }

    private IForm Hello(IHandleContext ctx, Message request)
    {
        throw new NotImplementedException();
    }

    private IForm HelloAdmin(IHandleContext ctx, Message request)
    {
        throw new NotImplementedException();
    }

    private async Task<bool> AdminGate(IHandleContext ctx, Message request)
    {
        return await _usersProvider.CheckIfAsync(ctx.UserId, Role.Administrator);
    }
}