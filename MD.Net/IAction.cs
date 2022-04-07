namespace MD.Net
{
    public interface IAction
    {
        IDevice Device { get; }

        string Description { get; }

        void Apply(IToolManager toolManager, IStatus status);
    }
}
