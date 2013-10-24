using System;

namespace SharpDiff
{
    public class Range
    {
        public Range(int from, int to)
        {
            From = @from;
            To = to;
        }

        public int From { get; private set; }

        public int To { get; internal set; }

        public override string ToString()
        {
            return String.Format("[{0}-{1}]", From, To);
        }
    }
}