using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CryptoTrader.Core;
using CryptoTrader.Hyperopts;
using CryptoTrader.Hyperopts.Loss;
using CryptoTrader.Strategy;
using CryptoTrader.Utils;
using Spectre.Console.Cli;

namespace CryptoTrader.Commands
{
    internal class HyperoptSettings : CommandSettings
    {
        [CommandArgument(0, "<STRATEGY>")]
        public string Strategy { get; set; }
        
        [CommandArgument(1, "<HYPEROPT_LOSS>")]
        public string HyperoptLoss { get; set; }
        
        [Description("The cryptocurrency symbol to use")]
        [CommandOption("-s|--symbol")]
        [DefaultValue("BTCUSDT")]
        public string Symbol { get; set; }
        
        [Description("Number of epochs to run")]
        [CommandOption("-e|--epochs")]
        [DefaultValue(100)]
        public int Epochs { get; set; }
        
        [Description("Interval for the candles in seconds")]
        [CommandOption("-i|--interval")]
        [DefaultValue(720)]
        public int Interval { get; set; }
        
        [Description("Number of days to get candles")]
        [CommandOption("-d|--days")]
        [DefaultValue(100)]
        public int Days { get; set; }
    }
    
    internal class HyperoptCommand : AsyncCommand<HyperoptSettings>
    {
        public override async Task<int> ExecuteAsync(CommandContext context, HyperoptSettings settings)
        {
            var strategy = ReflectionUtils.GetStrategyFromClassName(settings.Strategy);
            var hyperoptLoss = ReflectionUtils.GetHyperoptLossFromClassName(settings.HyperoptLoss);

            var candles = await DownloadManager.DownloadAndParseCandles(settings.Symbol,
                settings.Interval,
                DateTime.Now.AddDays(-settings.Days),
                DateTime.Now.AddDays(0));

            var strategyInstance = (BaseStrategy) Activator.CreateInstance(strategy)!;
            var hyperoptLossInstance = (IHyperoptLoss) Activator.CreateInstance(hyperoptLoss)!;

            var hyperopt = new Hyperopt(strategyInstance, hyperoptLossInstance, candles, settings.Epochs);
            
            hyperopt.Optimize();
            
            return 0;
        }
    }
}
