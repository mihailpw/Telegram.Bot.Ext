using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Ext.Utils.Formatting;

public interface IFormatting
{
    ParseMode TelegramParseMode { get; }

    IFormatting With(IFormatting inner);
    IFormatting WithText(string? text, bool escape = true);
    IFormatting WithNewLine(bool forceAdd = false);
    IFormatting WithBold(Action<IFormatting> build);
    IFormatting WithItalic(Action<IFormatting> build);
    IFormatting WithUnderline(Action<IFormatting> build);
    IFormatting WithStrikethrough(Action<IFormatting> build);
    IFormatting WithSpoiler(Action<IFormatting> build);
    IFormatting WithLink(string title, string url, bool escapeTitle = true);
    IFormatting WithUserMention(string title, long userId, bool escapeTitle = true);
    IFormatting WithInlineCode(Action<IFormatting> build);
    IFormatting WithCodeBlock(string? lang, Action<IFormatting> build);
    IFormatting WithQuote(Action<IFormatting> build, bool expandable = true);

    string Build();
}