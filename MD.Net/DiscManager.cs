﻿using System;
using System.Collections.Generic;
using System.Linq;
using TinyJson;

namespace MD.Net
{
    public class DiscManager : IDiscManager
    {
        public DiscManager(IToolManager toolManager)
        {
            this.ToolManager = toolManager;
        }

        public IToolManager ToolManager { get; private set; }

        public IDisc GetDisc(IDevice device)
        {
            var output = default(string);
            var error = default(string);
            var process = this.ToolManager.Start(Tools.NETMDCLI);
            var code = this.ToolManager.Exec(process, out output, out error);
            return this.GetDisc(device, output);
        }

        protected IDisc GetDisc(IDevice device, string output)
        {
            if (string.IsNullOrEmpty(output) || output.Contains(Constants.NETMDCLI_NO_DEVICE, true) || output.Contains(Constants.NETMDCLI_ERROR, true))
            {
                return default(IDisc);
            }
            var disc = output.FromJson<_Disc>();
            if (!string.Equals(device.Name, disc.device, StringComparison.OrdinalIgnoreCase))
            {
                return default(IDisc);
            }
            return new Disc(
                disc.title,
                Utility.GetTimeSpan(disc.recordedTime),
                Utility.GetTimeSpan(disc.totalTime),
                Utility.GetTimeSpan(disc.availableTime),
                new Tracks(this.GetTracks(disc.tracks))
            );
        }

        private IEnumerable<ITrack> GetTracks(IEnumerable<_Track> tracks)
        {
            if (tracks == null)
            {
                return Enumerable.Empty<ITrack>();
            }
            return tracks.Select(track => new Track(
                track.no,
                Utility.GetProtection(track.protect),
                Utility.GetCompression(track.bitrate),
                Utility.GetTimeSpan(track.time),
                track.name
            ));
        }

        public IResult ApplyActions(IDevice device, IActions actions, IStatus status)
        {
            var message = default(string);
            foreach (var action in actions)
            {
                try
                {
                    action.Apply(this.ToolManager);
                }
                catch (Exception e)
                {
                    message = e.Message;
                    break;
                }
            }
            if (!string.IsNullOrEmpty(message))
            {
                return Result.Failure(message);
            }
            return Result.Success;
        }

        private struct _Disc
        {
            public string device;

            public string title;

            public string recordedTime;

            public string totalTime;

            public string availableTime;

            public List<_Track> tracks;
        }

        private struct _Track
        {
            public int no;

            public string protect;

            public string bitrate;

            public string time;

            public string name;
        }
    }
}
