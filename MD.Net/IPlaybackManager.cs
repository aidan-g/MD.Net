namespace MD.Net
{
    public interface IPlaybackManager
    {
        void Play(IDevice device, ITrack track);

        void Stop(IDevice device);
    }
}
