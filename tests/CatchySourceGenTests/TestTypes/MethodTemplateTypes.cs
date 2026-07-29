using Catchy;

namespace CatchySourceGenTests.MethodTemplateSources
{
    // Template source: the generator will clone RecordValue once for each target type,
    // substituting the inferred template type (double, from MethodTemplateTargetAssertions<double>).
    public static partial class MethodTemplateExtensions
    {
        /// <summary>Captures the numeric value as a formatted string for later inspection.</summary>
        [GenerateTypedOverloads(typeof(float))]
        public static CatchySourceGenTests.TestTypes.MethodTemplateTargetAssertions<double> RecordValue(
            CatchySourceGenTests.TestTypes.MethodTemplateTargetAssertions<double> self,
            double expected)
        {
            self.CapturedTypeName = typeof(double).FullName ?? string.Empty;
            self.CapturedValue = expected.ToString(System.Globalization.CultureInfo.InvariantCulture);
            return self;
        }
    }
}

namespace CatchySourceGenTests.TestTypes
{
    public partial class MethodTemplateTargetAssertions<T>
        where T : struct
    {
        public MethodTemplateTargetAssertions(T value)
        {
            Value = value;
        }

        public T Value { get; }
        public string? CapturedTypeName { get; set; }
        public string? CapturedValue { get; set; }
    }
}
