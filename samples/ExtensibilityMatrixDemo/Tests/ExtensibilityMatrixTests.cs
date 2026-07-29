using CatchyTestHelpers;
using static Catchy.StatelessAlias;

namespace ExtensibilityMatrixDemo.Tests;

public class ExtensibilityMatrixTests
{
    [Test]
    public async Task AssertFor_and_plain_extension_work_together()
    {
        var invoice = new Invoice { Number = "INV-001", Total = 42m };

        await Check.That(invoice)
            .HasPositiveTotal()
            .And()
            .HasNumber("INV-001")
            .And()
            .HasNonEmptyNumber();
    }

    [Test]
    public async Task Assertable_generated_members_are_available()
    {
        var customer = new Customer { Name = "Olena", Age = 21 };

        await Check.That(customer)
            .HasName("Olena")
            .And()
            .HasAge(21);
    }

    [Test]
    public async Task AssertFor_failure_can_be_validated()
    {
        var invoice = new Invoice { Number = "INV-001", Total = -1m };

        var msg = await TestHelpers.ShouldFailWithMessageAsync(async () =>
            await Check.That(invoice).HasPositiveTotal());

        await Assert.That(msg).Contains("positive total");
    }
}
