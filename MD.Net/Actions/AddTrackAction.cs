using MD.Net.Resources;
using System.IO;

namespace MD.Net
{
    public class AddTrackAction : TrackAction
    {
        public AddTrackAction(IDevice device, ITrack track) : base(device, Track.None, track)
        {

        }

        public override string Description
        {
            get
            {
                return string.Format(Strings.AddTrackAction_Description, Path.GetFileName(this.UpdatedTrack.Location));
            }
        }

        public override void Prepare(IToolManager toolManager, IStatus status)
        {
            var formatManager = new FormatManager(toolManager);
            this.UpdatedTrack.Location = formatManager.Convert(this.UpdatedTrack.Location, this.UpdatedTrack.Compression, status);
            base.Prepare(toolManager, status);
        }

        public override void Apply(IToolManager toolManager, IStatus status)
        {
            var output = default(string);
            var error = default(string);
            var process = toolManager.Start(Tools.NETMDCLI, string.Format("{0} \"{1}\" \"{2}\"", Constants.NETMDCLI_SEND, this.UpdatedTrack.Location, this.UpdatedTrack.Name));
            var code = toolManager.Exec(process, out output, out error);
            if (code != 0)
            {
                toolManager.Throw(process, error);
            }
        }
    }
}
