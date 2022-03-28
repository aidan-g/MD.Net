namespace MD.Net
{
    public interface IAction
    {
        IDevice Device { get; }

        void Apply(IToolManager toolManager);
    }
}
