namespace Catchy
{
    public sealed class EqualsOptions
    {
        public bool IgnoreCase { get; set; }
        public bool IgnoreNullProperties { get; set; }
        public bool IgnoreCyclicReferences { get; set; }
        public StringComparison StringComparison { get; set; } = StringComparison.Ordinal;
        public bool IgnoreCollectionOrder { get; set; }
        public bool IgnoreExtraProperties { get; set; }
        public List<string> ExcludedProperties { get; } = [];
        public double? FloatTolerance { get; set; }
        public Func<string, bool>? PropertyFilter { get; set; }

        /// <summary>Shallow copy — used by slot-mutation helpers to avoid in-place modification.</summary>
        internal EqualsOptions Clone()
        {
            var copy = new EqualsOptions
            {
                IgnoreCase = IgnoreCase,
                IgnoreNullProperties = IgnoreNullProperties,
                IgnoreCyclicReferences = IgnoreCyclicReferences,
                StringComparison = StringComparison,
                IgnoreCollectionOrder = IgnoreCollectionOrder,
                IgnoreExtraProperties = IgnoreExtraProperties,
                FloatTolerance = FloatTolerance,
                PropertyFilter = PropertyFilter,
            };
            copy.ExcludedProperties.AddRange(ExcludedProperties);
            return copy;
        }

        internal bool ShouldExclude(string propertyName) =>
            PropertyFilter?.Invoke(propertyName) == true
            || ExcludedProperties.Any(p =>
                string.Equals(p, propertyName, StringComparison.OrdinalIgnoreCase));
    }
}
