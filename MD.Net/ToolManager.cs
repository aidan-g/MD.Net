using System;
using System.Diagnostics;
using System.Text;

namespace MD.Net
{
    public class ToolManager : IToolManager
    {
        public Process Start(string path, string args)
        {
            var info = new ProcessStartInfo(path, args);
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.UseShellExecute = false;
            info.CreateNoWindow = true;
            return Process.Start(info);
        }

        public int Exec(Process process, out string output, out string error)
        {
            var _output = new StringBuilder();
            var _error = new StringBuilder();
            process.OutputDataReceived += (sender, e) =>
            {
                _output.AppendLine(e.Data);
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                _error.AppendLine(e.Data);
            };
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            output = _output.ToString();
            error = _error.ToString();
            return process.ExitCode;
        }

        public void Throw(Process process, string message)
        {
            throw new ToolException(process.StartInfo.FileName, process.StartInfo.Arguments, process.ExitCode, message);
        }
    }

    public class ToolException : Exception
    {
        public ToolException(string path, string args, int code, string message) : base(GetMessage(path, args, code, message))
        {
            this.Path = path;
            this.Args = args;
            this.Code = code;
        }

        public string Path { get; private set; }

        public string Args { get; private set; }

        public int Code { get; private set; }

        private static string GetMessage(string path, string args, int code, string message)
        {
            return string.Format("Tool process \"{0} {1}\" exited with code {2}: {3}", path, args, code, message);
        }
    }
}
