using System;
using System.Collections.Generic;

namespace MD.Net
{
    public interface ITracks : IEnumerable<ITrack>, IEquatable<ITracks>
    {
        ITrack this[int position] { get; }

        int Count { get; }

        ITrack Add();

        void Remove(ITrack track);

        void Clear();

        ITracks Clone();
    }
}
