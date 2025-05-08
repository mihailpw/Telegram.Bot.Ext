using System.Text;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Ext.Utils.Formatting;

public abstract class FormattingBase : IFormatting, IEquatable<IFormatting>
{
    private readonly bool _trimNewLines;

    protected FormattingBase(ParseMode telegramParseMode, bool trimNewLines = true)
    {
        _trimNewLines = trimNewLines;
        TelegramParseMode = telegramParseMode;
    }

    public ParseMode TelegramParseMode { get; }

    protected StringBuilder Sb { get; } = new();

    public IFormatting With(IFormatting inner)
    {
        if (TelegramParseMode != inner.TelegramParseMode)
            throw new InvalidOperationException("Inner format doesn't match the current format");
        Sb.Append(inner.Build());
        return this;
    }

    public abstract IFormatting WithText(string? text, bool escape = true);
    public abstract IFormatting WithNewLine(bool forceAdd = false);
    public abstract IFormatting WithBold(Action<IFormatting> build);
    public abstract IFormatting WithItalic(Action<IFormatting> build);
    public abstract IFormatting WithUnderline(Action<IFormatting> build);
    public abstract IFormatting WithStrikethrough(Action<IFormatting> build);
    public abstract IFormatting WithSpoiler(Action<IFormatting> build);
    public abstract IFormatting WithLink(string title, string url, bool escapeTitle = true);
    public abstract IFormatting WithUserMention(string title, long userId, bool escapeTitle = true);
    public abstract IFormatting WithInlineCode(Action<IFormatting> build);
    public abstract IFormatting WithCodeBlock(string? lang, Action<IFormatting> build);
    public abstract IFormatting WithQuote(Action<IFormatting> build, bool expandable = true);

    public string Build()
    {
        if (_trimNewLines)
            while (EndsWith(Environment.NewLine))
                Sb.Length -= Environment.NewLine.Length;

        return Sb.ToString();
    }

    protected bool EndsWith(string val)
    {
        if (Sb.Length < val.Length)
            return false;
        for (var i = 1; i <= val.Length; i++)
            if (Sb[^i] != val[^i])
                return false;

        return true;
    }

    #region Equality

    protected virtual bool Equals(FormattingBase other)
        => TelegramParseMode == other.TelegramParseMode && Sb.Equals(other.Sb);

    public bool Equals(IFormatting? other)
        => other is FormattingBase fb && Equals(fb);

    public override bool Equals(object? obj)
        => obj is not null && (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((FormattingBase)obj));

    public override int GetHashCode()
        => HashCode.Combine((int)TelegramParseMode, Sb);

    #endregion
}