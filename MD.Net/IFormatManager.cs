namespace MD.Net
{
    public interface IFormatManager
    {
        void Validate(string fileName);

        string Convert(string fileName, Compression compression, IStatus status);
    }
}
