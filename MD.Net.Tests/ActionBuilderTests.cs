using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Linq;

namespace MD.Net.Tests
{
    [TestFixture]
    public class ActionBuilderTests
    {
        [TestCase("Original Title", "New Title")]
        public void UpdateDiscTitle(string currentTitle, string updatedTitle)
        {
            var formatManager = MockRepository.GenerateStub<IFormatManager>();
            var discActionBuilder = new ActionBuilder(formatManager);
            var device = default(IDevice);
            var currentDisc = new Disc(currentTitle, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, Tracks.None);
            var updatedDisc = new Disc(currentDisc)
            {
                Title = updatedTitle
            };
            var actions = discActionBuilder.GetActions(device, currentDisc, updatedDisc);
            Assert.AreEqual(1, actions.Count);
            var action = actions.First() as UpdateDiscTitleAction;
            Assert.IsNotNull(action);
        }

        [TestCase("Original Name", "New Name")]
        public void UpdateTrackName(string currentName, string updatedName)
        {
            var formatManager = MockRepository.GenerateStub<IFormatManager>();
            var discActionBuilder = new ActionBuilder(formatManager);
            var device = default(IDevice);
            var track1 = new Track(0, Protection.None, Compression.None, TimeSpan.Zero, string.Empty);
            var track2 = new Track(1, Protection.None, Compression.None, TimeSpan.Zero, currentName);
            var track3 = new Track(2, Protection.None, Compression.None, TimeSpan.Zero, string.Empty);
            var currentDisc = new Disc(string.Empty, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, new Tracks(new[] { track1, track2, track3 }));
            var updatedDisc = new Disc(currentDisc);
            updatedDisc.Tracks[1].Name = updatedName;
            var actions = discActionBuilder.GetActions(device, currentDisc, updatedDisc);
            Assert.AreEqual(1, actions.Count);
            var action = actions.First() as UpdateTrackNameAction;
            Assert.IsNotNull(action);
            Assert.IsTrue(object.ReferenceEquals(currentDisc.Tracks[1], action.CurrentTrack));
            Assert.IsTrue(object.ReferenceEquals(updatedDisc.Tracks[1], action.UpdatedTrack));
        }

        [TestCase(@"C:\My Music\Test.wav", Compression.None, "This is a test.")]
        public void AddTrack(string location, Compression compression, string name)
        {
            var formatManager = MockRepository.GenerateStub<IFormatManager>();
            var discActionBuilder = new ActionBuilder(formatManager);
            var device = default(IDevice);
            var track1 = new Track(0, Protection.None, Compression.None, TimeSpan.Zero, string.Empty);
            var track2 = new Track(1, Protection.None, Compression.None, TimeSpan.Zero, string.Empty);
            var track3 = new Track(2, Protection.None, Compression.None, TimeSpan.Zero, string.Empty);
            var currentDisc = new Disc(string.Empty, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, new Tracks(new[] { track1, track2, track3 }));
            var updatedDisc = new Disc(currentDisc);
            var track = updatedDisc.Tracks.Add(location, compression);
            track.Name = name;
            var actions = discActionBuilder.GetActions(device, currentDisc, updatedDisc);
            Assert.AreEqual(1, actions.Count);
            var action = actions.First() as AddTrackAction;
            Assert.IsNotNull(action);
            Assert.IsTrue(object.ReferenceEquals(action.UpdatedTrack, track));
        }

        [Test]
        public void RemoveTrack()
        {
            var formatManager = MockRepository.GenerateStub<IFormatManager>();
            var discActionBuilder = new ActionBuilder(formatManager);
            var device = default(IDevice);
            var track1 = new Track(0, Protection.None, Compression.None, TimeSpan.Zero, string.Empty);
            var track2 = new Track(1, Protection.None, Compression.None, TimeSpan.Zero, string.Empty);
            var track3 = new Track(2, Protection.None, Compression.None, TimeSpan.Zero, string.Empty);
            var currentDisc = new Disc(string.Empty, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, new Tracks(new[] { track1, track2, track3 }));
            var updatedDisc = new Disc(currentDisc);
            var track = updatedDisc.Tracks[1];
            updatedDisc.Tracks.Remove(track);
            var actions = discActionBuilder.GetActions(device, currentDisc, updatedDisc);
            Assert.AreEqual(1, actions.Count);
            var action = actions.First() as RemoveTrackAction;
            Assert.IsNotNull(action);
            Assert.IsTrue(object.ReferenceEquals(action.CurrentTrack, currentDisc.Tracks[track.Position]));
        }
    }
}
