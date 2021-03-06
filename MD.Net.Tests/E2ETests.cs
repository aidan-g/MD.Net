using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace MD.Net.Tests
{
    [TestFixture]
    [Explicit]
    public class E2ETests
    {
        public IStatus Status { get; private set; }

        [SetUp]
        public void SetUp()
        {
            this.Status = new Status();
            this.Status.Updated += this.OnUpdated;
        }

        [TearDown]
        public void TearDown()
        {
            //Nothing to do.
        }

        [Test]
        public void Test001()
        {
            var device = DeviceManager.Default.GetDevices().SingleOrDefault();
            var currentDisc = DiscManager.Default.GetDisc(device);
            var updatedDisc = currentDisc.Clone();

            updatedDisc.Title = string.Empty;
            updatedDisc.Tracks.Clear();

            var actions = ActionBuilder.Default.GetActions(device, currentDisc, updatedDisc);

            //Change title, remove existing tracks.
            Assert.AreEqual(currentDisc.Tracks.Count + 1, actions.Count);

            var result = DiscManager.Default.ApplyActions(device, actions, this.Status, true);
            Assert.AreEqual(ResultStatus.Success, result.Status);

            currentDisc = DiscManager.Default.GetDisc(device);
            Assert.AreEqual(Constants.UNTITLED, currentDisc.Title);
            Assert.AreEqual(0, currentDisc.Tracks.Count);
        }

        [TestCase(Compression.None)]
        [TestCase(Compression.LP2)]
        [TestCase(Compression.LP4)]
        public void Test002(Compression compression)
        {
            var device = DeviceManager.Default.GetDevices().SingleOrDefault();
            var currentDisc = DiscManager.Default.GetDisc(device);
            var updatedDisc = currentDisc.Clone();
            var title = "MD.Net.Tests - " + Math.Abs(DateTime.Now.Ticks);

            updatedDisc.Title = title;
            updatedDisc.Tracks.Clear();
            foreach (var name in new[] { "Track_001", "Track_002", "Track_003" })
            {
                var fileName = Path.Combine(Path.GetTempPath(), name + ".wav");
                using (var reader = Resources.ResourceManager.GetStream(name))
                {
                    using (var writer = File.Create(fileName))
                    {
                        reader.CopyTo(writer);
                    }
                }
                var track = updatedDisc.Tracks.Add(fileName, compression);
                track.Name = "MD.Net.Tests - " + updatedDisc.Tracks.Count;
            }

            var actions = ActionBuilder.Default.GetActions(device, currentDisc, updatedDisc);

            //Change title, remove existing tracks, add 3 new tracks.
            Assert.AreEqual(currentDisc.Tracks.Count + 4, actions.Count);

            var result = DiscManager.Default.ApplyActions(device, actions, this.Status, true);
            Assert.AreEqual(ResultStatus.Success, result.Status);

            currentDisc = DiscManager.Default.GetDisc(device);
            Assert.AreEqual(title, currentDisc.Title);
            Assert.AreEqual(3, currentDisc.Tracks.Count);
            Assert.AreEqual("MD.Net.Tests - 1", currentDisc.Tracks[0].Name);
            Assert.AreEqual(compression, currentDisc.Tracks[0].Compression);
            Assert.AreEqual("MD.Net.Tests - 2", currentDisc.Tracks[1].Name);
            Assert.AreEqual(compression, currentDisc.Tracks[1].Compression);
            Assert.AreEqual("MD.Net.Tests - 3", currentDisc.Tracks[2].Name);
            Assert.AreEqual(compression, currentDisc.Tracks[2].Compression);
        }

        protected virtual void OnUpdated(object sender, StatusEventArgs e)
        {
            TestContext.WriteLine("{0}: {1}/{2} - {3}", Enum.GetName(typeof(StatusType), e.Type), e.Position, e.Count, e.Message);
        }
    }
}
