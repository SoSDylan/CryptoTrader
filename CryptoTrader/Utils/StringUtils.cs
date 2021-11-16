namespace CryptoTrader.Utils
{
    public static class StringUtils
    {
        internal static string AsTableTitle(this string title)
        {
            return $"[grey][[[/] [yellow bold]{title}[/] [grey]]][/]";
        }
    }
}
