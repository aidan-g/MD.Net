using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MD.Net
{
    public class Actions : IActions
    {
        public Actions()
        {
            this.Store = new List<IAction>();
        }

        public Actions(IEnumerable<IAction> actions)
        {
            this.Store = actions.ToList();
        }

        public IList<IAction> Store { get; private set; }

        public int Count
        {
            get
            {
                return this.Store.Count;
            }
        }

        public IEnumerator<IAction> GetEnumerator()
        {
            return this.Store.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Store.GetEnumerator();
        }
    }
}
