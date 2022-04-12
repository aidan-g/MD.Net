namespace MD.Net
{
    public abstract class Aggregator<T>: IAggregator<T>
    {
        public abstract void Append(T value);

        public abstract T Aggregate();
    }
}
