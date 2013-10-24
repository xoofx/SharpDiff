using System.Collections.Generic;

namespace SharpDiff
{
    public class RangeList : List<Range>
    {
        public void AddDiff(Range diff)
        {
            int lastIndex = Count - 1;

            if (lastIndex >= 0 && (diff.From - this[lastIndex].To) < 2)
            {
                this[lastIndex].To = diff.To;
            }
            else
            {
                this.Add(diff);
            }
        }
    }
}