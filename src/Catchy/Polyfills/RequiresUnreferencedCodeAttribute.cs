#if !NET5_0_OR_GREATER

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Method, Inherited = false)]
    public sealed class RequiresUnreferencedCodeAttribute(string message) : Attribute
    {
        public string Message { get; } = message;
    }
}
#endif
