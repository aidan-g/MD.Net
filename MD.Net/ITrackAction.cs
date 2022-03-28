namespace MD.Net
{
    public interface ITrackAction : IAction
    {
        ITrack CurrentTrack { get; }

        ITrack UpdatedTrack { get; }
    }
}
