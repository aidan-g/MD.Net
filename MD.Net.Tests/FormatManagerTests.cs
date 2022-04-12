using NUnit.Framework;
using Rhino.Mocks;
using System;
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
            toolManager.Expect(tm => tm.Start(Tools.ATRACDENC, string.Format("{0} {1} {2} \"{3}\" {4} \"{5}\" {6} {7}", Constants.ATRACDENC_ENCODE, Constants.ATRACDENC_ATRAC3, Constants.ATRACDENC_INPUT, inputFileName, Constants.ATRACDENC_OUTPUT, outputFileName, Constants.ATRACDENC_BITRATE, bitrate)));
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, Arg<Action<string>>.Is.Anything, Arg<Action<string>>.Is.Anything)).WhenCalled(invocation =>
            {
                Utility.EmitLines(Resources.Convert_Atrac, invocation.Arguments[1] as Action<string>);
            }).Return(0);
            var formatManager = new FormatManager(toolManager);
            var status = MockRepository.GenerateStrictMock<IStatus>();
            for (var a = 0; a <= 100; a++)
            {
                status.Expect(s => s.Update(Arg<string>.Is.Anything, Arg<int>.Is.Equal(a), Arg<int>.Is.Equal(100), Arg<StatusType>.Is.Equal(StatusType.Encode))).Repeat.Once();
            }
            Assert.AreEqual(outputFileName, formatManager.Convert(inputFileName, compression, status));
            status.VerifyAllExpectations();
        }
    }
}
