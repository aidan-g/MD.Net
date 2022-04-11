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

        }

        [TestCase(Compression.None)]
        [TestCase(Compression.LP2)]
        [TestCase(Compression.LP4)]
        public void Test002(Compression compression)
        {
            var toolManager = new ToolManager();
            var deviceManager = new DeviceManager(toolManager);
            var discManager = new DiscManager(toolManager);

            var device = deviceManager.GetDevices().SingleOrDefault();
            var currentDisc = discManager.GetDisc(device);
            var updatedDisc = currentDisc.Clone();

            updatedDisc.Title = "MD.Net.Tests - " + Math.Abs(DateTime.Now.Ticks);
            updatedDisc.Tracks.Clear();
            foreach (var input in new[] { Resources.Track_001, Resources.Track_002, Resources.Track_003 })
            {
                var fileName = Path.GetTempFileName();
                using (var writer = File.Create(fileName))
                {
                    input.CopyTo(writer);
                }
                var track = updatedDisc.Tracks.Add();
                track.Name = "MD.Net.Tests - " + updatedDisc.Tracks.Count;
                track.Location = fileName;
                track.Compression = compression;
            }

            var actionBuilder = new ActionBuilder();
            var actions = actionBuilder.GetActions(device, currentDisc, updatedDisc);

            //Change title, remove existing tracks, add 3 new tracks.
            Assert.AreEqual(currentDisc.Tracks.Count + 4, actions.Count);

            var result = discManager.ApplyActions(device, actions, this.Status, true);
            Assert.AreEqual(ResultStatus.Success, result.Status);
        }

        protected virtual void OnUpdated(object sender, StatusEventArgs e)
        {
            TestContext.WriteLine("{0}: {1}/{2} - {3}", Enum.GetName(typeof(StatusType), e.Type), e.Position, e.Count, e.Message);
        }
    }
}
