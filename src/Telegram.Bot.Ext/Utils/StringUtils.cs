using System.Text;

namespace Telegram.Bot.Ext.Utils;

public static class StringUtils
{
    private const char CommandChar = '/';
    private const char ArgumentsSeparator = '_';
    private const char ChatSeparator = '@';

    public static bool IsCommand(string? text)
        => text?.StartsWith(CommandChar) == true;

    public static bool IsExactCommand(string? text, string command)
    {
        if (string.IsNullOrEmpty(command))
            throw new ArgumentException("Command is empty");

        if (string.IsNullOrEmpty(text))
            return false;

        command = PrepareCommand(command);
        if (!text.StartsWith(CommandChar))
            return false;
        return command == PrepareCommand(text);
    }

    public static IEnumerable<string>? GetCommandArguments(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return null;

        if (!text.StartsWith(CommandChar))
            return null;

        return text.Split(ArgumentsSeparator).Skip(1);
    }

    public static string PrepareCommand(string command)
    {
        if (string.IsNullOrEmpty(command))
            throw new ArgumentException("Command is empty");

        var preparedCommand = command.Split(ArgumentsSeparator, ChatSeparator).First();
        return preparedCommand.StartsWith(CommandChar)
            ? preparedCommand
            : $"{CommandChar}{preparedCommand}";
    }

    public static StringBuilder AppendEscaped(this StringBuilder sb, string? input, HashSet<char>? escapeSymbols = null)
    {
        if (escapeSymbols is not null && input is not null)
            foreach (var ch in input)
            {
                if (escapeSymbols.Contains(ch))
                    sb.Append('\\');
                sb.Append(ch);
            }
        else
            sb.Append(input);

        return sb;
    }

    public static StringBuilder AppendEscaped(this StringBuilder sb, string? input, Dictionary<char, string>? escapeSymbols = null)
    {
        if (escapeSymbols is not null && input is not null)
            foreach (var ch in input)
            {
                if (escapeSymbols.TryGetValue(ch, out var replaceSymbols))
                    sb.Append(replaceSymbols);
                else
                    sb.Append(ch);
            }
        else
            sb.Append(input);

        return sb;
    }
}
