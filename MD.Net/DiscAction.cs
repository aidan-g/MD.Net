﻿namespace MD.Net
{
    public abstract class DiscAction : ActionBase, IDiscAction
    {
        protected DiscAction(IDevice device, IDisc currentDisc, IDisc updatedDisc) : base(device)
        {
            this.CurrentDisc = currentDisc;
            this.UpdatedDisc = updatedDisc;
        }

        public IDisc CurrentDisc { get; }

        public IDisc UpdatedDisc { get; }
    }
}
