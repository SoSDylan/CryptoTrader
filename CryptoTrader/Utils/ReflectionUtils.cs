using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using CryptoTrader.Hyperopts.Loss;
using CryptoTrader.Strategy;
using DarkIce.Toolkit.Core.Extensions;
using Spectre.Console;

namespace CryptoTrader.Utils
{
    internal static class ReflectionUtils
    {
        private static List<Assembly> _assemblies;
    
        internal static Type GetStrategyFromClassName(string className)
        {
            var strategy = GetStrategies().FirstOrDefault(x => x.Name == className);
            
            if (strategy == null)
                throw new Exception($"Strategy \"{className}\" not found\n" +
                                    $"       Strategies: {string.Join(", ", GetStrategies().Select(x => x.Name))}");

            return strategy;
        }
        
        internal static Type GetHyperoptLossFromClassName(string className)
        {
            var hyperoptLoss = GetHyperoptLosses().FirstOrDefault(x => x.Name == className);
            
            if (hyperoptLoss == null)
                throw new Exception($"Hyperopt Loss \"{className}\" not found\n" +
                                    $"       Hyperopt Losses: {string.Join(", ", GetHyperoptLosses().Select(x => x.Name))}");

            return hyperoptLoss;
        }
        
        internal static List<Type> GetStrategies()
        {
            
            return GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(BaseStrategy).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .ToList();
        }
        
        internal static List<Type> GetHyperoptLosses()
        {
            return GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(IHyperoptLoss).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .ToList();
        }
        
        private static List<Assembly> GetAssemblies()
        {
            if (_assemblies.IsNullOrDefault())
            {
                _assemblies = new List<Assembly>();
                _assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());
                
                string addonsPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) +
                              Path.DirectorySeparatorChar + "Addons";

                try
                {
                    foreach (string dll in Directory.GetFiles(addonsPath, "*.dll"))
                    {
                        _assemblies.Add(Assembly.LoadFile(dll));
                    }
                }
                catch (IOException)
                {
                    AnsiConsole.MarkupLine("[yellow bold]Addons directory not found, trying to create directory at[/]");
                    AnsiConsole.MarkupLine($"[yellow bold]\"{addonsPath}\"[/]");
                    
                    try
                    {
                        Directory.CreateDirectory(addonsPath);
                        AnsiConsole.MarkupLine("[green bold]Created addons directory[/]");
                    }
                    catch
                    {
                        AnsiConsole.MarkupLine("[yellow bold]Could not create addons directory[/]");
                    }

                    return _assemblies;
                }
            }

            return _assemblies;
        }
    }
}
