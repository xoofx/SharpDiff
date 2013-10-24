using System;
using System.Text;

namespace SharpDiff
{
    public class Diff3Change
    {
        public Diff3Change()
        {
        }

        public RangeList Match { get; private set; }

        public RangeList UnMatchBase { get; private set; }

        public RangeList UnMatchFrom1 { get; private set; }

        public RangeList UnMatchFrom2 { get; private set; }

        public bool HasMatch
        {
            get
            {
                return Match != null;
            }
        }

        public bool CanMerge
        {
            get
            {
                return HasMatch || UnMatchFrom1 == null || UnMatchFrom2 == null;
            }
        }

        internal void AddMatch(Range diff)
        {
            if (Match == null) Match = new RangeList();
            Match.AddDiff(diff);
        }

        internal void AddDiff1(Range diff)
        {
            if (UnMatchFrom1 == null) UnMatchFrom1 = new RangeList();
            UnMatchFrom1.AddDiff(diff);
        }

        internal void AddDiff2(Range diff)
        {
            if (UnMatchFrom2 == null) UnMatchFrom2 = new RangeList();
            UnMatchFrom2.AddDiff(diff);
        }

        internal void AddDiffBase(Range diff)
        {
            if (UnMatchBase == null) UnMatchBase = new RangeList();
            UnMatchBase.AddDiff(diff);
        }

        public override string ToString()
        {
            if (Match != null)
            {
                return String.Format("Equal = ({0})", String.Join(",", Match));
            }

            var text = new StringBuilder();
            if (UnMatchFrom1 != null)
            {
                text.AppendFormat("[1] = ({0})", String.Join(",", UnMatchFrom1));
            }

            if (UnMatchBase != null)
            {
                if (text.Length > 0) text.AppendLine();
                text.AppendFormat("[0] = ({0})", String.Join(",", UnMatchBase));
            }

            if (UnMatchFrom2 != null)
            {
                if (text.Length > 0) text.AppendLine();
                text.AppendFormat("[2] = ({0})", String.Join(",", UnMatchFrom2));
            }

            return text.ToString();
        }
    }
}