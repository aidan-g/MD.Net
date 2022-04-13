using NUnit.Framework;
using System.IO;
using System.Linq;

namespace MD.Net.Tests
{
    [TestFixture]
    public class WavHeaderTests
    {
        [Test]
        public void Read_WAV()
        {
            var info = default(WavHeader.WavInfo);
            using (var reader = Resources.Track_001)
            {
                var result = WavHeader.Read(reader, out info);
                Assert.AreEqual(53785552, info.FileSize);
                Assert.AreEqual(WavHeader.WAV_FORMAT_PCM, info.Format);
                Assert.AreEqual(2, info.ChannelCount);
                Assert.AreEqual(44100, info.SampleRate);
                Assert.AreEqual(176400, info.ByteRate);
                Assert.AreEqual(4, info.BlockAlign);
                Assert.AreEqual(16, info.BitsPerSample);
                Assert.AreEqual(53745552, info.DataSize);
            }
        }

        [Test]
        public void Read_ATRAC3_LP2()
        {
            var info = default(WavHeader.WavInfo);
            using (var reader = new MemoryStream(Resources.WAV_ATRAC3_LP2))
            {
                var result = WavHeader.Read(reader, out info);
                Assert.IsTrue(result);
                Assert.AreEqual(5039714, info.FileSize);
                Assert.AreEqual(WavHeader.WAV_FORMAT_ATRAC3, info.Format);
                Assert.AreEqual(2, info.ChannelCount);
                Assert.AreEqual(44100, info.SampleRate);
                Assert.AreEqual(16537, info.ByteRate);
                Assert.AreEqual(384, info.BlockAlign);
                Assert.AreEqual(0, info.BitsPerSample);
                Assert.AreEqual(5039616, info.DataSize);
                Assert.IsTrue(Enumerable.SequenceEqual(new byte[] { 14, 0, 1, 0, 68, 172, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 }, info.Data));
                Assert.AreEqual(2, info.Chunks.Length);
                Assert.AreEqual("fact", info.Chunks[0].Name);
                Assert.IsTrue(Enumerable.SequenceEqual(new byte[] { 150, 17, 205, 0 }, info.Chunks[0].Data));
                Assert.AreEqual("LIST", info.Chunks[1].Name);
                Assert.IsTrue(Enumerable.SequenceEqual(new byte[] { 73, 78, 70, 79, 73, 83, 70, 84, 14, 0, 0, 0, 76, 97, 118, 102, 53, 57, 46, 49, 54, 46, 49, 48, 48, 0 }, info.Chunks[1].Data));
            }
        }

        [Test]
        public void Read_ATRAC3_LP4()
        {
            var info = default(WavHeader.WavInfo);
            using (var reader = new MemoryStream(Resources.WAV_ATRAC3_LP4))
            {
                var result = WavHeader.Read(reader, out info);
                Assert.IsTrue(result);
                Assert.AreEqual(2519906, info.FileSize);
                Assert.AreEqual(WavHeader.WAV_FORMAT_ATRAC3, info.Format);
                Assert.AreEqual(2, info.ChannelCount);
                Assert.AreEqual(44100, info.SampleRate);
                Assert.AreEqual(8268, info.ByteRate);
                Assert.AreEqual(192, info.BlockAlign);
                Assert.AreEqual(0, info.BitsPerSample);
                Assert.AreEqual(2519808, info.DataSize);
                Assert.IsTrue(Enumerable.SequenceEqual(new byte[] { 14, 0, 1, 0, 68, 172, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0 }, info.Data));
                Assert.AreEqual(2, info.Chunks.Length);
                Assert.AreEqual("fact", info.Chunks[0].Name);
                Assert.IsTrue(Enumerable.SequenceEqual(new byte[] { 195, 20, 205, 0 }, info.Chunks[0].Data));
                Assert.AreEqual("LIST", info.Chunks[1].Name);
                Assert.IsTrue(Enumerable.SequenceEqual(new byte[] { 73, 78, 70, 79, 73, 83, 70, 84, 14, 0, 0, 0, 76, 97, 118, 102, 53, 57, 46, 49, 54, 46, 49, 48, 48, 0 }, info.Chunks[1].Data));
            }
        }

        [Test]
        public void Write_ATRAC3_LP2()
        {
            var info = default(WavHeader.WavInfo);
            info.FileSize = 5039714;
            info.Format = WavHeader.WAV_FORMAT_ATRAC3;
            info.ChannelCount = 2;
            info.SampleRate = 44100;
            info.ByteRate = 16537;
            info.BlockAlign = 384;
            info.BitsPerSample = 0;
            info.Data = new byte[] { 14, 0, 1, 0, 68, 172, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 };
            info.Chunks = new[]
            {
                new WavHeader.WaveChunk()
                {
                    Name = "fact",
                    Data = new byte[] { 150, 17, 205, 0 }
                },
                new WavHeader.WaveChunk()
                {
                    Name = "LIST",
                    Data = new byte[] { 73, 78, 70, 79, 73, 83, 70, 84, 14, 0, 0, 0, 76, 97, 118, 102, 53, 57, 46, 49, 54, 46, 49, 48, 48, 0 }
                }
            };
            using (var writer = new MemoryStream())
            {
                var result = WavHeader.Write(writer, info);
                Assert.IsTrue(result);
                var expected = Resources.WAV_ATRAC3_LP2;
                var actual = writer.ToArray();
                Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
            }
        }

        [Test]
        public void Write_ATRAC3_LP4()
        {
            var info = default(WavHeader.WavInfo);
            info.FileSize = 2519906;
            info.Format = WavHeader.WAV_FORMAT_ATRAC3;
            info.ChannelCount = 2;
            info.SampleRate = 44100;
            info.ByteRate = 8268;
            info.BlockAlign = 192;
            info.BitsPerSample = 0;
            info.Data = new byte[] { 14, 0, 1, 0, 68, 172, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0 };
            info.Chunks = new[]
            {
                new WavHeader.WaveChunk()
                {
                    Name = "fact",
                    Data = new byte[] { 195, 20, 205, 0 }
                },
                new WavHeader.WaveChunk()
                {
                    Name = "LIST",
                    Data = new byte[] { 73, 78, 70, 79, 73, 83, 70, 84, 14, 0, 0, 0, 76, 97, 118, 102, 53, 57, 46, 49, 54, 46, 49, 48, 48, 0 }
                }
            };
            using (var writer = new MemoryStream())
            {
                var result = WavHeader.Write(writer, info);
                Assert.IsTrue(result);
                var expected = Resources.WAV_ATRAC3_LP4;
                var actual = writer.ToArray();
                Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
            }
        }
    }
}
