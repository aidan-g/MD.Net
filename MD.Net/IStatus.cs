using System;

namespace MD.Net
{
    public interface IStatus
    {
        void Update(string message, int position, int count);

        event StatusEventHandler Updated;
    }

    public delegate void StatusEventHandler(object sender, StatusEventArgs e);

    public class StatusEventArgs : EventArgs
    {
        public StatusEventArgs(string message, int position, int count)
        {
            this.Message = message;
            this.Position = position;
            this.Count = count;
        }

        public string Message { get; private set; }

        public int Position { get; private set; }

        public int Count { get; private set; }
    }
}
