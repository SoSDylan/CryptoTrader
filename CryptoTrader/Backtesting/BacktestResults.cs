using System;

namespace CryptoTrader.Backtesting
{
    public class BacktestResults
    {
        public readonly double ProfitPercentage;
        public readonly int TotalTradesCount;
        public readonly int SuccessfulTradesCount;
        public readonly int FailedTradesCount;

        public BacktestResults(double profitPercentage, int totalTradesCount, int successfulTradesCount, int failedTradesCount)
        {
            ProfitPercentage = profitPercentage;
            TotalTradesCount = totalTradesCount;
            SuccessfulTradesCount = successfulTradesCount;
            FailedTradesCount = failedTradesCount;
        }

        public void PrintBasicResults()
        {
            Console.Write("Profit (%): ");
            Console.WriteLine(ProfitPercentage);
        }

        public void PrintResults()
        {
            Console.WriteLine("--------- Hyperopt Results ----------");
            
            Console.Write("Profit (%): ");
            Console.WriteLine(ProfitPercentage);
            
            Console.Write("Trades (total, successful, failed): ");
            Console.Write(TotalTradesCount);
            Console.Write(", ");
            Console.Write(SuccessfulTradesCount);
            Console.Write(", ");
            Console.WriteLine(FailedTradesCount);
        }
    }
}
