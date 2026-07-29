#if !NET7_0_OR_GREATER
namespace System.Runtime.CompilerServices
{
    [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public sealed class CompilerFeatureRequiredAttribute : Attribute { }
}
#endif
