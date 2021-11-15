using System;

namespace CryptoTrader.Backtesting
{
    public class BacktestResults
    {
        public readonly double Profit;

        public BacktestResults(double profit)
        {
            Profit = profit;
        }

        public void PrintResults()
        {
            // TODO(dolan): print actual hyperopt test results
            Console.Write("Profit: ");
            Console.WriteLine(Profit);
        }
    }
}
