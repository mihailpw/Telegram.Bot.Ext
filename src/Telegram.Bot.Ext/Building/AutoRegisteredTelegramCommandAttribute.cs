namespace Telegram.Bot.Ext.Building;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class AutoRegisteredTelegramCommandAttribute : Attribute
{
    public AutoRegisteredTelegramCommandAttribute(params object[] parameters)
    {
        Parameters = parameters;
    }

    public bool Disabled { get; set; }
    public object[] Parameters { get; }
}