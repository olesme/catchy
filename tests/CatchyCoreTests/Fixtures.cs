using Catchy;

namespace CatchyCoreTests
{
    public record UserEntity(int Id, string Name, string Email, int Age);

    public record UserDto(int Id, string Name, string Email, int Age);

    public class Address
    {
        public string Street { get; init; } = "";
        public string City { get; init; } = "";
        public string Zip { get; init; } = "";
    }

    public class Order
    {
        public int Id { get; init; }
        public string Customer { get; init; } = "";
        public List<string> Items { get; init; } = [];
        public double Total { get; init; }
        public Address? Shipping { get; init; }
    }

    public class OrderSummary          // cross-type, partial overlap
    {
        public int Id { get; init; }
        public string Customer { get; init; } = "";
        public double Total { get; init; }
    }

    public class ProductEntity
    {
        public int Id { get; init; }
        public string Name { get; init; } = "";
        public double Price { get; init; }
    }

    public record ProductDto(int Id, string Name, decimal Price); // Price type differs

    public static class AssertionMessageCapture
    {
        /// <summary>Runs an assertion chain and returns the thrown exception message, or null if it passed.</summary>
        public static async Task<string?> CaptureFailureMessageAsync(Func<Task> assertion)
        {
            try
            {
                await assertion();
                return null;
            }
            catch (AssertionException ex)
            {
                return ex.Message;
            }
        }
    }
}
