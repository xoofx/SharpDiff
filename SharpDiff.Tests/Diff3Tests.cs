using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SharpDiff.Tests
{
    [TestFixture]
    public class Diff3Tests
    {
        [Test]
        public void TestAllEquals()
        {
            //                                   0    1    2    3    4    5 
            var baseList = new List<string>() { "A", "B", "C", "D", "E", "F" };
            var mod1List = new List<string>() { "A", "B", "C", "D", "E", "F" };
            var mod2List = new List<string>() { "A", "B", "C", "D", "E", "F" };

            var diff3 = Diff3.Compare(baseList, mod1List, mod2List, EqualityComparer<string>.Default).ToList();
            Assert.AreEqual(1, diff3.Count);

            // All elements are identical
            Assert.AreEqual(Diff3ChangeType.Equal, diff3[0].ChangeType);
            Assert.AreEqual(new Span(0, 5), diff3[0].Base);
            Assert.AreEqual(new Span(0, 5), diff3[0].From1);
            Assert.AreEqual(new Span(0, 5), diff3[0].From2);
        }

        [Test]
        public void ConflictFirstElement()
        {
            //                                   0    1    2    3    4    5 
            var baseList = new List<string>() { "A", "B", "C", "D", "E", "F" };
            var mod1List = new List<string>() { "X", "B", "C", "D", "E", "F" };
            var mod2List = new List<string>() { "Y", "B", "C", "D", "E", "F" };

            var diff3 = Diff3.Compare(baseList, mod1List, mod2List, EqualityComparer<string>.Default).ToList();
            Assert.AreEqual(2, diff3.Count);

            // Conflict on base:"A", v1:"X", v2:"Y"
            Assert.AreEqual(Diff3ChangeType.Conflict, diff3[0].ChangeType);
            Assert.AreEqual(new Span(0, 0), diff3[0].Base);
            Assert.AreEqual(new Span(0, 0), diff3[0].From1);
            Assert.AreEqual(new Span(0, 0), diff3[0].From2); 

            // First set is identical
            Assert.AreEqual(Diff3ChangeType.Equal, diff3[1].ChangeType);
            Assert.AreEqual(new Span(1, 5), diff3[1].Base);
            Assert.AreEqual(new Span(1, 5), diff3[1].From1);
            Assert.AreEqual(new Span(1, 5), diff3[1].From2);
        }


        [Test]
        public void ConflictMiddle()
        {
            //                                   0    1    2    3    4    5 
            var baseList = new List<string>() { "A", "B", "C", "D", "E", "F" };
            var mod1List = new List<string>() { "A", "B", "X", "D", "E", "F" };
            var mod2List = new List<string>() { "A", "B", "Y", "D", "E", "F" };

            var diff3 = Diff3.Compare(baseList, mod1List, mod2List, EqualityComparer<string>.Default).ToList();
            Assert.AreEqual(3, diff3.Count);

            // First set is identical
            Assert.AreEqual(Diff3ChangeType.Equal, diff3[0].ChangeType);
            Assert.AreEqual(new Span(0, 1), diff3[0].Base);
            Assert.AreEqual(new Span(0, 1), diff3[0].From1);
            Assert.AreEqual(new Span(0, 1), diff3[0].From2); 

            // Conflict on base:"C", v1:"X", v2:"Y"
            Assert.AreEqual(Diff3ChangeType.Conflict, diff3[1].ChangeType);
            Assert.AreEqual(new Span(2, 2), diff3[1].Base);
            Assert.AreEqual(new Span(2, 2), diff3[1].From1);
            Assert.AreEqual(new Span(2, 2), diff3[1].From2);

            // Last set is identical
            Assert.AreEqual(Diff3ChangeType.Equal, diff3[2].ChangeType);
            Assert.AreEqual(new Span(3, 5), diff3[2].Base);
            Assert.AreEqual(new Span(3, 5), diff3[2].From1);
            Assert.AreEqual(new Span(3, 5), diff3[2].From2);
        }


        [Test]
        public void ConflictLastElement()
        {
            //                                   0    1    2    3    4    5 
            var baseList = new List<string>() { "A", "B", "C", "D", "E", "F" };
            var mod1List = new List<string>() { "A", "B", "C", "D", "E", "X" };
            var mod2List = new List<string>() { "A", "B", "C", "D", "E", "Y" };

            var diff3 = Diff3.Compare(baseList, mod1List, mod2List, EqualityComparer<string>.Default).ToList();
            Assert.AreEqual(2, diff3.Count);

            // First set is identical
            Assert.AreEqual(Diff3ChangeType.Equal, diff3[0].ChangeType);
            Assert.AreEqual(new Span(0, 4), diff3[0].Base);
            Assert.AreEqual(new Span(0, 4), diff3[0].From1);
            Assert.AreEqual(new Span(0, 4), diff3[0].From2);

            // Conflict on base:"C", v1:"X", v2:"Y"
            Assert.AreEqual(Diff3ChangeType.Conflict, diff3[1].ChangeType);
            Assert.AreEqual(new Span(5, 5), diff3[1].Base);
            Assert.AreEqual(new Span(5, 5), diff3[1].From1);
            Assert.AreEqual(new Span(5, 5), diff3[1].From2);
        }


        [Test]
        public void MergeFirstElement()
        {
            //                                   0    1    2    3    4    5 
            var baseList = new List<string>() { "A", "B", "C", "D", "E", "F" };
            var mod1List = new List<string>() { "X", "B", "C", "D", "E", "F" };
            var mod2List = new List<string>() { "A", "B", "C", "D", "E", "F" };

            var diff3 = Diff3.Compare(baseList, mod1List, mod2List, EqualityComparer<string>.Default).ToList();
            Assert.AreEqual(2, diff3.Count);

            // Merge from v1 "X" to base, v2
            Assert.AreEqual(Diff3ChangeType.MergeFrom1, diff3[0].ChangeType);
            Assert.AreEqual(new Span(0, 0), diff3[0].Base);
            Assert.AreEqual(new Span(0, 0), diff3[0].From1);
            Assert.AreEqual(new Span(0, 0), diff3[0].From2);

            // First set is identical
            Assert.AreEqual(Diff3ChangeType.Equal, diff3[1].ChangeType);
            Assert.AreEqual(new Span(1, 5), diff3[1].Base);
            Assert.AreEqual(new Span(1, 5), diff3[1].From1);
            Assert.AreEqual(new Span(1, 5), diff3[1].From2);
        }

        [Test]
        public void MergeFirstElementFrom1And2()
        {
            //                                   0    1    2    3    4    5 
            var baseList = new List<string>() { "X", "B", "C", "D", "E", "F" };
            var mod1List = new List<string>() { "A", "B", "C", "D", "E", "F" };
            var mod2List = new List<string>() { "A", "B", "C", "D", "E", "F" };

            var diff3 = Diff3.Compare(baseList, mod1List, mod2List, EqualityComparer<string>.Default).ToList();
            Assert.AreEqual(2, diff3.Count);

            // Merge from v1 "X" to base, v2
            Assert.AreEqual(Diff3ChangeType.MergeFrom1And2, diff3[0].ChangeType);
            Assert.AreEqual(new Span(0, 0), diff3[0].Base);
            Assert.AreEqual(new Span(0, 0), diff3[0].From1);
            Assert.AreEqual(new Span(0, 0), diff3[0].From2);

            // First set is identical
            Assert.AreEqual(Diff3ChangeType.Equal, diff3[1].ChangeType);
            Assert.AreEqual(new Span(1, 5), diff3[1].Base);
            Assert.AreEqual(new Span(1, 5), diff3[1].From1);
            Assert.AreEqual(new Span(1, 5), diff3[1].From2);
        }

        [Test]
        public void MergeMiddle()
        {
            //                                   0    1    2    3    4    5 
            var baseList = new List<string>() { "A", "B", "C", "D", "E", "F" };
            var mod1List = new List<string>() { "A", "B", "C", "D", "E", "F" };
            var mod2List = new List<string>() { "A", "B", "X", "D", "E", "F" };

            var diff3 = Diff3.Compare(baseList, mod1List, mod2List, EqualityComparer<string>.Default).ToList();
            Assert.AreEqual(3, diff3.Count);

            // First set is identical
            Assert.AreEqual(Diff3ChangeType.Equal, diff3[0].ChangeType);
            Assert.AreEqual(new Span(0, 1), diff3[0].Base);
            Assert.AreEqual(new Span(0, 1), diff3[0].From1);
            Assert.AreEqual(new Span(0, 1), diff3[0].From2);

            // Conflict on base:"C", v1:"X", v2:"Y"
            Assert.AreEqual(Diff3ChangeType.MergeFrom2, diff3[1].ChangeType);
            Assert.AreEqual(new Span(2, 2), diff3[1].Base);
            Assert.AreEqual(new Span(2, 2), diff3[1].From1);
            Assert.AreEqual(new Span(2, 2), diff3[1].From2);

            // Last set is identical
            Assert.AreEqual(Diff3ChangeType.Equal, diff3[2].ChangeType);
            Assert.AreEqual(new Span(3, 5), diff3[2].Base);
            Assert.AreEqual(new Span(3, 5), diff3[2].From1);
            Assert.AreEqual(new Span(3, 5), diff3[2].From2);
        }


        [Test]
        public void MergeLastElement()
        {
            //                                   0    1    2    3    4    5 
            var baseList = new List<string>() { "A", "B", "C", "D", "E", "F" };
            var mod1List = new List<string>() { "A", "B", "C", "D", "E", "X" };
            var mod2List = new List<string>() { "A", "B", "C", "D", "E", "F" };

            var diff3 = Diff3.Compare(baseList, mod1List, mod2List, EqualityComparer<string>.Default).ToList();
            Assert.AreEqual(2, diff3.Count);

            // First set is identical
            Assert.AreEqual(Diff3ChangeType.Equal, diff3[0].ChangeType);
            Assert.AreEqual(new Span(0, 4), diff3[0].Base);
            Assert.AreEqual(new Span(0, 4), diff3[0].From1);
            Assert.AreEqual(new Span(0, 4), diff3[0].From2);

            // Conflict on base:"C", v1:"X", v2:"Y"
            Assert.AreEqual(Diff3ChangeType.MergeFrom1, diff3[1].ChangeType);
            Assert.AreEqual(new Span(5, 5), diff3[1].Base);
            Assert.AreEqual(new Span(5, 5), diff3[1].From1);
            Assert.AreEqual(new Span(5, 5), diff3[1].From2);
        }

        [Test]
        public void MergeDifferentSize()
        {
            //                                   0    1    2    3    4    5    6
            var baseList = new List<string>() { "A", "B", "C" };
            var mod1List = new List<string>() { "A", "B", "C", "D", "E", "F" };
            var mod2List = new List<string>() { "A", "B", "C", "D", "E", "F", "G" };

            var diff3 = Diff3.Compare(baseList, mod1List, mod2List, EqualityComparer<string>.Default).ToList();
            Assert.AreEqual(3, diff3.Count);

            // Check 0) Equal "A B C"
            Assert.AreEqual(Diff3ChangeType.Equal, diff3[0].ChangeType);
            Assert.AreEqual(new Span(0, 2), diff3[0].Base);
            Assert.AreEqual(new Span(0, 2), diff3[0].From1);
            Assert.AreEqual(new Span(0, 2), diff3[0].From2);

            // Check 1) MergeFrom1And2 "D E F"
            Assert.AreEqual(Diff3ChangeType.MergeFrom1And2, diff3[1].ChangeType);
            Assert.AreEqual(new Span(3, 3), diff3[1].Base);
            Assert.AreEqual(new Span(3, 5), diff3[1].From1);
            Assert.AreEqual(new Span(3, 5), diff3[1].From2);

            // Check 2) MergeFrom2 "G"
            Assert.AreEqual(Diff3ChangeType.MergeFrom2, diff3[2].ChangeType);
            Assert.AreEqual(new Span(3, 3), diff3[2].Base);
            Assert.AreEqual(Span.Invalid, diff3[2].From1);
            Assert.AreEqual(new Span(6, 6), diff3[2].From2);
        }

        [Test]
        public void ConflictMergeMultipleElementsFrom1And2()
        {
            //                                   0    1    2    3    4    5 
            var baseList = new List<string>() { "A", "F" };
            var mod1List = new List<string>() { "A", "X", "F" };
            var mod2List = new List<string>() { "A", "Y", "F" };

            var diff3 = Diff3.Compare(baseList, mod1List, mod2List, EqualityComparer<string>.Default).ToList();
            Assert.AreEqual(3, diff3.Count);

            // 0) Equal A = A = A
            Assert.AreEqual(Diff3ChangeType.Equal, diff3[0].ChangeType);
            Assert.AreEqual(new Span(0, 0), diff3[0].Base);
            Assert.AreEqual(new Span(0, 0), diff3[0].From1);
            Assert.AreEqual(new Span(0, 0), diff3[0].From2);

            // 1) Conflict on X
            Assert.AreEqual(Diff3ChangeType.Conflict, diff3[1].ChangeType);
            Assert.AreEqual(new Span(1, 1), diff3[1].Base);
            Assert.AreEqual(new Span(1, 1), diff3[1].From1);
            Assert.AreEqual(new Span(1, 1), diff3[1].From2);

            // 4) Equal "F"
            Assert.AreEqual(Diff3ChangeType.Equal, diff3[2].ChangeType);
            Assert.AreEqual(new Span(1, 1), diff3[2].Base);
            Assert.AreEqual(new Span(2, 2), diff3[2].From1);
            Assert.AreEqual(new Span(2, 2), diff3[2].From2);
        }

        [Test]
        public void ConflictMultipleElementsFrom1And2()
        {
            //                                   0    1    2    3    4    5 
            var baseList = new List<string>() { "A", "F" };
            var mod1List = new List<string>() { "A", "X", "Y", "F" };
            var mod2List = new List<string>() { "A", "Y", "Z", "W", "F" };

            var diff3 = Diff3.Compare(baseList, mod1List, mod2List, EqualityComparer<string>.Default).ToList();
            Assert.AreEqual(5, diff3.Count);

            // 0) Equal A = A = A
            Assert.AreEqual(Diff3ChangeType.Equal, diff3[0].ChangeType);
            Assert.AreEqual(new Span(0, 0), diff3[0].Base);
            Assert.AreEqual(new Span(0, 0), diff3[0].From1);
            Assert.AreEqual(new Span(0, 0), diff3[0].From2);

            // 1) Conflict on X
            Assert.AreEqual(Diff3ChangeType.Conflict, diff3[1].ChangeType);
            Assert.AreEqual(new Span(1, 1), diff3[1].Base);
            Assert.AreEqual(new Span(1, 1), diff3[1].From1);
            Assert.AreEqual(Span.Invalid, diff3[1].From2);

            // 2) MergeFrom1And2 "Y" and "Y" 
            Assert.AreEqual(Diff3ChangeType.MergeFrom1And2, diff3[2].ChangeType);
            Assert.AreEqual(new Span(1, 1), diff3[2].Base);
            Assert.AreEqual(new Span(2, 2), diff3[2].From1);
            Assert.AreEqual(new Span(1, 1), diff3[2].From2);

            // 3) MergeFrom2 "Z" and "W" 
            Assert.AreEqual(Diff3ChangeType.MergeFrom2, diff3[3].ChangeType);
            Assert.AreEqual(new Span(1, 1), diff3[3].Base);
            Assert.AreEqual(Span.Invalid, diff3[3].From1);
            Assert.AreEqual(new Span(2, 3), diff3[3].From2);

            // 4) Equal "F"
            Assert.AreEqual(Diff3ChangeType.Equal, diff3[4].ChangeType);
            Assert.AreEqual(new Span(1, 1), diff3[4].Base);
            Assert.AreEqual(new Span(3, 3), diff3[4].From1);
            Assert.AreEqual(new Span(4, 4), diff3[4].From2);
        }

        [Test]
        public void ConflictAndMergeSimple()
        {
            //                                   0    1    2    3    4    5 
            var baseList = new List<string>() { "A", "B", "C", "D", "E", "F"};
            var mod1List = new List<string>() { "A", "D", "E", "B", "C", "F"};
            var mod2List = new List<string>() { "A", "B", "D", "E", "C", "F"};

            var diff3 = Diff3.Compare(baseList, mod1List, mod2List, EqualityComparer<string>.Default).ToList();

            //     0    1     2    3    4
            //base A          B  C,D,E  F
            //v1   A    D,E   B  C      F
            //v2   A          B  D,E,C  F

            Assert.AreEqual(5, diff3.Count);

            // Check 0) A same for base, v1, v2
            Assert.AreEqual(Diff3ChangeType.Equal, diff3[0].ChangeType);
            Assert.AreEqual(new Span(0, 0), diff3[0].Base);
            Assert.AreEqual(new Span(0, 0), diff3[0].From1);
            Assert.AreEqual(new Span(0, 0), diff3[0].From2);

            // Check 1) Merge from 1
            Assert.AreEqual(Diff3ChangeType.MergeFrom1, diff3[1].ChangeType);
            Assert.AreEqual(Span.Invalid, diff3[1].Base);
            Assert.AreEqual(new Span(1, 2), diff3[1].From1);
            Assert.AreEqual(Span.Invalid, diff3[1].From2);

            // Check 2) B same for base, v1, v2
            Assert.AreEqual(Diff3ChangeType.Equal, diff3[2].ChangeType);
            Assert.AreEqual(new Span(1, 1), diff3[2].Base);
            Assert.AreEqual(new Span(3, 3), diff3[2].From1);
            Assert.AreEqual(new Span(1, 1), diff3[2].From2);

            // Check 3) Merge from base
            Assert.AreEqual(Diff3ChangeType.Conflict, diff3[3].ChangeType);
            Assert.AreEqual(new Span(2, 4), diff3[3].Base);
            Assert.AreEqual(new Span(4, 4), diff3[3].From1);
            Assert.AreEqual(new Span(2, 4), diff3[3].From2);

            // Check 4) B same for base, v1, v2
            Assert.AreEqual(Diff3ChangeType.Equal, diff3[4].ChangeType);
            Assert.AreEqual(new Span(5, 5), diff3[4].Base);
            Assert.AreEqual(new Span(5, 5), diff3[4].From1);
            Assert.AreEqual(new Span(5, 5), diff3[4].From2);
        }

        [Test]
        public void MergeChangesOnlyFrom1WithEmptyBase()
        {
            //                                   0    1    2    3    4    5 
            var baseList = new List<string>() {};
            var mod1List = new List<string>() { "A", "D", "E", "B", "C", "F" };
            var mod2List = new List<string>() {};

            var diff3 = Diff3.Compare(baseList, mod1List, mod2List, EqualityComparer<string>.Default).ToList();

            Assert.AreEqual(1, diff3.Count);
            Assert.AreEqual(Diff3ChangeType.MergeFrom1, diff3[0].ChangeType);
            Assert.AreEqual(new Span(0, 0), diff3[0].Base);
            Assert.AreEqual(new Span(0, 5), diff3[0].From1);
            Assert.AreEqual(Span.Invalid, diff3[0].From2);
        }

        [Test]
        public void MergeChangesOnlyFrom2WithEmptyBase()
        {
            //                                   0    1    2    3    4    5 
            var baseList = new List<string>() { };
            var mod1List = new List<string>() { };
            var mod2List = new List<string>() { "A", "D", "E", "B", "C", "F" };

            var diff3 = Diff3.Compare(baseList, mod1List, mod2List, EqualityComparer<string>.Default).ToList();

            Assert.AreEqual(1, diff3.Count);
            Assert.AreEqual(Diff3ChangeType.MergeFrom2, diff3[0].ChangeType);
            Assert.AreEqual(new Span(0, 0), diff3[0].Base);
            Assert.AreEqual(Span.Invalid, diff3[0].From1);
            Assert.AreEqual(new Span(0, 5), diff3[0].From2);
        }

        [Test]
        public void MergeChangesOnlyFrom1And2WithEmptyBase()
        {
            //                                   0    1    2    3    4    5 
            var baseList = new List<string>() { };
            var mod1List = new List<string>() { "A", "D", "E", "B", "C", "F" };
            var mod2List = new List<string>() { "A", "D", "E", "B", "C", "F" };

            var diff3 = Diff3.Compare(baseList, mod1List, mod2List, EqualityComparer<string>.Default).ToList();

            Assert.AreEqual(1, diff3.Count);
            Assert.AreEqual(Diff3ChangeType.MergeFrom1And2, diff3[0].ChangeType);
            Assert.AreEqual(new Span(0, 0), diff3[0].Base);
            Assert.AreEqual(new Span(0, 5), diff3[0].From1);
            Assert.AreEqual(new Span(0, 5), diff3[0].From2);
        }
    }
}