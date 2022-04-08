using System;

namespace MD.Net
{
    public interface ITrack : IEquatable<ITrack>
    {
        string Id { get; }

        int Position { get; set; }

        Protection Protection { get; }

        Compression Compression { get; }

        TimeSpan Time { get; }

        string Name { get; set; }

        string Location { get; set; }

        ITrack Clone();
    }

    public enum Protection
    {
        None,
        Protected
    }

    public enum Compression
    {
        None,
        LP2,
        LP4,
        Unknown
    }
}
