namespace MD.Net
{
    public class AddTrackAction : TrackAction
    {
        public AddTrackAction(IDevice device, ITrack track) : base(device, Track.None, track)
        {

        }

        public override void Apply(IToolManager toolManager)
        {
            var output = default(string);
            var error = default(string);
            var process = toolManager.Start(Tools.NETMDCLI, string.Format("{0} {1} {2}", Constants.NETMDCLI_SEND, this.UpdatedTrack.Location, this.UpdatedTrack.Name));
            var code = toolManager.Exec(process, out output, out error);
        }
    }
}
