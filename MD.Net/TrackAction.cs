namespace MD.Net
{
    public abstract class TrackAction : ActionBase, ITrackAction
    {
        protected TrackAction(IDevice device, ITrack currentTrack, ITrack updatedTrack) : base(device)
        {
            this.CurrentTrack = currentTrack;
            this.UpdatedTrack = updatedTrack;
        }

        public ITrack CurrentTrack { get; }

        public ITrack UpdatedTrack { get; }
    }
}
