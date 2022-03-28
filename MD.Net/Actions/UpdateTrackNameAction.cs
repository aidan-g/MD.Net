namespace MD.Net
{
    public class UpdateTrackNameAction : TrackAction
    {
        public UpdateTrackNameAction(IDevice device, ITrack currentTrack, ITrack updatedTrack) : base(device, currentTrack, updatedTrack)
        {

        }

        public override void Apply(IToolManager toolManager)
        {
            var output = default(string);
            var error = default(string);
            var process = toolManager.Start(Tools.NETMDCLI, string.Format("{0} {1} {2}", Constants.NETMDCLI_RENAME, this.CurrentTrack.Position, this.UpdatedTrack.Name));
            var code = toolManager.Exec(process, out output, out error);
        }
    }
}
