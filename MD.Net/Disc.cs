using System;

namespace MD.Net
{
    public class Disc : IDisc
    {
        public Disc(IDisc disc) 
        {
            this.Title = disc.Title;
            this.RecordedTime = disc.RecordedTime;
            this.TotalTime = disc.TotalTime;
            this.AvailableTime = disc.AvailableTime;
            this.Tracks = disc.Tracks.Clone();
            this.Id = disc.Id;
        }

        public Disc(string title, TimeSpan recordedTime, TimeSpan totalTime, TimeSpan availableTime, ITracks tracks)
        {
            this.Title = title;
            this.RecordedTime = recordedTime;
            this.TotalTime = totalTime;
            this.AvailableTime = availableTime;
            this.Tracks = tracks;
            this.Id = GetId(this);
        }

        public string Id { get; private set; }

        public string Title { get; set; }

        public TimeSpan RecordedTime { get; }

        public TimeSpan TotalTime { get; }

        public TimeSpan AvailableTime { get; }

        public ITracks Tracks { get; }

        public static IDisc None
        {
            get
            {
                return new Disc(string.Empty, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, global::MD.Net.Tracks.None);
            }
        }

        public static string GetId(IDisc disc)
        {
            var id = default(int);
            unchecked
            {
                id += disc.Title.GetHashCode();
            }
            return Math.Abs(id).ToString();
        }
    }
}
