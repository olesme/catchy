using Catchy;
using CatchySourceGenTests.MethodTemplateSources;
using CatchySourceGenTests.TestTypes;
using static Catchy.StatelessAlias;

namespace CatchySourceGenTests
{
    public class MethodTemplateGeneratorTests
    {
        [Test]
        public async Task MethodTemplate_GeneratesConcreteOverload_ForEachTargetType()
        {
            // The float overload must exist: [GenerateTypedOverloads(typeof(float))] on the double template.
            var floatTarget = new MethodTemplateTargetAssertions<float>(3.14f);

            MethodTemplateExtensions.RecordValue(floatTarget, 1.5f);

            await Check.That(floatTarget.CapturedTypeName).Is(typeof(float).FullName);
            await Check.That(floatTarget.CapturedValue).Is("1.5");
        }

        [Test]
        public async Task MethodTemplate_OriginalTemplateOverload_StillWorks()
        {
            // The original double overload (the template source itself) must still be callable directly.
            var doubleTarget = new MethodTemplateTargetAssertions<double>(0d);

            MethodTemplateExtensions.RecordValue(doubleTarget, 2.5d);

            await Check.That(doubleTarget.CapturedTypeName).Is(typeof(double).FullName);
            await Check.That(doubleTarget.CapturedValue).Is("2.5");
        }

        [Test]
        public async Task MethodTemplate_GeneratedOverload_SubstitutesTypeInReturnType()
        {
            // Return type should be MethodTemplateTargetAssertions<float>, not <double>.
            var floatTarget = new MethodTemplateTargetAssertions<float>(0f);

            var returned = MethodTemplateExtensions.RecordValue(floatTarget, 9.9f);

            await Check.That(ReferenceEquals(floatTarget, returned)).IsTrue();
            await Check.That(returned.GetType().GetGenericArguments()[0].Name).Is(nameof(Single));
        }
    }
}
