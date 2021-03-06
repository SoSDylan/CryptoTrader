using System;
using CryptoTrader.Core;
using CryptoTrader.Indicators;

namespace TestAddon.Indicators
{
    public class MoonPhases : Indicator
    {
        public MoonPhases(Candles candles) : base(candles)
        {
        }

        public double? this[int index]
        {
            get
            {
                var candle = Candles[index];

                if (candle == null)
                    return null;
                
                var closeTime = candle.CloseTime;
                
                return MoonAge(closeTime.Day, closeTime.Month, closeTime.Year);
            }
        }
        
        private int JulianDate(int d, int m, int y)
        { 
            int mm, yy;
            int k1, k2, k3;
            int j;

            yy = y - (int)((12 - m) / 10);
            mm = m + 9;
            if (mm >= 12)
            {
                mm = mm - 12;
            }
            
            k1 = (int)(365.25 * (yy + 4712));
            k2 = (int)(30.6001 * mm + 0.5);
            k3 = (int)((int)((yy / 100) + 49) * 0.75) - 38;
            
            // 'j' for dates in Julian calendar:
            j = k1 + k2 + d + 59;
            
            if (j > 2299160)
            {
                // For Gregorian calendar:
                j = j - k3; // 'j' is the Julian date at 12h UT (Universal Time)
            }
            
            return j;
        }
        
        private int MoonAge(int d, int m, int y)
        { 
            double ag;
            int j = JulianDate(d, m, y);
            
            // Calculate the approximate phase of the moon
            double ip = (j + 4.867) / 29.53059;
            ip = ip - Math.Floor(ip); 
            
            // After several trials I've seen to add the following lines,
            // which gave the result was not bad
            if(ip < 0.5)
                ag = ip * 29.53059 + 29.53059 / 2;
            else
                ag = ip * 29.53059 - 29.53059 / 2;
            
            // Moon's age in days
            ag = Math.Floor(ag) + 1;
            
            return (int) ag;
        }
    }
}
