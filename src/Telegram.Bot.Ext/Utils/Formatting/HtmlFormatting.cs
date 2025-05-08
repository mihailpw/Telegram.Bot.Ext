using System.Text;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Ext.Utils.Formatting;

// https://core.telegram.org/bots/api#html-style
public class HtmlFormatting : FormattingBase
{
    private static readonly Dictionary<char, string> EscapeSymbols = new()
    {
        ['<'] = "&lt;",
        ['>'] = "&gt;",
        ['&'] = "&amp;",
    };

    public HtmlFormatting(bool trimNewLines = true)
        : base(ParseMode.Html, trimNewLines)
    {
    }

    public override IFormatting WithText(string? text, bool escape = true)
        => Do(sb => sb.Append(Escape(text, escape)));
    public override IFormatting WithNewLine(bool forceAdd = false)
        => Do(sb => sb.AppendLine(), forceAdd || !EndsWith(Environment.NewLine));
    public override IFormatting WithBold(Action<IFormatting> build)
        => Wrap("b", build);
    public override IFormatting WithItalic(Action<IFormatting> build)
        => Wrap("i", build);
    public override IFormatting WithUnderline(Action<IFormatting> build)
        => Wrap("u", build);
    public override IFormatting WithStrikethrough(Action<IFormatting> build)
        => Wrap("s", build);
    public override IFormatting WithSpoiler(Action<IFormatting> build)
        => Wrap("tg-spoiler", build);
    public override IFormatting WithLink(string title, string url, bool escapeTitle = true)
        => Do(sb => sb.Append($"<a href=\"{Escape(url)}\">{Escape(title, escapeTitle)}</a>"));
    public override IFormatting WithUserMention(string title, long userId, bool escapeTitle = true)
        => Do(sb => sb.Append($"<a href=\"tg://user?id={userId}\">{Escape(title, escapeTitle)}</a>"));
    public override IFormatting WithInlineCode(Action<IFormatting> build)
        => Wrap("code", build);
    public override IFormatting WithCodeBlock(string? lang, Action<IFormatting> build)
        => Wrap("pre", b =>
        {
            if (lang is null) build(b);
            else ((HtmlFormatting)b).Wrap($"code class=\"{Escape(lang)}\"", "code", build);
        });
    public override IFormatting WithQuote(Action<IFormatting> build, bool expandable = true)
        => Wrap($"blockquote {(expandable ? "expandable" : "")}", "blockquote", build);

    private IFormatting Do(Action<StringBuilder> action, bool condition = true)
    {
        if (condition)
            action(Sb);
        return this;
    }

    private IFormatting Wrap(string tag, Action<IFormatting> action, bool newLineBeforeAfter = false)
        => Wrap(tag, tag, action, newLineBeforeAfter);

    private IFormatting Wrap(string tagStart, string tagEnd, Action<IFormatting> action,
        bool newLineBeforeAfter = false)
    {
        Sb.Append($"<{tagStart}>");
        if (newLineBeforeAfter)
            Sb.AppendLine();
        action(this);
        if (newLineBeforeAfter && !EndsWith(Environment.NewLine))
            Sb.AppendLine();
        Sb.Append($"</{tagEnd}>");
        return this;
    }

    private static StringBuilder Escape(string? input, bool escape = true)
        => escape ? new StringBuilder().AppendEscaped(input, EscapeSymbols) : new StringBuilder(input ?? "");
}
