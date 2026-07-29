using System;
using Catchy;

namespace CatchySourceGenTests.TestTypes
{
    [Assertable("CatchySourceGenTests.TestTypes.PersonSnapshot")]
    public sealed class PersonModel
    {
        [AssertMember(UseStringComparison = true, StringComparison = StringComparison.OrdinalIgnoreCase)]
        public string Name { get; set; } = string.Empty;

        [AssertMember(MapTo = "Years")]
        public int Age { get; set; }

        [AssertMember(Skip = true)]
        public string InternalToken { get; set; } = string.Empty;
    }

    public sealed class PersonSnapshot
    {
        public string Name { get; set; } = string.Empty;
        public int Years { get; set; }
        public string InternalToken { get; set; } = string.Empty;
    }
}
