namespace Catchy.Sdk
{
    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
    public class GenerateArityOverloadsAttribute(string target, int from = 2, int to = 5) : Attribute
    {
        public string Target { get; set; } = target;
        public int From { get; set; } = from;
        public int To { get; set; } = to;
    }
}
