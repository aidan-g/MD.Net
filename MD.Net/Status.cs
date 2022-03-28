namespace MD.Net
{
    public class Status : IStatus
    {
        public void Update(string message, int position, int count)
        {
            if (this.Updated == null)
            {
                return;
            }
            this.Updated(this, new StatusEventArgs(message, position, count));
        }

        public event StatusEventHandler Updated;

        public static IStatus Ignore
        {
            get
            {
                return new Status();
            }
        }
    }
}
