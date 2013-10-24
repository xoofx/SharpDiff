using System.Collections.Generic;
using System.Linq;

namespace SharpDiff
{
    internal struct Diff3<T>
    {
        private readonly IList<T> baseList;
        private readonly IList<T> modified1List;
        private readonly IList<T> modified2List;
        private readonly IEqualityComparer<T> comparer;

        public Diff3(IList<T> baseList, IList<T> modified1List, IList<T> modified2List, IEqualityComparer<T> comparer)
        {
            this.baseList = baseList;
            this.modified1List = modified1List;
            this.modified2List = modified2List;
            this.comparer = comparer;
        }

        public IEnumerable<Diff3Change> Generate()
        {
            var diffBaseModified1 = ConvertDiffChangeWithNoRange(Diff2.Compare(baseList, modified1List, comparer)).ToList();
            var diffBaseModified2 = ConvertDiffChangeWithNoRange(Diff2.Compare(baseList, modified2List, comparer)).ToList();

            Diff3Change currentChunk = null;
            int index1 = 0;
            int index2 = 0;
            int baseIndex1 = 0;
            int baseIndex2 = 0;
            int diffIndex1 = 0;
            int diffIndex2 = 0;
            while (diffIndex1 < diffBaseModified1.Count && diffIndex2 < diffBaseModified2.Count)
            {
                var diff1 = diffBaseModified1[diffIndex1];
                var diff2 = diffBaseModified2[diffIndex2];

                // If element1 and element2 are both equals to the base element
                if (diff1.Equal && diff2.Equal && baseIndex1 == baseIndex2 && comparer.Equals(modified1List[index1], modified2List[index2]))
                {
                    if (currentChunk == null || currentChunk.Match == null)
                    {
                        if (currentChunk != null)
                            yield return currentChunk;
                        currentChunk = new Diff3Change();
                    }

                    currentChunk.AddMatch(new Range(baseIndex1, baseIndex1));

                    baseIndex1 += diff1.Length1;
                    index1 += diff1.Length2;
                    baseIndex2 += diff2.Length1;
                    index2 += diff2.Length2;
                    diffIndex1++;
                    diffIndex2++;
                }
                else
                {
                    if (currentChunk == null || currentChunk.Match != null)
                    {
                        if (currentChunk != null)
                            yield return currentChunk;
                        currentChunk = new Diff3Change();
                    }

                    if (!diff1.Equal || baseIndex2 > baseIndex1)
                    {
                        if (diff1.Length2 > 0)
                            currentChunk.AddDiff1(new Range(index1, index1 + diff1.Length2 - 1));
                        
                        if (diff1.Length1 > 0)
                            currentChunk.AddDiffBase(new Range(baseIndex1, baseIndex1 + diff1.Length1 - 1));

                        baseIndex1 += diff1.Length1;
                        index1 += diff1.Length2;
                        diffIndex1++;
                    }

                    if (!diff2.Equal || baseIndex1 > baseIndex2)
                    {
                        if (diff2.Length2 > 0)
                            currentChunk.AddDiff2(new Range(index2, index2 + diff2.Length2 - 1));

                        if (diff2.Length1 > 0)
                            currentChunk.AddDiffBase(new Range(baseIndex2, baseIndex2 + diff2.Length1 - 1));
                        
                        baseIndex2 += diff2.Length1;
                        index2 += diff2.Length2;
                        diffIndex2++;
                    }
                }
            }

            // Remaining chunks if any for diff1
            for (; diffIndex1 < diffBaseModified1.Count; diffIndex1++)
            {
                if (currentChunk == null || currentChunk.Match != null)
                {
                    if (currentChunk != null)
                        yield return currentChunk;
                    currentChunk = new Diff3Change();
                }

                var diff1 = diffBaseModified1[diffIndex1];
                if (diff1.Length2 > 0)
                    currentChunk.AddDiff1(new Range(index1, index1 + diff1.Length2 - 1));
                if (diff1.Length1 > 0)
                    currentChunk.AddDiffBase(new Range(baseIndex1, baseIndex1 + diff1.Length1 - 1));
                index1 += diff1.Length2;
            }

            // Remaining chunks if any for diff2
            for (; diffIndex2 < diffBaseModified2.Count; diffIndex2++)
            {
                if (currentChunk == null || currentChunk.Match != null)
                {
                    if (currentChunk != null)
                        yield return currentChunk;
                    currentChunk = new Diff3Change();
                }

                var diff2 = diffBaseModified2[diffIndex2];
                if (diff2.Length2 > 0)
                    currentChunk.AddDiff2(new Range(index2, index2 + diff2.Length2 - 1));
                if (diff2.Length1 > 0)
                    currentChunk.AddDiffBase(new Range(baseIndex2, baseIndex2 + diff2.Length1 - 1));
                index2 += diff2.Length2;
            }

            if (currentChunk != null)
            {
                yield return currentChunk;
            }
        }

        private IEnumerable<Diff2Change> ConvertDiffChangeWithNoRange(IEnumerable<Diff2Change> changes)
        {
            foreach (var diffChange in changes)
            {
                if (diffChange.Equal)
                {
                    for (int i = 0; i < diffChange.Length1; i++)
                    {
                        yield return new Diff2Change(diffChange.Equal, 1, 1);
                    }
                }
                else
                {
                    yield return diffChange;
                }
            }
        }
    }
}