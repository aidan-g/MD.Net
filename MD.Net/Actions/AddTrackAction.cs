using MD.Net.Resources;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MD.Net
{
    public class AddTrackAction : TrackAction
    {
        static readonly Regex NETMDCLI_PROGRESS = new Regex(
            @"(?<" + DefaultStatusEmitter.GROUP_POSITION + @">\d+) of (?<" + DefaultStatusEmitter.GROUP_COUNT + @">\d+) bytes \(\d\d?%\) transferred",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture
        );

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
            var process = default(Process);
            if (!string.IsNullOrEmpty(this.UpdatedTrack.Name))
            {
                process = toolManager.Start(Tools.NETMDCLI, string.Format("{0} \"{1}\" \"{2}\" {3}", Constants.NETMDCLI_SEND, this.UpdatedTrack.Location, this.UpdatedTrack.Name, Constants.NETMDCLI_VERBOSE));
            }
            else
            {
                process = toolManager.Start(Tools.NETMDCLI, string.Format("{0} \"{1}\" {2}", Constants.NETMDCLI_SEND, this.UpdatedTrack.Location, Constants.NETMDCLI_VERBOSE));
            }
            using (var emitter = new DefaultStatusEmitter(this.Description, StatusType.Transfer, NETMDCLI_PROGRESS, status))
            {
                var error = new StringBuilder();
                var code = toolManager.Exec(process, emitter.Action, data => error.AppendLine(data));
                if (code != 0)
                {
                    toolManager.Throw(process, error.ToString());
                }
            }
        }

        public override void Commit()
        {
            this.CurrentDisc.Tracks.Add(this.UpdatedTrack);
            base.Commit();
        }
    }
}
