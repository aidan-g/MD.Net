using System;
using System.IO;

namespace MD.Net.Tests
{
    public static class Utility
    {
        public static void EmitLines(string value, Action<string> handler)
        {
            using (var reader = new StringReader(value))
            {
                var line = default(string);
                while ((line = reader.ReadLine()) != null)
                {
                    handler(line);
                }
            }
        }
    }
}
