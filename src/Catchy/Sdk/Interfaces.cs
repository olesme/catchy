namespace Catchy
{
    namespace Sdk
    {
        public interface IAssertions
        {
            AssertionPipeline GetPipeline();
            bool IsSkipped();
            void AddOp(CheckOperation op);
            void AddLink(string link);
            void AddLinks(string?[] links);
            void Skip(string? reason = null);
        }
    }
}