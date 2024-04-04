using System.ComponentModel;
using System.Globalization;

namespace Telegram.Bot.Ext.Features.Users.Identifiers;

public class IdentifierTypeConverter : StringConverter
{
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        return Identifier.Parse((string)value);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == typeof(string))
            return value?.ToString();
        throw new NotSupportedException();
    }
}