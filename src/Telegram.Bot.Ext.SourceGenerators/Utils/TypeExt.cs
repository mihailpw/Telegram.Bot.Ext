using System;
using System.Linq;

namespace Telegram.Bot.Ext.SourceGenerators.Utils;

public static class TypeExt
{
    public static string GetFullName(this Type type)
    {
        if (!type.IsGenericType)
            return type.FullName!;

        if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
            return $"{type.GenericTypeArguments[0]}?";

        return $"{type.FullName!.Split('`').First()}<{string.Join(",", type.GenericTypeArguments.Select(GetFullName))}>";
    }
}
