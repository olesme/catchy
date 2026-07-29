using Catchy;

namespace CatchySourceGenTests.TestTypes
{
    public interface ITestPersonGeneratedMarker { }

    [Assertable]
    public class TestAddress
    {
        [AssertMember]
        public string City { get; set; } = string.Empty;
    }

    public class NonAssertableProfile
    {
        public string Bio { get; set; } = string.Empty;
    }

    [Assertable(MarkerInterfaces = new[] { "CatchySourceGenTests.TestTypes.ITestPersonGeneratedMarker" })]
    public class TestPerson
    {
        [AssertMember]
        public string Name { get; set; } = string.Empty;

        [AssertMember]
        public int Age { get; set; }

        [AssertMember]
        public TestAddress Address { get; set; } = new();

        [AssertMember]
        public NonAssertableProfile Profile { get; set; } = new();
    }

    [Assertable(BaseAssertionType = typeof(StructuralAssertions<>))]
    public class StructuralTestPerson
    {
        [AssertMember]
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// A custom assertion type used to verify <see cref="AssertMemberAttribute.TransitionType"/> support.
    /// </summary>
    public class CustomAddressAssertions(global::Catchy.Sdk.AssertionPipeline pipeline, TestAddress value)
        : global::Catchy.ValueAssertions<TestAddress>(pipeline, value)
    {
        public bool IsCustom => true;
    }

    [Assertable]
    public class PersonWithCustomTransition
    {
        [AssertMember(TransitionType = typeof(CustomAddressAssertions))]
        public TestAddress HomeAddress { get; set; } = new();
    }

    [Assertable]
    public class PrimaryCtorAssertable(string name, int age)
    {
        [AssertMember]
        public string Name { get; } = name;

        [AssertMember]
        public int Age { get; } = age;
    }
}
