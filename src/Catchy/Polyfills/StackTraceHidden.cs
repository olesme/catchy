#if !NET6_0_OR_GREATER
namespace System.Diagnostics
{
    [global::System.AttributeUsage(global::System.AttributeTargets.Method, AllowMultiple = false)]
    public sealed class StackTraceHiddenAttribute : global::System.Attribute { }
}
#endif
