using MongoDB.Driver.Core.Misc;
using MongoDB.Bson;

namespace MongoDB.Labs.Search
{
    public class FuzzyOptions
    {
        private int? _maxEdits;
        private int? _prefixLength;
        private int? _maxExpansions;

        public int? MaxEdits
        {
            get { return _maxEdits; }
            set { _maxEdits = EnsureExtensions.IsNullOrBetween(value, 1, 2, nameof(value)); }
        }

        public int? PrefixLength
        {
            get { return _prefixLength; }
            set { _prefixLength = Ensure.IsNullOrGreaterThanOrEqualToZero(value, nameof(value)); }
        }

        public int? MaxExpansions
        {
            get { return _maxExpansions; }
            set { _maxExpansions = Ensure.IsNullOrGreaterThanZero(value, nameof(value)); }
        }

        internal BsonDocument Render()
        {
            BsonDocument document = new BsonDocument();
            if (_maxEdits != null)
            {
                document.Add("maxEdits", _maxEdits);
            }
            if (_prefixLength != null)
            {
                document.Add("prefixLength", _prefixLength);
            }
            if (_maxExpansions != null)
            {
                document.Add("maxExpansions", _maxExpansions);
            }
            return document;
        }
    }
}
