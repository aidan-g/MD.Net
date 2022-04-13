using System;

namespace MD.Net
{
    public static class Collector<T>
    {
        public static Action<T> Collect(IAggregator<T> aggregator, out Func<T> value)
        {
            try
            {
                return aggregator.Append;
            }
            finally
            {
                value = () => aggregator.Aggregate();
            }
        }
    }
}
