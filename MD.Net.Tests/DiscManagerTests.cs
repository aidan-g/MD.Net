using MD.Net.Resources;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Diagnostics;
using System.Linq;

namespace MD.Net.Tests
{
    [TestFixture]
    public class DiscManagerTests
    {
        [Test]
        public void NoDisk()
        {
            var toolManager = MockRepository.GenerateStub<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI)).Return(null);
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(Resources.Sony_MDS_JE780___No_Disk).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0);
            var discManager = new DiscManager(toolManager);
            var device = new Device("Sony MDS-JE780/JB980");
            var disc = discManager.GetDisc(device);
            Assert.IsNotNull(disc);
            Assert.AreEqual("<Untitled>", disc.Title);
            Assert.AreEqual(new TimeSpan(1143, 67, 0), disc.RecordedTime);
            Assert.AreEqual(new TimeSpan(80, 0, 0), disc.TotalTime);
            Assert.AreEqual(new TimeSpan(0, 11, 0), disc.AvailableTime);
        }

        [Test]
        public void Evanescence()
        {
            var toolManager = MockRepository.GenerateStub<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI)).Return(null);
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(Resources.Sony_MDS_JE780___Evanescence).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0);
            var discManager = new DiscManager(toolManager);
            var device = new Device("Sony MDS-JE780/JB980");
            var disc = discManager.GetDisc(device);
            Assert.IsNotNull(disc);
            Assert.AreEqual("Evanescence", disc.Title);
            Assert.AreEqual(new TimeSpan(0, 0, 47, 20, 104), disc.RecordedTime);
            Assert.AreEqual(new TimeSpan(0, 1, 14, 59, 487), disc.TotalTime);
            Assert.AreEqual(new TimeSpan(0, 0, 27, 26, 951), disc.AvailableTime);
            Assert.AreEqual(12, disc.Tracks.Count);
        }

        [TestCase("Original Title", "New Title")]
        public void UpdateDiscTitle(string currentTitle, string updatedTitle)
        {
            var toolManager = MockRepository.GenerateStrictMock<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI, string.Format("{0} {1}", Constants.NETMDCLI_SETTITLE, updatedTitle))).Return(null);
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(string.Empty).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0);
            var device = new Device("Sony MDS-JE780/JB980");
            var currentDisc = new Disc(currentTitle, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, Tracks.None);
            var updatedDisc = new Disc(currentDisc)
            {
                Title = updatedTitle
            };
            var actions = new Actions(
                device,
                currentDisc,
                updatedDisc,
                new[]
                {
                    new UpdateDiscTitleAction(device,currentDisc,updatedDisc)
                }
            );
            var discManager = new DiscManager(toolManager);
            var result = discManager.ApplyActions(device, actions, Status.Ignore, false);
            Assert.AreEqual(ResultStatus.Success, result.Status);
        }

        [TestCase("Original Name", "New Name")]
        public void UpdateTrackName(string currentName, string updatedName)
        {
            var toolManager = MockRepository.GenerateStrictMock<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI, string.Format("{0} {1} {2}", Constants.NETMDCLI_RENAME, 1, updatedName))).Return(null);
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(string.Empty).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0);
            var device = new Device("Sony MDS-JE780/JB980");
            var track1 = new Track(0, Protection.None, Compression.None, TimeSpan.Zero, string.Empty);
            var track2 = new Track(1, Protection.None, Compression.None, TimeSpan.Zero, currentName);
            var track3 = new Track(2, Protection.None, Compression.None, TimeSpan.Zero, string.Empty);
            var currentDisc = new Disc(string.Empty, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, new Tracks(new[] { track1, track2, track3 }));
            var updatedDisc = new Disc(currentDisc);
            updatedDisc.Tracks[1].Name = updatedName;
            var actions = new Actions(
                device,
                currentDisc,
                updatedDisc,
                new[]
                {
                    new UpdateTrackNameAction(device, currentDisc.Tracks[1], updatedDisc.Tracks[1])
                }
            );
            var discManager = new DiscManager(toolManager);
            var result = discManager.ApplyActions(device, actions, Status.Ignore, false);
            Assert.AreEqual(ResultStatus.Success, result.Status);
        }

        [TestCase(@"C:\My Music\Test.wav", "This is a test.")]
        public void AddTrack(string location, string name)
        {
            var toolManager = MockRepository.GenerateStrictMock<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI, string.Format("{0} {1} {2}", Constants.NETMDCLI_SEND, location, name))).Return(null);
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(string.Empty).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0);
            var device = new Device("Sony MDS-JE780/JB980");
            var track1 = new Track(0, Protection.None, Compression.None, TimeSpan.Zero, string.Empty);
            var track2 = new Track(1, Protection.None, Compression.None, TimeSpan.Zero, string.Empty);
            var track3 = new Track(2, Protection.None, Compression.None, TimeSpan.Zero, string.Empty);
            var currentDisc = new Disc(string.Empty, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, new Tracks(new[] { track1, track2, track3 }));
            var updatedDisc = new Disc(currentDisc);
            var track = updatedDisc.Tracks.Add();
            track.Location = location;
            track.Name = name;
            var actions = new Actions(
                device,
                currentDisc,
                updatedDisc,
                new[]
                {
                    new AddTrackAction(device, track)
                }
            );
            var discManager = new DiscManager(toolManager);
            var result = discManager.ApplyActions(device, actions, Status.Ignore, false);
            Assert.AreEqual(ResultStatus.Success, result.Status);
        }

        [Test]
        public void RemoveTrack()
        {
            var toolManager = MockRepository.GenerateStrictMock<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI, string.Format("{0} {1}", Constants.NETMDCLI_DELETE, 1))).Return(null);
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(string.Empty).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0);
            var device = new Device("Sony MDS-JE780/JB980");
            var track1 = new Track(0, Protection.None, Compression.None, TimeSpan.Zero, string.Empty);
            var track2 = new Track(1, Protection.None, Compression.None, TimeSpan.Zero, string.Empty);
            var track3 = new Track(2, Protection.None, Compression.None, TimeSpan.Zero, string.Empty);
            var currentDisc = new Disc(string.Empty, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, new Tracks(new[] { track1, track2, track3 }));
            var updatedDisc = new Disc(currentDisc);
            var track = updatedDisc.Tracks[1];
            updatedDisc.Tracks.Remove(track);
            var actions = new Actions(
                device,
                currentDisc,
                updatedDisc,
                new[]
                {
                    new RemoveTrackAction(device, track)
                }
            );
            var discManager = new DiscManager(toolManager);
            var result = discManager.ApplyActions(device, actions, Status.Ignore, false);
            Assert.AreEqual(ResultStatus.Success, result.Status);
        }

        [TestCase("\"title\":\"Evanescence\"", "\"title\":\"Other\"")]
        [TestCase("\"name\":\"What You Want - Evanescence\"", "\"name\":\"Other\"")]
        public void Error_DiscWasModified(string oldValue, string newValue)
        {
            var toolManager = MockRepository.GenerateStrictMock<IToolManager>();
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI)).Return(null).Repeat.Once();
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(Resources.Sony_MDS_JE780___Evanescence).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0).Repeat.Once();
            var discManager = new DiscManager(toolManager);
            var device = new Device("Sony MDS-JE780/JB980");
            var disc = discManager.GetDisc(device);
            toolManager.Expect(tm => tm.Start(Tools.NETMDCLI)).Return(null).Repeat.Once();
            toolManager.Expect(tm => tm.Exec(Arg<Process>.Is.Anything, out Arg<string>.Out(Resources.Sony_MDS_JE780___Evanescence.Replace(oldValue, newValue)).Dummy, out Arg<string>.Out(string.Empty).Dummy)).Return(0).Repeat.Once();
            var actions = new Actions(
                device,
                disc,
                Disc.None,
                Actions.None
            );
            var result = discManager.ApplyActions(device, actions, Status.Ignore, true);
            Assert.AreEqual(ResultStatus.Failure, result.Status);
            Assert.AreEqual(Strings.Error_DiscWasModified, result.Message);
        }
    }
}
