using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Catchy
{
    public sealed class AssertionResult<T>
    {
        private T? _value;
        private bool _hasValue;

        internal void Set(T value) { _value = value; _hasValue = true; }

        public T Value => _hasValue
            ? _value!
            : throw new InvalidOperationException(
                "AssertionResult has no value yet — await the assertion chain first.");

        [EditorBrowsable(EditorBrowsableState.Never)]
        public TaskAwaiter<T> GetAwaiter() => Task.FromResult(Value).GetAwaiter();
    }
}
