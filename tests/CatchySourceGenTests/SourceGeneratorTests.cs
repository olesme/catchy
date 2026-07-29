using Catchy;
using CatchySourceGenTests.TestTypes;
using CatchyTestHelpers;
using static Catchy.StatelessAlias;

namespace CatchySourceGenTests
{
    public class SourceGeneratorTests
    {
        [Test]
        public async Task Assertable_GeneratesFieldAssertions_ForAnnotatedMembers()
        {
            var person = new TestPerson { Name = "John", Age = 30 };

            await Check.That(person)
                .HasName("John")
                .And()
                .HasAge(30);
        }

        [Test]
        public async Task Assertable_GeneratedFieldAssertion_FailsWithExpectedMessage()
        {
            var person = new TestPerson { Name = "John", Age = 30 };

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Check.That(person).HasName("Jane"));

            await Check.That(message).Contains("Expected");
            await Check.That(message).Contains("HasName");
            await Check.That(message).Contains("Name = Jane");
        }

        [Test]
        public async Task Assertable_GeneratesQuantifiedEntryPoints_WithGeneratedTypes()
        {
            var people = new[]
            {
                new TestPerson { Name = "Alice", Age = 25 },
                new TestPerson { Name = "Bob", Age = 30 },
                new TestPerson { Name = "Charlie", Age = 35 }
            };

            var each = Check.ThatEachOf(people);
            var any = Check.ThatAnyOf(people);
            var none = Check.ThatNoneOf(people);

            await Check.That(each.GetType().Name).Contains("QuantifiedAssertions");
            await Check.That(any.GetType().Name).Contains("QuantifiedAssertions");
            await Check.That(none.GetType().Name).Contains("QuantifiedAssertions");
        }

        [Test]
        public async Task Assertable_GeneratesQuantifiedArityOverloads()
        {
            var person1 = new TestPerson { Name = "Alice", Age = 25 };
            var person2 = new TestPerson { Name = "Bob", Age = 30 };
            var person3 = new TestPerson { Name = "Charlie", Age = 35 };

            var each2 = Check.ThatEachOf(new[] { person1, person2 });
            var any3 = Check.ThatAnyOf(new[] { person1, person2, person3 });
            var none2 = Check.ThatNoneOf(new[] { person1, person2 });

            await Check.That(each2.GetType().Name).Contains("QuantifiedAssertions");
            await Check.That(any3.GetType().Name).Contains("QuantifiedAssertions");
            await Check.That(none2.GetType().Name).Contains("QuantifiedAssertions");
        }

        [Test]
        public async Task AssertFor_GeneratesCustomAssertionMethods()
        {
            var freezingTemp = new Temperature { Celsius = -10 };
            var boilingTemp = new Temperature { Celsius = 110 };
            var normalTemp = new Temperature { Celsius = 25 };

            await Check.That(freezingTemp).IsFreezing();
            await Check.That(boilingTemp).IsBoiling();

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Check.That(normalTemp).IsFreezing());
            await Check.That(message).Contains("Expected");
            await Check.That(message).Contains("IsFreezing");
            await Check.That(message).Contains("freezing");
        }

        [Test]
        public async Task Assertable_Supports_Classic_And_PrimaryConstructor_Syntax()
        {
            var classic = new TestPerson { Name = "John", Age = 30 };
            var primary = new PrimaryCtorAssertable("John", 30);

            await Check.That(classic)
                .HasName("John")
                .And()
                .HasAge(30);

            await Check.That(primary)
                .HasName("John")
                .And()
                .HasAge(30);
        }

        [Test]
        public async Task AssertFor_GenericAndNonGenericAttributes_BothGenerateAssertions()
        {
            var humidity = new Humidity { Percent = 45 };
            var muggy = new Humidity { Percent = 80 };

            await Check.That(humidity).IsComfortable();

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Check.That(muggy).IsComfortable());
            await Check.That(message).Contains("Expected");
            await Check.That(message).Contains("IsComfortable");
            await Check.That(message).Contains("comfortable");
        }

        [Test]
        public async Task GeneratedAssertions_ChainWithCommonAndGeneratedMethods()
        {
            var person = new TestPerson { Name = "Test", Age = 25 };

            await Check.That(person)
                .HasNameContains("es").And()
                .HasAgeGreaterThan(20).And()
                .IsNotNull();
        }

        [Test]
        public async Task Assertable_GeneratedAssertions_ImplementConfiguredMarkerInterface()
        {
            var person = new TestPerson { Name = "John", Age = 30 };
            var assertion = Check.That(person);
            var interfaceNames = string.Join("|", assertion.GetType().GetInterfaces().Select(i => i.FullName));

            await Check.That(interfaceNames).Contains("CatchySourceGenTests.TestTypes.ITestPersonGeneratedMarker");
        }

        [Test]
        public async Task Assertable_GeneratedAssertions_ApplyNullableTemplate_ForReferenceTypeAssertables()
        {
            var person = new TestPerson { Name = "John", Age = 30 };

            await Check.That(person).IsNotNull();
        }

        [Test]
        public async Task AssertFor_GeneratedAssertions_ApplyNullableTemplate_ForReferenceTypeTargets()
        {
            var humidity = new Humidity { Percent = 45 };

            await Check.That(humidity).IsNotNull();
        }

        [Test]
        public async Task AssertFor_GeneratedAssertions_ApplyNullableTemplate_ForNullableReferenceTargets()
        {
            HumidityReading? reading = new HumidityReading { Percent = 60 };

            await Check.That(reading).IsNotNull();
            await Check.That(reading).HasValue();
        }

        [Test]
        public async Task AssertFor_GeneratedAssertions_ApplyNullableTemplate_ForNullableReferenceTargets_NullPassesIsNull()
        {
            HumidityReading? reading = null;

            await Check.That(reading).IsNull();
        }

        [Test]
        public async Task AssertFor_GeneratedAssertions_ForStructTarget_AreAvailable()
        {
            var reading = new HumidityReading { Percent = 60 };

            await Check.That(reading).IsHumid();

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Check.That(new HumidityReading { Percent = 40 }).IsHumid());

            await Check.That(message).Contains("Expected");
            await Check.That(message).Contains("humid");
        }

        [Test]
        public async Task Assertable_GeneratesPropertyTransitions_ToFieldAssertionTypes()
        {
            var person = new TestPerson { Name = "Test", Age = 25 };

            await Check.That(person).Name().Is("Test");
            await Check.That(person).Age().Is(25);
        }

        [Test]
        public async Task Assertable_GeneratesPropertyTransitions_ForReferenceTypeMembers()
        {
            var person = new TestPerson
            {
                Name = "Test",
                Age = 25,
                Address = new TestAddress { City = "Kyiv" }
            };

            await Check.That(person).Address().IsNotNull();
        }

        [Test]
        public async Task Assertable_GeneratesNestedPropertyTransitions_ForAssertableMembers()
        {
            var person = new TestPerson
            {
                Name = "Test",
                Age = 25,
                Address = new TestAddress { City = "Kyiv" }
            };

            await Check.That(person).Address().City().Is("Kyiv");
        }

        [Test]
        public async Task Assertable_NestedTransition_ReturnsValueAssertions_ByDefault()
        {
            var person = new TestPerson
            {
                Name = "Test",
                Age = 25,
                Address = new TestAddress { City = "Kyiv" },
                Profile = new NonAssertableProfile { Bio = "About" }
            };

            var addressAssertions = Check.That(person).Address();

            // Default transition always returns ValueAssertions<T>, even when member type is [Assertable]
            await Check.That(addressAssertions.GetType().Name).Contains("ValueAssertions");
            await Check.That(addressAssertions.GetType().Name).DoesNotContain("TestAddressAssertions");
            await Check.That(person).Address().City().Is("Kyiv");
        }

        [Test]
        public async Task Assertable_ReferenceMember_UsesValueAssertions_WhenMemberTypeIsNotAssertable()
        {
            var person = new TestPerson
            {
                Name = "Test",
                Age = 25,
                Address = new TestAddress { City = "Kyiv" },
                Profile = new NonAssertableProfile { Bio = "About" }
            };

            var profileAssertions = Check.That(person).Profile();

            // Non-[Assertable] member transitions land on ValueAssertions<T>, not a concrete wrapper
            await Check.That(profileAssertions.GetType().Name).Contains("ValueAssertions");
            await Check.That(profileAssertions.GetType().Name).DoesNotContain("NonAssertableProfileAssertions");
            await Check.That(profileAssertions.GetType().Name).DoesNotContain("StructuralAssertions");
            await Check.That(person).Profile().IsNotNull();
        }

        [Test]
        public async Task Assertable_NonAssertableReferenceMember_DoesNotExposeNestedTransitionMembers()
        {
            var person = new TestPerson
            {
                Name = "Test",
                Age = 25,
                Address = new TestAddress { City = "Kyiv" },
                Profile = new NonAssertableProfile { Bio = "About" }
            };

            var profileAssertions = Check.That(person).Profile();
            var publicInstancePropertyNames = profileAssertions.GetType()
                .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Select(p => p.Name)
                .ToArray();

            await Check.That(publicInstancePropertyNames).DoesNotContain("Bio");
        }

        [Test]
        public async Task Assertable_Can_Target_Structural_Base_Surface()
        {
            var actual = new StructuralTestPerson { Name = "John" };
            var expected = new StructuralTestPerson { Name = "John" };

            var assertions = Check.That(actual);

            await Check.That(typeof(StructuralAssertions<StructuralTestPerson>).IsAssignableFrom(assertions.GetType())).Is(true);
            await assertions.HasName("John");
            await assertions.IsEquivalentTo(expected);
        }

        [Test]
        public async Task CrossType_GeneratedDeepEqualRule_AppliesWithoutManualWiring()
        {
            var actual = new PersonModel
            {
                Name = "JOHN",
                Age = 42,
                InternalToken = "actual-token"
            };

            var expected = new PersonSnapshot
            {
                Name = "john",
                Years = 42,
                InternalToken = "expected-token"
            };

            await Check.That<object>(actual).IsEquivalentTo(expected);
        }

        [Test]
        public async Task CrossType_GeneratedDeepEqualRule_FailsOnMappedDifference()
        {
            var actual = new PersonModel
            {
                Name = "JOHN",
                Age = 41,
                InternalToken = "actual-token"
            };

            var expected = new PersonSnapshot
            {
                Name = "john",
                Years = 42,
                InternalToken = "expected-token"
            };

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Check.That<object>(actual).IsEquivalentTo(expected));

            await Check.That(message).Contains("Expected");
            await Check.That(message).Contains("equivalent to");
            await Check.That(message).Contains("differ");
        }

        [Test]
        public async Task CrossTypeRule_GeneratedDeepEqualRule_AppliesWithoutTargetAttributes()
        {
            var actual = new ExternalOrderEntity
            {
                CustomerName = "ALICE",
                Quantity = 4,
                InternalCode = "E-01"
            };

            var expected = new ExternalOrderDto
            {
                Name = "alice",
                Qty = 4,
                Extra = "ignored"
            };

            await Check.That<object>(actual).IsEquivalentTo(expected);
        }

        [Test]
        public async Task CrossTypeRule_GeneratedDeepEqualRule_FailsOnMappedDifference()
        {
            var actual = new ExternalOrderEntity
            {
                CustomerName = "ALICE",
                Quantity = 5,
                InternalCode = "E-01"
            };

            var expected = new ExternalOrderDto
            {
                Name = "alice",
                Qty = 4,
                Extra = "ignored"
            };

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Check.That<object>(actual).IsEquivalentTo(expected));

            await Check.That(message).Contains("Expected");
            await Check.That(message).Contains("equivalent to");
            await Check.That(message).Contains("differ");
        }

        [Test]
        public async Task CrossTypeRule_DoesNotOverrideInlineAssertableRule_ForSamePair()
        {
            var actual = new PersonModel
            {
                Name = "JOHN",
                Age = 42,
                InternalToken = "actual-token"
            };

            var expected = new PersonSnapshot
            {
                Name = "john",
                Years = 42,
                InternalToken = "expected-token"
            };

            await Check.That<object>(actual).IsEquivalentTo(expected);
        }

        [Test]
        public async Task GeneratedAssertions_WorkWithSoftAssertions()
        {
            var stateful = Asserter.NewStateful();
            var person = new TestPerson { Name = "John", Age = 30 };

            await stateful.Soft.That(person).HasAge(31);

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Check.That(stateful.Soft).HasNoErrors());

            await Check.That(message).Contains("Expected");
            await Check.That(message).Contains("HasAge");
            await Check.That(message).Contains("31");
            await Check.That(message).Contains("HasNoErrors");
        }

        [Test]
        public async Task GeneratedAssertions_WorkWithExecutionWrappers_AndTrailingModifiers()
        {
            var hookCount = 0;
            var stateful = Asserter.NewStateful(s =>
            {
                s.OnExecution.Add(async (_, next) =>
                {
                    hookCount++;
                    await next().ConfigureAwait(false);
                });
            });

            var person = new TestPerson { Name = "John", Age = 30 };

            await stateful.That(person)
                .HasName("John")
                .And()
                .HasAgeGreaterThan(20)
                .Because("generated assertions should respect trailing modifiers");

            await Check.That(hookCount).Is(1);
        }

        [Test]
        public async Task Assertable_TransitionType_UsesCustomAssertionType_WhenSpecifiedInAssertMember()
        {
            var person = new PersonWithCustomTransition
            {
                HomeAddress = new TestAddress { City = "Lviv" }
            };

            // The generated HomeAddress() transition must return CustomAddressAssertions, not ValueAssertions<TestAddress>
            var addressAssertions = Check.That(person).HomeAddress();

            await Check.That(addressAssertions.GetType().Name).Is(nameof(CustomAddressAssertions));
            await Check.That(addressAssertions.IsCustom).Is(true);
            await Check.That(person).HomeAddress().City().Is("Lviv");
        }
    }
}
