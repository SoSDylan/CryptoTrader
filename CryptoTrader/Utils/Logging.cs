using Spectre.Console;

namespace CryptoTrader.Utils
{
    public static class Logging
    {
        public static void LogError(string error)
        {
            AnsiConsole.MarkupLine($"[red]{error}[/]");
        }
    }
}
