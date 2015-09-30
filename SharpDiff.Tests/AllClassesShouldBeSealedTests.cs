using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace SharpDiff.Tests
{
    [TestFixture]
    public class AllClassesShouldBeSealedTests
    {
        public IEnumerable<Type> AllTypesInDiffLib()
        {
            return
                from type in typeof(Diff2).Assembly.GetTypes()
                where type.IsPublic
                      && type.IsClass
                select type;
        }

        [TestCaseSource("AllTypesInDiffLib")]
        public void TypeShouldBeSealed(Type type)
        {
            Assert.That(type.IsSealed, Is.True, $"Type {type.FullName} is not sealed");
        }
    }
}