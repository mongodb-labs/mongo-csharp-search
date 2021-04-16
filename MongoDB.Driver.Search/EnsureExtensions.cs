using MongoDB.Driver.Core.Misc;

namespace MongoDB.Driver.Search
{
    internal static class EnsureExtensions
    {
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
