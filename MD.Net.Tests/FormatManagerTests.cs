using NUnit.Framework;
using Rhino.Mocks;
using System.Diagnostics;

namespace MD.Net.Tests
{
    [TestFixture]
    public class FormatManagerTests
    {
        [TestCase(@"C:\My Music\Test.wav", @"C:\My Music\Test.at3", Compression.LP2, "128")]
        [TestCase(@"C:\My Music\Test.wav", @"C:\My Music\Test.at3", Compression.LP4, "64")]
        public void Convert(string inputFileName, string outputFileName, Compression compression, string bitrate)
        {
            var toolManager = MockRepository.GenerateStrictMock<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.ATRACDENC, string.Format("{0} {1} {2} {3} {4} {5} {6} {7}", Constants.ATRACDENC_ENCODE, Constants.ATRACDENC_ATRAC3, Constants.ATRACDENC_INPUT, inputFileName, Constants.ATRACDENC_OUTPUT, outputFileName, Constants.ATRACDENC_BITRATE, bitrate)));
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(string.Empty).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0);
            var formatManager = new FormatManager(toolManager);
            Assert.AreEqual(outputFileName, formatManager.Convert(inputFileName, compression, Status.Ignore));
        }
    }
}
