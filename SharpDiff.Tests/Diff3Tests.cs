using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace SharpDiff.Tests
{
    [TestFixture]
    public class Diff3Tests
    {
        [Test]
        public void SimpleTest()
        {
            var baseList = new List<int>() {1, 2, 3, 4, 5, 6};
            var mod1List = new List<int>() {1, 4, 5, 2, 3, 6};
            var mod2List = new List<int>() {1, 2, 4, 5, 3, 6};

            var diff3 = Diff3.Compare(baseList, mod1List, mod2List);

            foreach (var chunk in diff3)
            {
                Console.WriteLine(chunk);
            }
        }

         
    }
}