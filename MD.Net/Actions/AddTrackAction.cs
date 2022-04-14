using MD.Net.Resources;
using System.Diagnostics;
using System.IO;

namespace MD.Net
{
    public class AddTrackAction : TrackAction
    {
        public AddTrackAction(IDevice device, IDisc currentDisc, IDisc updatedDisc, ITrack track) : base(device, currentDisc, updatedDisc, Track.None, track)
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
            if (this.UpdatedTrack is Track track)
            {
                track.Location = formatManager.Convert(this.UpdatedTrack.Location, this.UpdatedTrack.Compression, status);
            }
            else
            {
                //Model is not writable, cannot convert.
            }
            base.Prepare(toolManager, status);
        }

        public override void Apply(IToolManager toolManager, IStatus status)
        {
            var output = default(string);
            var error = default(string);
            var process = default(Process);
            if (!string.IsNullOrEmpty(this.UpdatedTrack.Name))
            {
                process = toolManager.Start(Tools.NETMDCLI, string.Format("{0} \"{1}\" \"{2}\"", Constants.NETMDCLI_SEND, this.UpdatedTrack.Location, this.UpdatedTrack.Name));
            }
            else
            {
                process = toolManager.Start(Tools.NETMDCLI, string.Format("{0} \"{1}\"", Constants.NETMDCLI_SEND, this.UpdatedTrack.Location));
            }
            var code = toolManager.Exec(process, out output, out error);
            if (code != 0)
            {
                toolManager.Throw(process, error);
            }
        }

        public override void Commit()
        {
            this.CurrentDisc.Tracks.Add(this.UpdatedTrack);
            base.Commit();
        }
    }
}
