﻿using System;

namespace MD.Net
{
    public class Track : ITrack
    {
        public Track(ITrack track)
        {
            this.Position = track.Position;
            this.Protection = track.Protection;
            this.Compression = track.Compression;
            this.Time = track.Time;
            this.Name = track.Name;
            this.Id = track.Id;
        }

        public Track(int position, Protection protection, Compression compression, TimeSpan time, string name)
        {
            this.Position = position;
            this.Protection = protection;
            this.Compression = compression;
            this.Time = time;
            this.Name = name;
            this.Id = GetId(this);
        }

        public string Id { get; private set; }

        public int Position { get; set; }

        public Protection Protection { get; private set; }

        public Compression Compression { get; private set; }

        public TimeSpan Time { get; }

        public string Name { get; set; }

        public string Location { get; set; }

        public ITrack Clone()
        {
            return new Track(this);
        }

        public static ITrack None
        {
            get
            {
                return new Track(-1, Protection.None, Compression.None, TimeSpan.Zero, string.Empty);
            }
        }

        public static string GetId(ITrack track)
        {
            var id = default(int);
            unchecked
            {
                id += track.Position.GetHashCode();
                id += track.Name.GetHashCode();
            }
            return Math.Abs(id).ToString();
        }
    }
}
