using System;
using System.Diagnostics;

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
            var _output = default(Func<string>);
            var _error = default(Func<string>);
            try
            {
                return this.Exec(process, Collector<string>.Collect(StringAggregator.NewLine, out _output), Collector<string>.Collect(StringAggregator.NewLine, out _error));
            }
            finally
            {
                output = _output();
                error = _error();
            }
        }

        public int Exec(Process process, Action<string> outputHandler, Action<string> errorHandler)
        {
            process.OutputDataReceived += (sender, e) =>
            {
                outputHandler(e.Data);
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                errorHandler(e.Data);
            };
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
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
