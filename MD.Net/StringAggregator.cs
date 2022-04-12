using System;
using System.Text;

namespace MD.Net
{
    public class StringAggregator : Aggregator<string>
    {
        private StringAggregator()
        {
            this.Builder = new StringBuilder();
        }

        public StringAggregator(string delimiter) : this()
        {
            this.Delimiter = delimiter;
        }

        public StringBuilder Builder { get; private set; }

        public string Delimiter { get; private set; }

        public override void Append(string value)
        {
            if (this.Builder.Length > 0)
            {
                this.Builder.Append(this.Delimiter);
            }
            this.Builder.Append(value);
        }

        public override string Aggregate()
        {
            return this.Builder.ToString();
        }

        public static IAggregator<string> NewLine
        {
            get
            {
                return new StringAggregator(Environment.NewLine);
            }
        }
    }
}
