using System.Text;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Ext.Utils.Formatting;

// https://core.telegram.org/bots/api#markdownv2-style
public class MarkdownV2Formatting : FormattingBase
{
    private static readonly HashSet<char> EscapeSymbols
        = new() { '_', '*', '[', ']', '(', ')', '~', '`', '>', '#', '+', '-', '=', '|', '{', '}', '.', '!' };

    public MarkdownV2Formatting(bool trimNewLines = true)
        : base(ParseMode, trimNewLines)
    {
    }

    public static ParseMode ParseMode => ParseMode.MarkdownV2;

    public override IFormatting WithText(string? text, bool escape = true)
        => Do(sb => sb.Append(Escape(text, escape)));
    public override IFormatting WithNewLine(bool forceAdd = false)
        => Do(sb => sb.AppendLine(), forceAdd || !EndsWith(Environment.NewLine));
    public override IFormatting WithBold(Action<IFormatting> build)
        => Wrap("*", build);
    public override IFormatting WithItalic(Action<IFormatting> build)
        => Wrap("_", build);
    public override IFormatting WithUnderline(Action<IFormatting> build)
        => Wrap("__", build);
    public override IFormatting WithStrikethrough(Action<IFormatting> build)
        => Wrap("~", build);
    public override IFormatting WithSpoiler(Action<IFormatting> build)
        => Wrap("||", build);
    public override IFormatting WithLink(string title, string url, bool escapeTitle = true)
        => Do(sb => sb.Append('[').Append(Escape(title, escapeTitle)).Append("](").Append(Escape(url)).Append(')'));
    public override IFormatting WithUserMention(string title, long userId, bool escapeTitle = true)
        => Do(sb => sb.Append('[').Append(Escape(title, escapeTitle)).Append("](tg://user?id=").Append(userId).Append(')'));
    public override IFormatting WithInlineCode(Action<IFormatting> build)
        => Wrap("`", build);
    public override IFormatting WithCodeBlock(string? lang, Action<IFormatting> build)
        => Wrap($"```{Escape(lang)}", "```", build, true);
    public override IFormatting WithQuote(Action<IFormatting> build, bool expandable = true)
        => Wrap(">", Environment.NewLine, build);

    private IFormatting Do(Action<StringBuilder> action, bool condition = true)
    {
        if (condition)
            action(Sb);
        return this;
    }

    private MarkdownV2Formatting Wrap(string bracket, Action<MarkdownV2Formatting> action, bool newLineBeforeAfter = false)
        => Wrap(bracket, bracket, action, newLineBeforeAfter);

    private MarkdownV2Formatting Wrap(string bracketStart, string bracketEnd, Action<MarkdownV2Formatting> action,
        bool newLineBeforeAfter = false)
    {
        Sb.Append(bracketStart);
        if (newLineBeforeAfter)
            Sb.AppendLine();
        action(this);
        if (newLineBeforeAfter && !EndsWith(Environment.NewLine))
            Sb.AppendLine();
        Sb.Append(bracketEnd);
        return this;
    }

    private static StringBuilder Escape(string? input, bool escape = true)
        => escape ? new StringBuilder().AppendEscaped(input, EscapeSymbols) : new StringBuilder(input ?? "");
}