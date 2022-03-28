namespace MD.Net
{
    public class RemoveTrackAction : TrackAction
    {
        public RemoveTrackAction(IDevice device, ITrack track) : base(device, track, Track.None)
        {

        }

        public override void Apply(IToolManager toolManager)
        {
            var output = default(string);
            var error = default(string);
            var process = toolManager.Start(Tools.NETMDCLI, string.Format("{0} {1}", Constants.NETMDCLI_DELETE, this.CurrentTrack.Position));
            var code = toolManager.Exec(process, out output, out error);
        }
    }
}
