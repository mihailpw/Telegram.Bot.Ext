namespace Telegram.Bot.Ext.Utils;

public static class StringUtils
{
    private const string CommandChar = "/";

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

    public static string PrepareCommand(string command)
    {
        if (string.IsNullOrEmpty(command))
            throw new ArgumentException("Command is empty");

        var preparedCommand = command.Split(' ', '@').First();
        return preparedCommand.StartsWith(CommandChar)
            ? preparedCommand
            : $"{CommandChar}{preparedCommand}";
    }
}
