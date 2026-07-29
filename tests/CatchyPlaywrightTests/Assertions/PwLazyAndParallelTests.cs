using Catchy;
using CatchyPlaywrightTests.Support;
using CatchyTestHelpers;

namespace CatchyPlaywrightTests.Assertions
{
    public sealed class PwLazyAndParallelTests : PlaywrightTestFixture
    {
        [Test]
        public async Task DefaultTimeout_UsedWhenWithTimeoutNotCalled()
        {
            await UseDynamicPageAsync();
            var page = EnsurePage();

            await Stateless.Assert.That(page.Locator("#title")).HasText("stable-title");
        }

        [Test]
        public async Task TimeoutModifier_AppliedLazily_WhenSetAfterAssertion()
        {
            await UseDynamicPageAsync();
            var page = EnsurePage();

            await Stateless.Assert.That(page.Locator("#late-element"))
                .IsVisible()
                .WithTimeout(1200);
        }

        [Test]
        public async Task IgnoringCase_WithTimeoutModifier_BothAppliedLazily()
        {
            await UseDynamicPageAsync();
            var page = EnsurePage();

            await Stateless.Assert.That(page.Locator("#title"))
                .ContainsText("TITLE")
                .IgnoringCase()
                .WithTimeout(1200);

            var failMessage = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("#title"))
                    .ContainsText("MISSING-TOKEN")
                    .IgnoringCase()
                    .WithTimeout(1200));

            await Stateless.Assert.That(failMessage).Contains(".ContainsText(\"MISSING-TOKEN\")");
            await Stateless.Assert.That(failMessage).Contains(".IgnoringCase()");
            await Stateless.Assert.That(failMessage).Contains(".WithTimeout(1200)");
        }

        [Test]
        public async Task MultipleModifiers_ChainedAtEnd_AppliedCorrectly()
        {
            await UseDynamicPageAsync();
            var page = EnsurePage();

            await Stateless.Assert.That(page.Locator("#title"))
                .ContainsText("TITLE")
                .IgnoringCase()
                .UsingInnerText()
                .WithTimeout(1200);

            var failMessage = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("#title"))
                    .ContainsText("MISSING-TOKEN")
                    .IgnoringCase()
                    .UsingInnerText()
                    .WithTimeout(1200));

            await Stateless.Assert.That(failMessage).Contains(".ContainsText(\"MISSING-TOKEN\")");
            await Stateless.Assert.That(failMessage).Contains(".IgnoringCase()");
            await Stateless.Assert.That(failMessage).Contains(".UsingInnerText()");
            await Stateless.Assert.That(failMessage).Contains(".WithTimeout(1200)");
        }

        [Test]
        public async Task LazyEvaluation_MultipleAssertions_EachUsesOwnSlot()
        {
            await UseDynamicPageAsync();
            var page = EnsurePage();
            var title = page.Locator("#title");

            // Try with very short timeout - should fail because "stable-title" appears at 1100ms
            var failMessage = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(title)
                    .HasText("stable-title")
                    .WithTimeout(50)); // Much shorter to avoid race

            await Stateless.Assert.That(failMessage).Contains("Locator expected to have text 'stable-title'");
            await Stateless.Assert.That(failMessage).Contains(@"Expect ""ToHaveTextAsync"" with timeout 50ms");

            // Now with sufficient timeout - should pass
            await Stateless.Assert.That(title)
                .HasText("stable-title")
                .WithTimeout(2200);
        }

        [Test]
        public async Task StatefulAsserter_TimeoutModifier_AppliedLazily()
        {
            await UseDynamicPageAsync();
            var page = EnsurePage();
            var stateful = Asserter.NewStateful();

            await stateful.That(page.Locator("#late-element"))
                .IsVisible()
                .WithTimeout(1200);
        }

        [Test]
        public async Task SoftAsserter_TimeoutModifier_AppliedToEachAssertion()
        {
            await UseDynamicPageAsync();
            var page = EnsurePage();
            var verify = Asserter.NewSoft();
            var title = page.Locator("#title");

            await verify.That(title)
                .HasText("stable-title")
                .WithTimeout(2500);

            await Stateless.Assert.That(verify.HasFailures).IsFalse();
            await Stateless.Assert.That(verify.ErrorCount).Is(0);

            await verify.That(title)
                .HasText("never-happens")
                .WithTimeout(150);

            await Stateless.Assert.That(verify.HasFailures).IsTrue();
            await Stateless.Assert.That(verify.ErrorCount).Is(1);
            await Stateless.Assert.That(verify.Errors).HasCount(1);
            await Stateless.Assert.That(verify.Errors[0].Message).Contains("never-happens");
            await Stateless.Assert.That(verify.Errors[0].Message).Contains("timeout 150").IgnoringCase();
        }

        [Test]
        public async Task ParallelAssertions_MultipleThreads()
        {
            var tasks = Enumerable.Range(1, 16)
                .Select(async i => await Stateless.Assert.That($"value-{i}").Contains("value"));

            await Task.WhenAll(tasks);
        }
    }
}
