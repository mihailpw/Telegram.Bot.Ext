namespace Telegram.Bot.Ext.Utils;

public static class StringUtils
{
    private const string CommandChar = "/";

    public static bool IsCommand(string? text)
        => text?.StartsWith(CommandChar) == true;

    public static string PrepareCommand(string command)
    {
        var preparedCommand = command.Split(' ', '@').First();
        return preparedCommand.StartsWith(CommandChar)
            ? preparedCommand
            : $"{CommandChar}{preparedCommand}";
    }
}
