using System;
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
        private List<T> subList1;
        private List<T> subList2;

        public Diff3(IList<T> baseList, IList<T> modified1List, IList<T> modified2List, IEqualityComparer<T> comparer)
        {
            this.baseList = baseList;
            this.modified1List = modified1List;
            this.modified2List = modified2List;
            this.comparer = comparer;
            subList1 = null;
            subList2 = null;
        }

        public List<Diff3Change> Generate()
        {
            var diffBaseModified1 = ConvertDiffChangeWithNoRange(Diff2.Compare(baseList, modified1List, comparer)).ToList();
            var diffBaseModified2 = ConvertDiffChangeWithNoRange(Diff2.Compare(baseList, modified2List, comparer)).ToList();

            var changes = new List<Diff3Change>();

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
                if (diff1.Equal && diff2.Equal && baseIndex1 == baseIndex2)
                {
                    AddChangeType(changes, Diff3ChangeType.Equal, new Span(baseIndex1, baseIndex2), new Span(index1, index1 + diff1.Length2 - 1), new Span(index2, index2 + diff2.Length2 - 1), true, true);

                    baseIndex1 += diff1.Length1;
                    index1 += diff1.Length2;
                    baseIndex2 += diff2.Length1;
                    index2 += diff2.Length2;
                    diffIndex1++;
                    diffIndex2++;
                }
                else
                {
                    var baseRange = Span.Invalid;
                    var from1Range = Span.Invalid;
                    var from2Range = Span.Invalid;

                    bool change1 = false;
                    bool change2 = false;

                    var diff1Equal = true;
                    var diff2Equal = true;

                    // Try to advance as much as possible on both sides until we reach a common point (baseIndex1 == baseIndex2)
                    // This will form a single conflict node
                    while (true)
                    {
                        if (diffIndex1 < diffBaseModified1.Count && !diff1.Equal || baseIndex2 > baseIndex1)
                        {
                            // Advance in list1
                            change1 = true;
                            diff1Equal &= diff1.Equal;

                            if (diff1.Length2 > 0)
                                from1Range = new Span(from1Range != Span.Invalid ? from1Range.From : index1, index1 + diff1.Length2 - 1);

                            if (diff1.Length1 > 0)
                                baseRange = new Span(baseRange != Span.Invalid ? baseRange.From : baseIndex1, baseIndex1 + diff1.Length1 - 1);

                            baseIndex1 += diff1.Length1;
                            index1 += diff1.Length2;

                            if (++diffIndex1 < diffBaseModified1.Count)
                                diff1 = diffBaseModified1[diffIndex1];
                        }
                        else if (diffIndex2 < diffBaseModified2.Count && !diff2.Equal || baseIndex1 > baseIndex2)
                        {
                            // Advance in list2
                            change2 = true;
                            diff2Equal &= diff2.Equal;

                            if (diff2.Length2 > 0)
                                from2Range = new Span(from2Range != Span.Invalid ? from2Range.From : index2, index2 + diff2.Length2 - 1);

                            if (diff2.Length1 > 0)
                                baseRange = new Span(baseRange != Span.Invalid ? baseRange.From : baseIndex2, baseIndex2 + diff2.Length1 - 1);

                            baseIndex2 += diff2.Length1;
                            index2 += diff2.Length2;

                            if (++diffIndex2 < diffBaseModified2.Count)
                                diff2 = diffBaseModified2[diffIndex2];
                        }
                        else
                        {
                            break;
                        }
                    }

                    AddChangeType(changes, Diff3ChangeType.Conflict, baseRange, from1Range, from2Range, change1 ? (bool?)diff1Equal : null, change2 ? (bool?)diff2Equal : null);
                }
            }

            // Not sure this could happen, but in case, output conflicts instead of merge if original base-v1 and base-v2 diffs were not entirely processed
            bool isInConflict = diffIndex1 < diffBaseModified1.Count && diffIndex2 < diffBaseModified2.Count;

            // Process remaining diffs from1
            while (diffIndex1 < diffBaseModified1.Count)
            {
                var diff1 = diffBaseModified1[diffIndex1];
                AddChangeType(changes, isInConflict ? Diff3ChangeType.Conflict : Diff3ChangeType.MergeFrom1, new Span(baseIndex1, baseIndex1), new Span(index1, index1 + diff1.Length2 - 1), Span.Invalid, null, null);

                baseIndex1 += diff1.Length1;
                index1 += diff1.Length2;
                diffIndex1++;
            }

            // Process remaining diffs from2
            while (diffIndex2 < diffBaseModified2.Count)
            {
                var diff2 = diffBaseModified2[diffIndex2];
                AddChangeType(changes, isInConflict ? Diff3ChangeType.Conflict : Diff3ChangeType.MergeFrom2, new Span(baseIndex2, baseIndex2), Span.Invalid, new Span(index2, index2 + diff2.Length2 - 1), null, null);

                baseIndex2 += diff2.Length1;
                index2 += diff2.Length2;
                diffIndex2++;
            }

            return FixPreviousConflictAsMerge(changes);
        }


        private void AddChangeType(List<Diff3Change> changes, Diff3ChangeType changeType, Span baseSpan, Span v1Index, Span v2Index, bool? baseEqualFrom1, bool? baseEqualFrom2)
        {
            bool addNew = changes.Count == 0;

            if (!addNew)
            {
                var previousChange = changes[changes.Count - 1];
                if (previousChange.ChangeType == changeType)
                {
                    bool keepPrevious = false;

                    if (previousChange.Base == baseSpan && (previousChange.From1 != Span.Invalid || previousChange.From2 != Span.Invalid))
                    {
                        if (previousChange.From1.CanMergeWith(v1Index) && (!previousChange.IsBaseEqualFrom1.HasValue || baseEqualFrom1 == previousChange.IsBaseEqualFrom1))
                        {
                            previousChange.From1 = Span.Merge(previousChange.From1, v1Index);
                            previousChange.IsBaseEqualFrom1 = baseEqualFrom1;
                            keepPrevious = true;
                        }

                        if (previousChange.From2.CanMergeWith(v2Index) && (!previousChange.IsBaseEqualFrom2.HasValue || baseEqualFrom2 == previousChange.IsBaseEqualFrom2))
                        {
                            previousChange.From2 = Span.Merge(previousChange.From2, v2Index);
                            previousChange.IsBaseEqualFrom2 = baseEqualFrom2;
                            keepPrevious = true;
                        }
                    }
                    else
                    {
                        if (previousChange.Base.CanMergeWith(baseSpan))
                        {
                            keepPrevious = true;
                        }

                        if (previousChange.From1.CanMergeWith(v1Index))
                        {
                            previousChange.From1 = Span.Merge(previousChange.From1, v1Index);
                            previousChange.IsBaseEqualFrom1 = null;
                            keepPrevious = true;
                        }

                        if (previousChange.From2.CanMergeWith(v2Index))
                        {
                            previousChange.From2 = Span.Merge(previousChange.From2, v2Index);
                            previousChange.IsBaseEqualFrom2 = null;
                            keepPrevious = true;
                        }
                    }

                    if (keepPrevious)
                    {
                        previousChange.Base = Span.Merge(previousChange.Base, baseSpan);
                    }
                    else
                    {
                        addNew = true;
                    }
                }
                else
                {
                    addNew = true;
                }
            }

            if (addNew)
            {
                var diff3 = new Diff3Change()
                {
                    Base = baseSpan,
                    From1 = v1Index,
                    From2 = v2Index,
                    ChangeType = changeType
                };
                if (baseEqualFrom1.HasValue)
                    diff3.IsBaseEqualFrom1 = baseEqualFrom1.Value;

                if (baseEqualFrom2.HasValue)
                    diff3.IsBaseEqualFrom2 = baseEqualFrom2.Value;


                changes.Add(diff3);
            }
        }

        private void AddChangeType(List<Diff3Change> changes, Diff3Change change)
        {
            AddChangeType(changes, change.ChangeType, change.Base, change.From1, change.From2, change.IsBaseEqualFrom1, change.IsBaseEqualFrom2);
        }

        private List<Diff3Change> FixPreviousConflictAsMerge(List<Diff3Change> changes)
        {
            List<Diff3Change> toChanges = null;

            for (int index = 0; index < changes.Count; index++)
            {
                var diff3 = changes[index];

                if (diff3.ChangeType != Diff3ChangeType.Conflict)
                {
                    if (toChanges != null)
                    {
                        AddChangeType(toChanges, diff3);
                    }
                    continue;
                }

                if (!diff3.Base.IsValid && diff3.From1.IsValid && !diff3.From2.IsValid)
                {
                    diff3.ChangeType = Diff3ChangeType.MergeFrom1;
                    if (toChanges != null)
                        AddChangeType(toChanges, diff3);
                }
                else if (!diff3.Base.IsValid && !diff3.From1.IsValid && diff3.From2.IsValid)
                {
                    diff3.ChangeType = Diff3ChangeType.MergeFrom2;
                    if (toChanges != null)
                        AddChangeType(toChanges, diff3);
                }
                else if (diff3.Base.IsValid && diff3.From1.IsValid && !diff3.From2.IsValid)
                {
                    if (IsRangeEqual(baseList, diff3.Base, modified1List, diff3.From1))
                        diff3.ChangeType = Diff3ChangeType.MergeFrom2;

                    if (toChanges != null)
                        AddChangeType(toChanges, diff3);
                }
                else if (diff3.Base.IsValid && diff3.From2.IsValid && !diff3.From1.IsValid)
                {
                    if (IsRangeEqual(baseList, diff3.Base, modified2List, diff3.From2))
                        diff3.ChangeType = Diff3ChangeType.MergeFrom1;

                    if (toChanges != null)
                        AddChangeType(toChanges, diff3);
                }
                else if (diff3.From1.IsValid && diff3.From2.IsValid)
                {
                    // If there is no base, we will try to merge 1 and 2 together directly
                    if (!diff3.Base.IsValid)
                    {
                        if (toChanges == null)
                        {
                            toChanges = new List<Diff3Change>(changes.Count);
                            for (int i = 0; i < index; i++)
                            {
                                toChanges.Add(changes[i]);
                            }
                        }

                        var from1Range = diff3.From1;
                        var from2Range = diff3.From2;

                        if (subList1 == null)
                        {
                            subList1 = new List<T>(Math.Max(10, from1Range.Length));
                        }
                        if (subList2 == null)
                        {
                            subList2 = new List<T>(Math.Max(10, from2Range.Length));
                        }
                        subList1.Clear();
                        subList2.Clear();
                        for (int i = from1Range.From; i <= from1Range.To; i++)
                        {
                            subList1.Add(modified1List[i]);
                        }
                        for (int i = from2Range.From; i <= from2Range.To; i++)
                        {
                            subList2.Add(modified2List[i]);
                        }

                        var diffModified1Modified2 = Diff2.Compare(subList1, subList2, comparer).ToList();
                        int diff1Index = from1Range.From;
                        int diff2Index = from2Range.From;

                        // Try to evaluate the base index to give an indication where the merge would be inserted in base
                        var baseIndex = index > 0 ? changes[index - 1].Base.IsValid ? changes[index - 1].Base.To + 1 : 0 : 0;
                        var spanBase = new Span(baseIndex, baseIndex);
                        foreach (var diff2Change in diffModified1Modified2)
                        {
                            if (diff2Change.Equal)
                            {
                                AddChangeType(toChanges, Diff3ChangeType.MergeFrom1And2, spanBase, new Span(diff1Index, diff1Index + diff2Change.Length1 - 1), new Span(diff2Index, diff2Index + diff2Change.Length2 - 1), null, null);
                            }
                            else if (diff2Change.Length1 == 0)
                            {
                                AddChangeType(toChanges, Diff3ChangeType.MergeFrom2, spanBase, Span.Invalid, new Span(diff2Index, diff2Index + diff2Change.Length2 - 1), null, null);
                            }
                            else if (diff2Change.Length2 == 0)
                            {
                                AddChangeType(toChanges, Diff3ChangeType.Conflict, spanBase, new Span(diff1Index, diff1Index + diff2Change.Length1 - 1), Span.Invalid, null, null);
                            }
                            else
                            {
                                AddChangeType(toChanges, Diff3ChangeType.Conflict, spanBase, new Span(diff1Index, diff1Index + diff2Change.Length1 - 1), new Span(diff2Index, diff2Index + diff2Change.Length2 - 1), null, null);
                            }
                            diff1Index += diff2Change.Length1;
                            diff2Index += diff2Change.Length2;
                        }
                    }
                    else
                    {
                        // Else we have a base != v1 and base != v2
                        // Check that if v1 = v2 and range is same for (base,v1,v2) then 
                        // this can be considered as a MergeFrom1And2
                        if (diff3.Base.Length == diff3.From1.Length && diff3.Base.Length == diff3.From2.Length)
                        {
                            if (diff3.IsBaseEqualFrom1.HasValue && diff3.IsBaseEqualFrom1.Value && (!diff3.IsBaseEqualFrom2.HasValue || !diff3.IsBaseEqualFrom2.Value))
                            {
                                diff3.ChangeType = Diff3ChangeType.MergeFrom2;
                            }
                            else if (diff3.IsBaseEqualFrom2.HasValue && diff3.IsBaseEqualFrom2.Value && (!diff3.IsBaseEqualFrom1.HasValue || !diff3.IsBaseEqualFrom1.Value))
                            {
                                diff3.ChangeType = Diff3ChangeType.MergeFrom1;
                            }
                            else
                            {
                                bool isFrom1AndFrom2Equal = true;
                                for (int i = 0; i < diff3.Base.Length; i++)
                                {
                                    if (!comparer.Equals(modified1List[diff3.From1.From + i], modified2List[diff3.From2.From + i]))
                                    {
                                        isFrom1AndFrom2Equal = false;
                                        break;
                                    }
                                }
                                if (isFrom1AndFrom2Equal)
                                {
                                    diff3.ChangeType = Diff3ChangeType.MergeFrom1And2;
                                }
                            }
                        }

                        if (toChanges != null)
                            AddChangeType(toChanges, diff3);
                    }
                }
                else
                {
                    if (toChanges != null)
                        AddChangeType(toChanges, diff3);
                }
            }
            return toChanges ?? changes;
        }

        private bool IsRangeEqual(IList<T> list1, Span range1, IList<T> list2, Span range2)
        {
            var length1 = range1.Length;
            if (length1 != range2.Length)
                return false;

            bool isEqual = true;
            for (int i = 0; i < length1; i++)
            {
                if (!comparer.Equals(list1[range1.From + i], list2[range2.From + i]))
                {
                    isEqual = false;
                    break;
                }
            }
            return isEqual;
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