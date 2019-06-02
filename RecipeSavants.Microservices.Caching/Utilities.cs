using System;
using System.Collections.Generic;
using System.Text;

namespace RecipeSavants.Microservices.Caching
{
    public enum CacheType
    {
        Absolute,
        Sliding
    };

    public enum RedisCacheLife //defined in minutes
    {
        Tiny = 60,
        Short = 192,
        Medium = 576,
        Long = 1152,
        ExtraLong = 4032,
        UTCMidnight = 0
    }

    public static class Utilities
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static double MinutesToMidnight()
        {
            return (DateTime.Today.AddDays(1.0) - DateTime.Now).TotalMinutes;
        }
    }
}
