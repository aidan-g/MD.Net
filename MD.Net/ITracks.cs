using System.Collections.Generic;

namespace MD.Net
{
    public interface ITracks : IEnumerable<ITrack>
    {
        ITrack this[int position] { get; }

        int Count { get; }

        ITrack Add();

        void Remove(ITrack track);

        ITracks Clone();
    }
}
