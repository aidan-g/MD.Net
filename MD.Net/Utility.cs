using System;

namespace MD.Net
{
    public static class Utility
    {
        public static TimeSpan GetTimeSpan(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return TimeSpan.Zero;
            }

            var parts = value.Split(new[] { ':', '.' }, 4);
            if (parts.Length != 4)
            {
                return TimeSpan.Zero;
            }

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

            //TODO: Frames are ignored, not sure how to convert to ms.
            return new TimeSpan(hours, minutes, seconds);
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
    }
}
