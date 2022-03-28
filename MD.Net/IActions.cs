using System.Collections.Generic;

namespace MD.Net
{
    public interface IActions : IEnumerable<IAction>
    {
        int Count { get; }
    }
}
