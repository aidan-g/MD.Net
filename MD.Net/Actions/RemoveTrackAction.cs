using MD.Net.Resources;

namespace MD.Net
{
    public class RemoveTrackAction : TrackAction
    {
        public RemoveTrackAction(IDevice device, ITrack track) : base(device, track, Track.None)
        {

        }

        public override string Description
        {
            get
            {
                return string.Format(Strings.RemoveTrackAction_Description, this.CurrentTrack.Name);
            }
        }

        public override void Apply(IToolManager toolManager, IStatus status)
        {
            var output = default(string);
            var error = default(string);
            var process = toolManager.Start(Tools.NETMDCLI, string.Format("{0} {1}", Constants.NETMDCLI_DELETE, this.CurrentTrack.Position));
            var code = toolManager.Exec(process, out output, out error);
        }
    }
}
