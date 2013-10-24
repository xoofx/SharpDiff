using System;
using System.Collections.Generic;

namespace SharpDiff
{
    /// <summary>
    /// This class provides a 3-ways diff between 3 collections. 
    /// </summary>
    public static class Diff3
    {
        /// <summary>
        /// Compares the three collections and generate a 3-ways diff using a default comparer for T.
        /// </summary>
        /// <typeparam name="T">Type of the element in the list</typeparam>
        /// <param name="baseCollection">The base collection.</param>
        /// <param name="modified1">The collection modified 1.</param>
        /// <param name="modified2">The collection modified 2.</param>
        /// <returns>An enumeration of <see cref="Diff3Change" />.</returns>
        public static IEnumerable<Diff3Change> Compare<T>(IEnumerable<T> baseCollection, IEnumerable<T> modified1,
            IEnumerable<T> modified2)
        {
            return Compare(baseCollection, modified1, modified2, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Compares the three collections and generate a 3-ways diff.
        /// </summary>
        /// <typeparam name="T">Type of the element in the list</typeparam>
        /// <param name="baseCollection">The base collection.</param>
        /// <param name="modified1">The collection modified 1.</param>
        /// <param name="modified2">The collection modified 2.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>An enumeration of <see cref="Diff3Change" />.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// baseCollection
        /// or
        /// modified1
        /// or
        /// modified2
        /// </exception>
        public static IEnumerable<Diff3Change> Compare<T>(IEnumerable<T> baseCollection, IEnumerable<T> modified1,
            IEnumerable<T> modified2, IEqualityComparer<T> comparer)
        {
            if (baseCollection == null) throw new ArgumentNullException("baseCollection");
            if (modified1 == null) throw new ArgumentNullException("modified1");
            if (modified2 == null) throw new ArgumentNullException("modified2");
            return Compare(baseCollection.ToRandomAccess(), modified1.ToRandomAccess(), modified2.ToRandomAccess(), comparer);
        }

        /// <summary>
        /// Compares the three collections and generate a 3-ways diff using a default comparer for T.
        /// </summary>
        /// <typeparam name="T">Type of the element in the list</typeparam>
        /// <param name="baseCollection">The base collection.</param>
        /// <param name="modified1">The collection modified 1.</param>
        /// <param name="modified2">The collection modified 2.</param>
        /// <returns>An enumeration of <see cref="Diff3Change" />.</returns>
        public static IEnumerable<Diff3Change> Compare<T>(IList<T> baseCollection, IList<T> modified1,
            IList<T> modified2)
        {
            return Compare(baseCollection, modified1, modified2, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Compares the three collections and generate a 3-ways diff.
        /// </summary>
        /// <typeparam name="T">Type of the element in the list</typeparam>
        /// <param name="baseCollection">The base collection.</param>
        /// <param name="modified1">The modified1.</param>
        /// <param name="modified2">The modified2.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>An enumeration of <see cref="Diff3Change" />.</returns>
        /// <exception cref="System.ArgumentNullException">comparer</exception>
        public static IEnumerable<Diff3Change> Compare<T>(IList<T> baseCollection, IList<T> modified1, IList<T> modified2, IEqualityComparer<T> comparer)
        {
            if (baseCollection == null) throw new ArgumentNullException("baseCollection");
            if (modified1 == null) throw new ArgumentNullException("modified1");
            if (modified2 == null) throw new ArgumentNullException("modified2");
            if (comparer == null) throw new ArgumentNullException("comparer");

            var diff = new Diff3<T>(baseCollection, modified1, modified2, comparer);
            return diff.Generate();
        }
    }
}
