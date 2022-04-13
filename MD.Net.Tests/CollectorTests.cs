using NUnit.Framework;
using System;

namespace MD.Net.Tests
{
    [TestFixture]
    public class CollectorTests
    {
        [Test]
        public void Collect()
        {
            var emitter = new Action<Action<string>>(handler =>
            {
                handler("A");
                handler("B");
                handler("C");
            });
            var value = default(Func<string>);
            emitter(Collector<string>.Collect(StringAggregator.NewLine, out value));
            Assert.AreEqual(string.Join(Environment.NewLine, "A", "B", "C"), value());
        }
    }
}
