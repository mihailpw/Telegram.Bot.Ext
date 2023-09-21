using System.Text;

namespace Telegram.Bot.Ext.Utils;

// https://core.telegram.org/bots/api#markdownv2-style
public class TelegramMarkdownV2
{
    private static readonly List<char> EscapeSymbols
        = new() { '_', '*', '[', ']', '(', ')', '~', '`', '>', '#', '+', '-', '=', '|', '{', '}', '.', '!' };

    private readonly bool _trimNewLines;
    private readonly StringBuilder _sb = new();

    public TelegramMarkdownV2(bool trimNewLines = true)
    {
        _trimNewLines = trimNewLines;
    }

    public static implicit operator string(TelegramMarkdownV2 builder) => builder.Build();

    public TelegramMarkdownV2 With(TelegramMarkdownV2 inner)
        => Do(sb => sb.Append(inner._sb));

    public TelegramMarkdownV2 WithText(string text)
        => Do(sb => sb.Append(Escape(text)));
    public TelegramMarkdownV2 WithNewLine(bool forceAdd = false)
        => Do(sb => sb.AppendLine(), forceAdd || !EndsWith(Environment.NewLine));
    public TelegramMarkdownV2 WithBold(Action<TelegramMarkdownV2> build)
        => Wrap("*", build);
    public TelegramMarkdownV2 WithItalic(Action<TelegramMarkdownV2> build)
        => Wrap("_", build);
    public TelegramMarkdownV2 WithUnderline(Action<TelegramMarkdownV2> build)
        => Wrap("__", build);
    public TelegramMarkdownV2 WithStrikethrough(Action<TelegramMarkdownV2> build)
        => Wrap("~", build);
    public TelegramMarkdownV2 WithSpoiler(Action<TelegramMarkdownV2> build)
        => Wrap("||", build);
    public TelegramMarkdownV2 WithLink(string title, string url)
        => Do(sb => sb.Append('[').Append(Escape(title)).Append("](").Append(Escape(url)).Append(')'));
    public TelegramMarkdownV2 WithUserMention(string title, long userId)
        => Do(sb => sb.Append('[').Append(Escape(title)).Append("](tg://user?id=").Append(userId).Append(')'));
    public TelegramMarkdownV2 WithInlineCode(Action<TelegramMarkdownV2> build)
        => Wrap("`", build);
    public TelegramMarkdownV2 WithCodeBlock(string? lang, Action<TelegramMarkdownV2> build)
        => Wrap($"```{Escape(lang)}", "```", build, true);

    public override string ToString() => Build();

    public string Build()
    {
        if (_trimNewLines)
            if (EndsWith(Environment.NewLine))
                _sb.Length -= Environment.NewLine.Length;

        return _sb.ToString();
    }

    private TelegramMarkdownV2 Do(Action<StringBuilder> action, bool condition = true)
    {
        if (condition)
            action(_sb);
        return this;
    }

    private TelegramMarkdownV2 Wrap(string bracket, Action<TelegramMarkdownV2> action, bool newLineBeforeAfter = false)
        => Wrap(bracket, bracket, action, newLineBeforeAfter);

    private TelegramMarkdownV2 Wrap(string bracketStart, string bracketEnd, Action<TelegramMarkdownV2> action,
        bool newLineBeforeAfter = false)
    {
        _sb.Append(bracketStart);
        if (newLineBeforeAfter)
            _sb.AppendLine();
        action(this);
        if (newLineBeforeAfter && !EndsWith(Environment.NewLine))
            _sb.AppendLine();
        _sb.Append(bracketEnd);
        return this;
    }

    private bool EndsWith(string val)
    {
        if (_sb.Length < val.Length)
            return false;
        for (var i = 1; i <= val.Length; i++)
            if (_sb[^i] != val[^i])
                return false;

        return true;
    }

    private static StringBuilder Escape(string? input)
    {
        var sb = new StringBuilder(input ?? "");
        for (var i = 0; i < sb.Length; i++)
        {
            if (EscapeSymbols.Contains(sb[i]))
            {
                sb.Insert(i, '\\');
                i++;
            }
        }

        return sb;
    }
}

public static class TelegramMarkdownV2Ext
{
    public static TelegramMarkdownV2 WithBold(this TelegramMarkdownV2 target, string text)
        => target.WithBold(b => b.WithText(text));
    public static TelegramMarkdownV2 WithItalic(this TelegramMarkdownV2 target, string text)
        => target.WithItalic(b => b.WithText(text));
    public static TelegramMarkdownV2 WithUnderline(this TelegramMarkdownV2 target, string text)
        => target.WithUnderline(b => b.WithText(text));
    public static TelegramMarkdownV2 WithStrikethrough(this TelegramMarkdownV2 target, string text)
        => target.WithStrikethrough(b => b.WithText(text));
    public static TelegramMarkdownV2 WithSpoiler(this TelegramMarkdownV2 target, string text)
        => target.WithSpoiler(b => b.WithText(text));
    public static TelegramMarkdownV2 WithInlineCode(this TelegramMarkdownV2 target, string text)
        => target.WithInlineCode(b => b.WithText(text));
    public static TelegramMarkdownV2 WithCodeBlock(this TelegramMarkdownV2 target, string text, string? lang = default)
        => target.WithCodeBlock(lang, b => b.WithText(text));

    public static TelegramMarkdownV2 Row(this TelegramMarkdownV2 target, Action<TelegramMarkdownV2> build)
    {
        build(target);
        target.WithNewLine();
        return target;
    }

    public static TelegramMarkdownV2 If(this TelegramMarkdownV2 target, bool condition, Action<TelegramMarkdownV2> build)
    {
        if (condition)
            build(target);
        return target;
    }

    public static TelegramMarkdownV2 WithWrapping(this TelegramMarkdownV2 target, string start, string end, Action<TelegramMarkdownV2> build)
    {
        target.WithText(start);
        build(target);
        target.WithText(end);
        return target;
    }
}
