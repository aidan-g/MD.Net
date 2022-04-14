using MD.Net.Resources;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MD.Net
{
    public class FormatManager : IFormatManager
    {
        const string LP2 = "128";

        const string LP4 = "64";

        static readonly Regex ATRACDENC_PROGRESS = new Regex(
            @"(\d\d?)%",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        public FormatManager(IToolManager toolManager)
        {
            this.ToolManager = toolManager;
        }

        public IToolManager ToolManager { get; private set; }

        public string Convert(string fileName, Compression compression, IStatus status)
        {
            switch (compression)
            {
                case Compression.LP2:
                    return this.ConvertAtrac(fileName, LP2, status);
                case Compression.LP4:
                    return this.ConvertAtrac(fileName, LP4, status);
            }
            return fileName;
        }

        protected virtual string ConvertAtrac(string fileName, string bitrate, IStatus status)
        {
            var result = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + ".at3");
            var process = this.ToolManager.Start(Tools.ATRACDENC, string.Format("{0} {1} {2} \"{3}\" {4} \"{5}\" {6} {7}", Constants.ATRACDENC_ENCODE, Constants.ATRACDENC_ATRAC3, Constants.ATRACDENC_INPUT, fileName, Constants.ATRACDENC_OUTPUT, result, Constants.ATRACDENC_BITRATE, bitrate));
            using (var emitter = new PercentStatusEmitter(string.Format(Strings.FormatManager_Description, Path.GetFileName(fileName)), StatusType.Encode, ATRACDENC_PROGRESS, status))
            {
                var error = new StringBuilder();
                var code = this.ToolManager.Exec(process, emitter.Action, data => error.AppendLine(data));
                if(code != 0)
                {
                    this.ToolManager.Throw(process, error.ToString());
                }
            }
            if (File.Exists(result))
            {
                result = this.ConvertWav(result);
            }
            return result;
        }

        protected virtual string ConvertWav(string fileName)
        {
            var result = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + ".wav");
            using (var reader = File.OpenRead(fileName))
            {
                var omaInfo = default(OMAHeader.OMAInfo);
                if (OMAHeader.Read(reader, out omaInfo))
                {
                    var wavInfo = default(WavHeader.WavInfo);
                    wavInfo.FileSize = global::System.Convert.ToInt32(new FileInfo(fileName).Length);
                    wavInfo.Format = WavHeader.WAV_FORMAT_ATRAC3;
                    wavInfo.ChannelCount = 2;
                    wavInfo.SampleRate = omaInfo.SampleRate;
                    wavInfo.ByteRate = OMAHeader.GetBitRate(omaInfo) / 8;
                    wavInfo.BlockAlign = omaInfo.Framesize;
                    wavInfo.BitsPerSample = 0;
                    using (var writer = File.Create(result))
                    {
                        WavHeader.Write(writer, wavInfo);
                        reader.CopyTo(writer);
                    }
                }
            }
            return result;
        }
    }
}
