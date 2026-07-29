namespace Catchy
{
    public enum OrderingDirection
    {
        Unknown = 0,
        Ascending = 1,
        Descending = 2
    }

    public interface IOrderingDirectionProvider
    {
        OrderingDirection Direction { get; }
    }

    public interface IOrderingRule<T>
    {
        int Compare(T x, T y);
    }
}
