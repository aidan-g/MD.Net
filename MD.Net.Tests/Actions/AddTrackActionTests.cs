using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Diagnostics;

namespace MD.Net.Tests
{
    [TestFixture]
    public class AddTrackActionTests
    {
        [TestCase(@"C:\My Music\Test.wav")]
        public void Apply(string location)
        {
            var track = new Track(0, Protection.None, Compression.None, TimeSpan.Zero, string.Empty)
            {
                Location = location
            };
            var toolManager = MockRepository.GenerateStrictMock<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI, string.Format("{0} \"{1}\" {2}", Constants.NETMDCLI_SEND, track.Location, Constants.NETMDCLI_VERBOSE)));
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, Arg<Action<string>>.Is.Anything, Arg<Action<string>>.Is.Anything)).WhenCalled(invocation =>
            {
                Utility.EmitLines(Resources.Send_SP, invocation.Arguments[1] as Action<string>);
            }).Return(0);
            var status = MockRepository.GenerateStrictMock<IStatus>();
            for (var a = 0; a <= 100; a++)
            {
                status.Expect(s => s.Update(Arg<string>.Is.Anything, Arg<int>.Is.Equal(a), Arg<int>.Is.Equal(100), Arg<StatusType>.Is.Equal(StatusType.Transfer))).Repeat.Once();
            }
            var action = new AddTrackAction(Device.None, Disc.None, Disc.None, track);
            action.Prepare(toolManager, Status.Ignore);
            action.Apply(toolManager, status);
            action.Commit();
            status.VerifyAllExpectations();
        }
    }
}
