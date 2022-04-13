using System;
using System.IO;

namespace MD.Net
{
    public static class OMAHeader
    {
        public const int OMA_HEADER_SIZE = 96;

        public const byte OMA_CODEC_ATRAC3 = 0;

        public const byte OMA_CODEC_ATRAC3PLUS = 1;

        public static int[] OMA_SAMPLERATES = { 32000, 44100, 48000, 88200, 96000 };

        public const byte OMA_MONO = 0;

        public const byte OMA_STEREO = 1;

        public const byte OMA_JOINT_STEREO = 2;

        public static int[] OMA_CHANNEL_ID = { OMA_MONO, OMA_STEREO };

        public static bool Read(Stream stream, out OMAInfo info)
        {
            var buffer = new byte[OMA_HEADER_SIZE];
            if (stream.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                //No header?
                info = default(OMAInfo);
                return false;
            }
            if (buffer[0] != 'E' || buffer[1] != 'A' || buffer[2] != '3')
            {
                //Not OMA?
                info = default(OMAInfo);
                return false;
            }
            if (buffer[6] != 0xff || buffer[7] != 0xff)
            {
                //Encrypted?
                info = default(OMAInfo);
                return false;
            }
            var parameters = ReadParameters(buffer[33], buffer[34], buffer[35]);
            switch (buffer[32])
            {
                case OMA_CODEC_ATRAC3:
                    return ReadAtrac3(parameters, out info);
                case OMA_CODEC_ATRAC3PLUS:
                    return ReadAtrac3Plus(parameters, out info);
                default:
                    //Unrecognized codec.
                    info = default(OMAInfo);
                    return false;
            }
        }

        private static int ReadParameters(byte a, byte b, byte c)
        {
            return a << 16 | b << 8 | c;
        }

        private static bool ReadAtrac3(int parameters, out OMAInfo info)
        {
            var jointStereo = (parameters >> 0x11) & 0x1;
            var sampleRate = GetSampleRate((parameters >> 0xd) & 0x7);
            if (sampleRate == 0)
            {
                //Unrecognized sample rate.
                info = default(OMAInfo);
                return false;
            }
            info.Codec = OMA_CODEC_ATRAC3;
            info.Framesize = (parameters & 0x3FF) * 0x8;
            info.SampleRate = sampleRate;
            info.ChannelFormat = jointStereo == 0 ? OMA_STEREO : OMA_JOINT_STEREO;
            return true;
        }

        //This code is untested, I don't have access to any ATRAC3plus media.
        private static bool ReadAtrac3Plus(int parameters, out OMAInfo info)
        {
            var sampleRate = GetSampleRate((parameters >> 13) & 0x7);
            if (sampleRate == 0)
            {
                //Unrecognized sample rate.
                info = default(OMAInfo);
                return false;
            }
            var channelFormat = GetChannelFormat((parameters >> 10) & 7);
            if (channelFormat == 0)
            {
                //Unrecognized channel format.
                info = default(OMAInfo);
                return false;
            }
            info.Codec = OMA_CODEC_ATRAC3PLUS;
            info.Framesize = ((parameters & 0x3FF) * 8) + 8;
            info.SampleRate = sampleRate;
            info.ChannelFormat = channelFormat;
            return true;
        }

        public static bool Write(Stream stream, OMAInfo info)
        {
            var writer = new BinaryWriter(stream);
            writer.Write('E');
            writer.Write('A');
            writer.Write('3');
            writer.Write(0x1);
            writer.Write(0x0);
            writer.Write(0x60);
            writer.Write(0xff);
            writer.Write(0xff);
            while (stream.Position < 31)
            {
                writer.Write(0x0);
            }
            writer.Write((byte)info.Codec);
            var parameters = WriteParameters(info.Framesize, info.SampleRate, info.ChannelFormat);
            writer.Write(parameters[0]);
            writer.Write(parameters[1]);
            writer.Write(parameters[2]);
            while (stream.Position < OMA_HEADER_SIZE)
            {
                writer.Write(0x0);
            }
            return true;
        }

        private static byte[] WriteParameters(int framesize, int sampleRate, int channelFormat)
        {
            throw new NotImplementedException();
        }

        private static int GetChannelFormat(int index)
        {
            if (index < OMA_CHANNEL_ID.Length)
            {
                return OMA_CHANNEL_ID[index];
            }
            return 0;
        }

        private static int GetSampleRate(int index)
        {
            if (index < OMA_SAMPLERATES.Length)
            {
                return OMA_SAMPLERATES[index];
            }
            return 0;
        }

        public struct OMAInfo
        {
            public int Codec;

            public int Framesize;

            public int SampleRate;

            public int ChannelFormat;
        }
    }
}
