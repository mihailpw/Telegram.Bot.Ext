using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Ext.Utils;

// https://core.telegram.org/bots/api#html-style
public class TelegramHtml
{
    private static readonly Dictionary<char, string> EscapeSymbols = new()
    {
        ['<'] = "&lt;",
        ['>'] = "&gt;",
        ['&'] = "&amp;",
    };

    private readonly bool _trimNewLines;
    private readonly StringBuilder _sb = new();

    public TelegramHtml(bool trimNewLines = true)
    {
        _trimNewLines = trimNewLines;
    }

    public TelegramHtml With(TelegramHtml inner)
        => Do(sb => sb.Append(inner._sb));
    public TelegramHtml WithText(string? text, bool escape = true)
        => Do(sb => sb.Append(Escape(text, escape)));
    public TelegramHtml WithNewLine(bool forceAdd = false)
        => Do(sb => sb.AppendLine(), forceAdd || !EndsWith(Environment.NewLine));
    public TelegramHtml WithBold(Action<TelegramHtml> build)
        => Wrap("b", build);
    public TelegramHtml WithItalic(Action<TelegramHtml> build)
        => Wrap("i", build);
    public TelegramHtml WithUnderline(Action<TelegramHtml> build)
        => Wrap("u", build);
    public TelegramHtml WithStrikethrough(Action<TelegramHtml> build)
        => Wrap("s", build);
    public TelegramHtml WithSpoiler(Action<TelegramHtml> build)
        => Wrap("tg-spoiler", build);
    public TelegramHtml WithLink(string title, string url, bool escapeTitle = true)
        => Do(sb => sb.Append($"<a href=\"{Escape(url)}\">{Escape(title, escapeTitle)}</a>"));
    public TelegramHtml WithUserMention(string title, long userId, bool escapeTitle = true)
        => Do(sb => sb.Append($"<a href=\"tg://user?id={userId}\">{Escape(title, escapeTitle)}</a>"));
    public TelegramHtml WithInlineCode(Action<TelegramHtml> build)
        => Wrap("code", build);
    public TelegramHtml WithCodeBlock(string? lang, Action<TelegramHtml> build)
        => Wrap("pre", b =>
        {
            if (lang is null) build(b);
            else b.Wrap($"code class=\"{Escape(lang)}\"", "code", build);
        });
    public TelegramHtml WithQuote(Action<TelegramHtml> build, bool expandable = true)
        => Wrap($"blockquote {(expandable ? "expandable" : "")}", "blockquote", build);

    public TelegramHtml WithFormatted(string? text, MessageEntity[]? entities = null)
    {
        if (text is null)
            return this;
        if (entities is null)
            return WithText(text);

        var sb = new StringBuilder(text);

        var groups = entities.GroupBy(e => e.Offset).OrderByDescending(g => g.Key);
        foreach (var group in groups)
        {
            var offset = group.Key;
            foreach (var entity in group.OrderByDescending(e => e.Length))
            {
                var start = offset;
                var end = offset + entity.Length;

                string startTag = "", endTag = "";

                switch (entity.Type)
                {
                    case MessageEntityType.Bold:
                        (startTag, endTag) = ("<b>", "</b>");
                        break;
                    case MessageEntityType.Italic:
                        (startTag, endTag) = ("<i>", "</i>");
                        break;
                    case MessageEntityType.Underline:
                        (startTag, endTag) = ("<u>", "</u>");
                        break;
                    case MessageEntityType.Strikethrough:
                        (startTag, endTag) = ("<s>",  "</s>");
                        break;
                    case MessageEntityType.Spoiler:
                        (startTag, endTag) = ("<tg-spoiler>", "</tg-spoiler>");
                        break;
                    case MessageEntityType.Code:
                        (startTag, endTag) = ("<code>", "</code>");
                        break;
                    case MessageEntityType.Pre:
                        (startTag, endTag) = ("<pre>", "</pre>");
                        break;
                    case MessageEntityType.TextLink:
                        (startTag, endTag) = ($"<a href=\"{Escape(entity.Url)}\">", "</a>");
                        break;
                    case MessageEntityType.TextMention:
                        if (entity.User != null)
                            (startTag, endTag) = ($"<a href=\"tg://user?id={entity.User.Id}\">", "</a>");
                        break;
                }

                sb.Insert(end, endTag);
                sb.Insert(start, startTag);
                offset += startTag.Length;
            }
        }

        _sb.Append(sb);
        return this;
    }

    public string Build()
    {
        if (_trimNewLines)
            while (EndsWith(Environment.NewLine))
                _sb.Length -= Environment.NewLine.Length;

        return _sb.ToString();
    }

    private TelegramHtml Do(Action<StringBuilder> action, bool condition = true)
    {
        if (condition)
            action(_sb);
        return this;
    }

    private TelegramHtml Wrap(string tag, Action<TelegramHtml> action, bool newLineBeforeAfter = false)
        => Wrap(tag, tag, action, newLineBeforeAfter);

    private TelegramHtml Wrap(string tagStart, string tagEnd, Action<TelegramHtml> action,
        bool newLineBeforeAfter = false)
    {
        _sb.Append($"<{tagStart}>");
        if (newLineBeforeAfter)
            _sb.AppendLine();
        action(this);
        if (newLineBeforeAfter && !EndsWith(Environment.NewLine))
            _sb.AppendLine();
        _sb.Append($"</{tagEnd}>");
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

    private static StringBuilder Escape(string? input, bool escape = true)
        => escape ? new StringBuilder().AppendEscaped(input, EscapeSymbols) : new StringBuilder(input ?? "");
}

public static class TelegramHtmlExt
{
    public static TelegramHtml WithBold(this TelegramHtml target, string text)
        => target.WithBold(b => b.WithText(text));
    public static TelegramHtml WithItalic(this TelegramHtml target, string text)
        => target.WithItalic(b => b.WithText(text));
    public static TelegramHtml WithUnderline(this TelegramHtml target, string text)
        => target.WithUnderline(b => b.WithText(text));
    public static TelegramHtml WithStrikethrough(this TelegramHtml target, string text)
        => target.WithStrikethrough(b => b.WithText(text));
    public static TelegramHtml WithSpoiler(this TelegramHtml target, string text)
        => target.WithSpoiler(b => b.WithText(text));
    public static TelegramHtml WithInlineCode(this TelegramHtml target, string text)
        => target.WithInlineCode(b => b.WithText(text));
    public static TelegramHtml WithCodeBlock(this TelegramHtml target, string text, string? lang = default)
        => target.WithCodeBlock(lang, b => b.WithText(text));
    public static TelegramHtml WithQuote(this TelegramHtml target, string text)
        => target.WithQuote(b => b.WithText(text));

    public static TelegramHtml Row(this TelegramHtml target, Action<TelegramHtml> build)
    {
        build(target);
        target.WithNewLine();
        return target;
    }

    public static TelegramHtml If(this TelegramHtml target, bool condition,
        Action<TelegramHtml> buildIf, Action<TelegramHtml>? buildElse = default)
    {
        if (condition)
            buildIf(target);
        else
            buildElse?.Invoke(target);
        return target;
    }

    public static TelegramHtml WithWrapping(this TelegramHtml target, string start, string end, Action<TelegramHtml> build)
    {
        target.WithText(start);
        build(target);
        target.WithText(end);
        return target;
    }
}
