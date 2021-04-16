using MongoDB.Driver.Core.Misc;
using System;

namespace MongoDB.Driver.Search
{
    internal static class EnsureExtensions
    {
        public static double IsGreaterThanZero(double value, string paramName)
        {
            if (value <= 0)
            {
                var message = string.Format("Value is not greater than zero: {0}", value);
                throw new ArgumentOutOfRangeException(paramName, message);
            }
            return value;
        }

        public static int? IsNullOrBetween(int? value, int min, int max, string paramName)
        {
            if (value != null)
            {
                Ensure.IsBetween(value.Value, min, max, paramName);
            }
            return value;
        }
    }
}
