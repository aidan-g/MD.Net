using MD.Net.Resources;
using System;
using System.IO;
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
            var error = default(string);
            var result = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + ".at3");
            var process = this.ToolManager.Start(Tools.ATRACDENC, string.Format("{0} {1} {2} \"{3}\" {4} \"{5}\" {6} {7}", Constants.ATRACDENC_ENCODE, Constants.ATRACDENC_ATRAC3, Constants.ATRACDENC_INPUT, fileName, Constants.ATRACDENC_OUTPUT, result, Constants.ATRACDENC_BITRATE, bitrate));
            using (var emitter = new PercentStatusEmitter(string.Format(Strings.FormatManager_Description, Path.GetFileName(fileName)), StatusType.Encode, ATRACDENC_PROGRESS, status))
            {
                var code = this.ToolManager.Exec(process, emitter.Action, Collector<string>.Collect(StringAggregator.NewLine, out error));
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
                var info = default(OMAHeader.OMAInfo);
                if (OMAHeader.Read(reader, out info))
                {
                    using (var writer = File.Create(result))
                    {
                        //WavHeader.Write(writer);
                        reader.CopyTo(writer);
                    }
                }
            }
            return result;
        }
    }
}
