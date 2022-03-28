using System;
using System.Collections.Generic;

namespace MD.Net
{
    public class ActionBuilder : IActionBuilder
    {
        public IActions GetActions(IDevice device, IDisc currentDisc, IDisc updatedDisc)
        {
            var actions = new List<IAction>();
            if (!string.Equals(currentDisc.Title, updatedDisc.Title, StringComparison.OrdinalIgnoreCase))
            {
                actions.Add(new UpdateDiscTitleAction(device, currentDisc, updatedDisc));
            }
            foreach (var updatedTrack in updatedDisc.Tracks)
            {
                var found = default(bool);
                foreach (var currentTrack in currentDisc.Tracks)
                {
                    if (updatedTrack.Id != currentTrack.Id)
                    {
                        continue;
                    }
                    found = true;
                }
                if (!found)
                {
                    this.AddTrack(actions, device, updatedTrack);
                }
            }
            foreach (var currentTrack in currentDisc.Tracks)
            {
                var found = default(bool);
                foreach (var updatedTrack in updatedDisc.Tracks)
                {
                    if (currentTrack.Id != updatedTrack.Id)
                    {
                        continue;
                    }
                    this.UpdateTrack(actions, device, currentTrack, updatedTrack);
                    found = true;
                }
                if (!found)
                {
                    this.RemoveTrack(actions, device, currentTrack);
                }
            }
            return new Actions(actions);
        }

        protected virtual void AddTrack(IList<IAction> actions, IDevice device, ITrack track)
        {
            actions.Add(new AddTrackAction(device, track));
        }

        protected virtual void UpdateTrack(IList<IAction> actions, IDevice device, ITrack currentTrack, ITrack updatedTrack)
        {
            if (!string.Equals(currentTrack.Name, updatedTrack.Name, StringComparison.OrdinalIgnoreCase))
            {
                actions.Add(new UpdateTrackNameAction(device, currentTrack, updatedTrack));
            }
        }

        protected virtual void RemoveTrack(IList<IAction> actions, IDevice device, ITrack track)
        {
            actions.Add(new RemoveTrackAction(device, track));
        }
    }
}
