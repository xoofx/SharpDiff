using System;

namespace SharpDiff
{
    public class Diff3Change
    {
        public Diff3Change()
        {
        }

        public Span Base;

        public Span From1;

        public Span From2;

        public bool? IsBaseEqualFrom1;

        public bool? IsBaseEqualFrom2;

        public Diff3ChangeType ChangeType
        {
            get;
            set;
        }

        public int EqualFlags;

        public bool CanMerge
        {
            get
            {
                return ChangeType != Diff3ChangeType.Conflict;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, Base = ({1}), [1] = {2}, [2] = {3}", ChangeType, Base, From1, From2);
        }
    }
}