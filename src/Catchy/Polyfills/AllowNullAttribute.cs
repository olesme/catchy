#if !NETSTANDARD2_1_OR_GREATER && !NET5_0_OR_GREATER
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    internal sealed class AllowNullAttribute : Attribute { }
}
#endif
