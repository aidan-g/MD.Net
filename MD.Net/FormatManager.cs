using System.IO;

namespace MD.Net
{
    public class FormatManager : IFormatManager
    {
        const string LP2 = "128";

        const string LP4 = "64";

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
            var output = default(string);
            var error = default(string);
            var process = this.ToolManager.Start(Tools.ATRACDENC, string.Format("{0} {1} {2} {3} {4} {5} {6} {7}", Constants.ATRACDENC_ENCODE, Constants.ATRACDENC_ATRAC3, Constants.ATRACDENC_INPUT, fileName, Constants.ATRACDENC_OUTPUT, result, Constants.ATRACDENC_BITRATE, bitrate));
            var code = this.ToolManager.Exec(process, out output, out error);
            return result;
        }
    }
}
