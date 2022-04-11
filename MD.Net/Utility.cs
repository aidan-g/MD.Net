﻿using System;

namespace MD.Net
{
    public static class Utility
    {
        //Source: https://www.minidisc.org/minidisc_faq.html
        const float MS_PER_FRAME = 11.6f;

        public static TimeSpan GetTimeSpan(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return TimeSpan.Zero;
            }

            var parts = value.Split(new[] { ':', '.' }, 4);
            if (parts.Length == 3)
            {
                var minutes = default(int);
                var seconds = default(int);
                var frames = default(int);

                if (!int.TryParse(parts[0], out minutes))
                {
                    return TimeSpan.Zero;
                }
                if (!int.TryParse(parts[1], out seconds))
                {
                    return TimeSpan.Zero;
                }
                if (!int.TryParse(parts[2], out frames))
                {
                    return TimeSpan.Zero;
                }

                return new TimeSpan(0, 0, minutes, seconds, FramesToMS(frames));
            }
            else if (parts.Length == 4)
            {

                var hours = default(int);
                var minutes = default(int);
                var seconds = default(int);
                var frames = default(int);

                if (!int.TryParse(parts[0], out hours))
                {
                    return TimeSpan.Zero;
                }
                if (!int.TryParse(parts[1], out minutes))
                {
                    return TimeSpan.Zero;
                }
                if (!int.TryParse(parts[2], out seconds))
                {
                    return TimeSpan.Zero;
                }
                if (!int.TryParse(parts[3], out frames))
                {
                    return TimeSpan.Zero;
                }

                return new TimeSpan(0, hours, minutes, seconds, FramesToMS(frames));
            }
            else
            {
                return TimeSpan.Zero;
            }
        }

        public static Protection GetProtection(string protect)
        {
            if (!string.Equals(protect, Constants.LIBNETMD_UNPROT, StringComparison.OrdinalIgnoreCase))
            {
                return Protection.Protected;
            }
            return Protection.None;
        }

        public static Compression GetCompression(string bitrate)
        {
            if (string.Equals(bitrate, Constants.LIBNETMD_SP, StringComparison.OrdinalIgnoreCase))
            {
                return Compression.None;
            }
            if (string.Equals(bitrate, Constants.LIBNETMD_LP2, StringComparison.OrdinalIgnoreCase))
            {
                return Compression.LP2;
            }
            if (string.Equals(bitrate, Constants.LIBNETMD_LP4, StringComparison.OrdinalIgnoreCase))
            {
                return Compression.LP4;
            }
            return Compression.Unknown;
        }

        public static int MSToFrames(int ms)
        {
            return Convert.ToInt32((float)ms / MS_PER_FRAME);
        }

        public static int FramesToMS(int frames)
        {
            return Convert.ToInt32((float)frames * MS_PER_FRAME);
        }
    }
}
