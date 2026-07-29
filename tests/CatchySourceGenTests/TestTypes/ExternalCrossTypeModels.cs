using System;
using Catchy;

namespace CatchySourceGenTests.TestTypes
{
    public sealed class ExternalOrderEntity
    {
        public string CustomerName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string InternalCode { get; set; } = string.Empty;
    }

    public sealed class ExternalOrderDto
    {
        public string Name { get; set; } = string.Empty;
        public int Qty { get; set; }
        public string Extra { get; set; } = string.Empty;
    }

    [CrossTypeRule(typeof(ExternalOrderEntity), typeof(ExternalOrderDto), StringComparison = StringComparison.OrdinalIgnoreCase, IgnoreExtraFields = true, AutoMapFields = false)]
    [CrossTypeMemberMap(nameof(ExternalOrderEntity.CustomerName), nameof(ExternalOrderDto.Name), UseStringComparison = true)]
    [CrossTypeMemberMap(nameof(ExternalOrderEntity.Quantity), nameof(ExternalOrderDto.Qty))]
    public static partial class ExternalOrderRuleDeclarations
    {
    }

    // Intentionally conflicts with inline [Assertable("...PersonSnapshot")] registration
    // to validate deterministic precedence (inline/generated rule should win).
    [CrossTypeRule(typeof(PersonModel), typeof(PersonSnapshot), StringComparison = StringComparison.Ordinal, IgnoreExtraFields = false, AutoMapFields = false)]
    [CrossTypeMemberMap(nameof(PersonModel.Name), nameof(PersonSnapshot.Name), UseStringComparison = false)]
    [CrossTypeMemberMap(nameof(PersonModel.Age), nameof(PersonSnapshot.Years))]
    public static partial class ExternalPersonRuleConflictDeclarations
    {
    }
}
