using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace MD.Net
{
    public static class WavHeader
    {
        public const int WAV_HEADER_SIZE = 20;

        public const int WAV_TAG_ATRAC3 = 0x270;

        public static bool Read(Stream stream, out WavInfo info)
        {
            var buffer = new byte[WAV_HEADER_SIZE];
            if (stream.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                //No header?
                info = default(WavInfo);
                return false;
            }
            if (buffer[0] != 'R' || buffer[1] != 'I' || buffer[2] != 'F' || buffer[3] != 'F')
            {
                //Not RIFF?
                info = default(WavInfo);
                return false;
            }
            var fileSize = Utility.LEWord32(buffer, 4);
            if (buffer[8] != 'W' || buffer[9] != 'A' || buffer[10] != 'V' || buffer[11] != 'E')
            {
                //Not WAVE?
                info = default(WavInfo);
                return false;
            }
            if (buffer[12] != 'f' || buffer[13] != 'm' || buffer[14] != 't' || buffer[15] != ' ')
            {
                //Not WAVE?
                info = default(WavInfo);
                return false;
            }
            var formatChunkSize = Utility.LEWord32(buffer, 16);
            if (!ReadChunk(stream, formatChunkSize, out buffer))
            {
                //EOF?
                info = default(WavInfo);
                return false;
            }
            var format = Utility.LEWord16(buffer, 0);
            var channelCount = Utility.LEWord16(buffer, 2);
            var sampleRate = Utility.LEWord32(buffer, 4);
            var byteRate = Utility.LEWord32(buffer, 8);
            var blockAlign = Utility.LEWord16(buffer, 12);
            var bitsPerSample = Utility.LEWord16(buffer, 14);
            info.FileSize = fileSize;
            info.Format = format;
            info.ChannelCount = channelCount;
            info.SampleRate = sampleRate;
            info.ByteRate = byteRate;
            info.BlockAlign = blockAlign;
            info.BitsPerSample = bitsPerSample;
            //This is later populated by the data chunk size.
            info.DataSize = 0;
            //If we have additional data, store it.
            if (formatChunkSize > 16)
            {
                info.Data = buffer.Skip(16).Take(formatChunkSize - 16).ToArray();
            }
            else
            {
                info.Data = null;
            }
            var chunks = new List<WaveChunk>();
            do
            {
                buffer = new byte[8];
                if (stream.Read(buffer, 0, buffer.Length) != buffer.Length)
                {
                    //No header?
                    info = default(WavInfo);
                    return false;
                }
                var chunkSize = Utility.LEWord32(buffer, 4);
                if (buffer[0] != 'd' || buffer[1] != 'a' || buffer[2] != 't' || buffer[3] != 'a')
                {
                    var name = Encoding.Default.GetString(buffer, 0, 4);
                    if (!ReadChunk(stream, chunkSize, out buffer))
                    {
                        //EOF?
                        break;
                    }
                    chunks.Add(new WaveChunk()
                    {
                        Name = name,
                        Data = buffer
                    });
                }
                else
                {
                    info.DataSize = chunkSize;
                    break;
                }
            } while (true);
            info.Chunks = chunks.ToArray();
            return true;
        }

        private static bool ReadChunk(Stream stream, int size, out byte[] buffer)
        {
            if (size <= 0 || size > 1024)
            {
                //Invalid chunk size.
                buffer = default(byte[]);
                return false;
            }
            buffer = new byte[size];
            return stream.Read(buffer, 0, buffer.Length) == buffer.Length;
        }

        public static bool Write(Stream stream, WavInfo info)
        {
            var writer = new BinaryWriter(stream);
            writer.Write('R');
            writer.Write('I');
            writer.Write('F');
            writer.Write('F');
            writer.Write(Utility.LEWord32(info.FileSize));
            writer.Write('W');
            writer.Write('A');
            writer.Write('V');
            writer.Write('E');
            writer.Write('f');
            writer.Write('m');
            writer.Write('t');
            writer.Write(' ');
            if (info.Data != null)
            {
                writer.Write(Utility.LEWord32(16 + info.Data.Length));
            }
            else
            {
                writer.Write(Utility.LEWord32(16));
            }
            writer.Write(Utility.LEWord16(info.Format));
            writer.Write(Utility.LEWord16(info.ChannelCount));
            writer.Write(Utility.LEWord32(info.SampleRate));
            writer.Write(Utility.LEWord32(info.ByteRate));
            writer.Write(Utility.LEWord16(info.BlockAlign));
            writer.Write(Utility.LEWord16(info.BitsPerSample));
            if (info.Data != null)
            {
                writer.Write(info.Data);
            }
            if (info.Chunks != null)
            {
                foreach (var chunk in info.Chunks)
                {
                    writer.Write(Encoding.ASCII.GetBytes(chunk.Name.ToCharArray(), 0, 4));
                    writer.Write(Utility.LEWord32(chunk.Data.Length));
                    writer.Write(chunk.Data);
                }
            }
            writer.Write('d');
            writer.Write('a');
            writer.Write('t');
            writer.Write('a');
            writer.Write(Utility.LEWord32(info.DataSize));
            return true;
        }

        public struct WavInfo
        {
            public int FileSize;

            public int Format;

            public int ChannelCount;

            public int SampleRate;

            public int ByteRate;

            public int BlockAlign;

            public int BitsPerSample;

            public int DataSize;

            public byte[] Data;

            public WaveChunk[] Chunks;
        }

        public struct WaveChunk
        {
            public string Name;

            public byte[] Data;
        }
    }
}
