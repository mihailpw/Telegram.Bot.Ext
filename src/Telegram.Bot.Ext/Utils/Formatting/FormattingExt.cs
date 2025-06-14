using Telegram.Bot.Types;

namespace Telegram.Bot.Ext.Utils.Formatting;

public static class FormattingExt
{
    public static IFormatting WithBold(this IFormatting target, string text)
        => target.WithBold(b => b.WithText(text));
    public static IFormatting WithItalic(this IFormatting target, string text)
        => target.WithItalic(b => b.WithText(text));
    public static IFormatting WithUnderline(this IFormatting target, string text)
        => target.WithUnderline(b => b.WithText(text));
    public static IFormatting WithStrikethrough(this IFormatting target, string text)
        => target.WithStrikethrough(b => b.WithText(text));
    public static IFormatting WithSpoiler(this IFormatting target, string text)
        => target.WithSpoiler(b => b.WithText(text));
    public static IFormatting WithInlineCode(this IFormatting target, string text)
        => target.WithInlineCode(b => b.WithText(text));
    public static IFormatting WithCodeBlock(this IFormatting target, string text, string? lang = null)
        => target.WithCodeBlock(lang, b => b.WithText(text));
    public static IFormatting WithQuote(this IFormatting target, string text)
        => target.WithQuote(b => b.WithText(text));
    public static IFormatting WithUserLinkOrName(this IFormatting target, Chat chat)
        => target.WithText(chat.Username is not null
            ? $"@{chat.Username}"
            : TelegramExt.FormatNameString(firstName: chat.FirstName, lastName: chat.LastName));
    public static IFormatting WithUserMention(this IFormatting target, Chat chat)
        => target.WithUserMention(
            TelegramExt.FormatNameString(firstName: chat.FirstName, lastName: chat.LastName), chat.Id);

    public static IFormatting Row(this IFormatting target, Action<IFormatting> build)
    {
        build(target);
        target.WithNewLine();
        return target;
    }

    public static IFormatting WithBuilder(this IFormatting target, Action<IFormatting> build)
    {
        build(target);
        return target;
    }

    public static IFormatting If(this IFormatting target, bool condition,
        Action<IFormatting> buildIf, Action<IFormatting>? buildElse = null)
    {
        if (condition)
            buildIf(target);
        else
            buildElse?.Invoke(target);
        return target;
    }

    public static IFormatting WithWrapping(this IFormatting target, string start, string end, Action<IFormatting> build)
    {
        target.WithText(start);
        build(target);
        target.WithText(end);
        return target;
    }
}