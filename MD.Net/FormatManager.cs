using MD.Net.Resources;
using System;
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

        public void Validate(string fileName)
        {
            if (!this.IsValid(fileName))
            {
                throw new WaveFormatException(fileName);
            }
        }

        protected virtual bool IsValid(string fileName)
        {
            using (var reader = File.OpenRead(fileName))
            {
                var info = default(WavHeader.WavInfo);
                if (!WavHeader.Read(reader, out info))
                {
                    return false;
                }
                if (info.SampleRate != 44100)
                {
                    return false;
                }
                if (info.BitsPerSample != 16)
                {
                    return false;
                }
                if (info.ChannelCount != 2)
                {
                    return false;
                }
            }
            return true;
        }

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
                if (code != 0)
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
                    wavInfo.FileSize = WavHeader.GetFileSize(reader, wavInfo);
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

    public class WaveFormatException : Exception
    {
        public WaveFormatException(string fileName) : base(GetMessage(fileName))
        {
            this.FileName = fileName;
        }

        public string FileName { get; private set; }

        private static string GetMessage(string fileName)
        {
            return string.Format(Strings.WaveFormatException_Message, fileName);
        }
    }
}
