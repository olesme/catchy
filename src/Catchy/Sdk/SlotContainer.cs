namespace Catchy.Sdk
{
    public sealed class SlotKey<T>
    {
        // Typed key
    }

    public sealed class SlotContainer
    {
        private Dictionary<object, object>? _dict = null;

        public void Set<T>(SlotKey<T> key, T value)
        {
            _dict ??= [];
            _dict[key] = value!;
        }

        public bool TryGet<T>(SlotKey<T> key, out T value)
        {
            if (_dict is not null && _dict.TryGetValue(key, out var obj))
            {
                if (obj is T v)
                {
                    value = v;
                    return true;
                }
            }

            value = default!;
            return false;
        }

        public T Get<T>(SlotKey<T> key)
        {
            return (T)_dict![key];
        }
    }
}
