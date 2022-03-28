namespace MD.Net
{
    public class PlaybackManager : IPlaybackManager
    {
        public PlaybackManager(IToolManager toolManager)
        {
            this.ToolManager = toolManager;
        }

        public IToolManager ToolManager { get; private set; }

        public void Play(IDevice device, ITrack track)
        {
            var output = default(string);
            var error = default(string);
            var process = this.ToolManager.Start(Tools.NETMDCLI, string.Format("{0} {1}", Constants.NETMDCLI_PLAY, track.Position));
            var code = this.ToolManager.Exec(process, out output, out error);
        }

        public void Stop(IDevice device)
        {
            var output = default(string);
            var error = default(string);
            var process = this.ToolManager.Start(Tools.NETMDCLI, Constants.NETMDCLI_STOP);
            var code = this.ToolManager.Exec(process, out output, out error);
        }
    }
}
