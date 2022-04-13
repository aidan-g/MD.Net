using NUnit.Framework;
using System.IO;
using System.Linq;

namespace MD.Net.Tests
{
    [TestFixture]
    public class OMAHeaderTests
    {
        [Test]
        public void Read_ATRAC3_LP2()
        {
            var info = default(OMAHeader.OMAInfo);
            using (var reader = new MemoryStream(Resources.OMA_ATRAC_LP2))
            {
                var result = OMAHeader.Read(reader, out info);
                Assert.IsTrue(result);
                Assert.AreEqual(OMAHeader.OMA_CODEC_ATRAC3, info.Codec);
                Assert.AreEqual(384, info.Framesize);
                Assert.AreEqual(44100, info.SampleRate);
                Assert.AreEqual(OMAHeader.OMA_STEREO, info.ChannelFormat);
            }
        }

        [Test]
        public void Read_ATRAC3_LP4()
        {
            var info = default(OMAHeader.OMAInfo);
            using (var reader = new MemoryStream(Resources.OMA_ATRAC_LP4))
            {
                var result = OMAHeader.Read(reader, out info);
                Assert.IsTrue(result);
                Assert.AreEqual(OMAHeader.OMA_CODEC_ATRAC3, info.Codec);
                Assert.AreEqual(192, info.Framesize);
                Assert.AreEqual(44100, info.SampleRate);
                Assert.AreEqual(OMAHeader.OMA_JOINT_STEREO, info.ChannelFormat);
            }
        }

        [Test]
        [Explicit("Not yet implemented.")]
        public void Write_ATRAC3_LP2()
        {
            var info = default(OMAHeader.OMAInfo);
            info.Codec = OMAHeader.OMA_CODEC_ATRAC3;
            info.Framesize = 192;
            info.SampleRate = 44100;
            info.ChannelFormat = OMAHeader.OMA_JOINT_STEREO;
            using (var writer = new MemoryStream())
            {
                var result = OMAHeader.Write(writer, info);
                Assert.IsTrue(result);
                var expected = Resources.OMA_ATRAC_LP2;
                var actual = writer.ToArray();
                Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
            }
        }

        [Test]
        [Explicit("Not yet implemented.")]
        public void Write_ATRAC3_LP4()
        {
            var info = default(OMAHeader.OMAInfo);
            info.Codec = OMAHeader.OMA_CODEC_ATRAC3;
            info.Framesize = 192;
            info.SampleRate = 44100;
            info.ChannelFormat = OMAHeader.OMA_JOINT_STEREO;
            using (var writer = new MemoryStream())
            {
                var result = OMAHeader.Write(writer, info);
                Assert.IsTrue(result);
                var expected = Resources.OMA_ATRAC_LP4;
                var actual = writer.ToArray();
                Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
            }
        }
    }
}
