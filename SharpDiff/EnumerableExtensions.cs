using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SharpDiff
{
    internal static class EnumerableExtensions
    {
        internal static IList<T> ToRandomAccess<T>(this IEnumerable<T> collection)
        {
            var result = collection as IList<T>;
            if (result != null)
                return result;

            return collection.ToList();
        }
    }
}