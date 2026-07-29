using System;

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class OverloadResolutionPriorityAttribute(int priority) : Attribute
    {
        public int Priority { get; } = priority;
    }
}
