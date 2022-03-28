using NUnit.Framework;
using Rhino.Mocks;
using System.Diagnostics;

namespace MD.Net.Tests
{
    [TestFixture]
    public class PlaybackManagerTests
    {
        [Test]
        public void Play()
        {
            var toolManager = MockRepository.GenerateStrictMock<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI, string.Format("{0} {1}", Constants.NETMDCLI_PLAY, Track.None.Position))).Return(null);
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(string.Empty).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0);
            var playbackManager = new PlaybackManager(toolManager);
            playbackManager.Play(Device.None, Track.None);
        }

        [Test]
        public void Stop()
        {
            var toolManager = MockRepository.GenerateStrictMock<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI, Constants.NETMDCLI_STOP)).Return(null);
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(string.Empty).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0);
            var playbackManager = new PlaybackManager(toolManager);
            playbackManager.Stop(Device.None);
        }
    }
}
