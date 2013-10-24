using System;
using System.Collections.Generic;


namespace SharpDiff
{

    /// <summary>
    /// This class provides a diff algorithm between 2 collections.
    /// </summary>
    /// <remarks>
    /// This class is the main entry point to perform diff comparisons between 2 collections. 
    /// It provides methods for a basic diff algorithm by recursively applying the Longest Common Substring 
    /// on pieces of the collections, and reporting sections that are similar, and those that are not,
    /// in the appropriate sequence.
    /// </remarks>
    public static class Diff2
    {
        /// <summary>
        /// Compares the two collections and generate a diff using a default comparer for T.
        /// </summary>
        /// <typeparam name="T">Type of the element in the list</typeparam>
        /// <param name="left">The first collection.</param>
        /// <param name="right">The second collection.</param>
        /// <returns>An enumeration of <see cref="Diff2Change"/>.</returns>
        public static IEnumerable<Diff2Change> Compare<T>(IEnumerable<T> left, IEnumerable<T> right)
        {
            return Compare(left, right, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Compares the two collections and generate a diff.
        /// </summary>
        /// <typeparam name="T">Type of the element in the list</typeparam>
        /// <param name="left">The first collection.</param>
        /// <param name="right">The second collection.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>An enumeration of <see cref="Diff2Change"/>.</returns>
        public static IEnumerable<Diff2Change> Compare<T>(IEnumerable<T> left, IEnumerable<T> right, IEqualityComparer<T> comparer)
        {
            if (left == null) throw new ArgumentNullException("left");
            if (right == null) throw new ArgumentNullException("right");
            return Compare(left.ToRandomAccess(), right.ToRandomAccess(), comparer);
        }

        /// <summary>
        /// Compares the two collections and generate a diff using a default comparer for T.
        /// </summary>
        /// <typeparam name="T">Type of the element in the list</typeparam>
        /// <param name="left">The first collection.</param>
        /// <param name="right">The second collection.</param>
        /// <returns>An enumeration of <see cref="Diff2Change"/>.</returns>
        public static IEnumerable<Diff2Change> Compare<T>(IList<T> left, IList<T> right)
        {
            return Compare(left, right, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Compares the two collections and generate a diff.
        /// </summary>
        /// <typeparam name="T">Type of the element in the list</typeparam>
        /// <param name="left">The first collection.</param>
        /// <param name="right">The second collection.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>An enumeration of <see cref="Diff2Change"/>.</returns>
        public static IEnumerable<Diff2Change> Compare<T>(IList<T> left, IList<T> right, IEqualityComparer<T> comparer)
        {
            if (left == null) throw new ArgumentNullException("left");
            if (right == null) throw new ArgumentNullException("right");
            if (comparer == null) throw new ArgumentNullException("comparer");

            var diff = new Diff2<T>(left, right, comparer);
            return diff.Generate();
        }

        /// <summary>
        /// Compares the two collections and generate a diff by attempting to align similar individual elements inside
        /// replace-blocks.
        /// </summary>
        /// <typeparam name="T">Type of the element in the list</typeparam>
        /// <param name="first">The first collection.</param>
        /// <param name="second">The second collection.</param>
        /// <param name="similarityComparer">
        /// The <see cref="ISimilarityComparer{T}"/> that will be used to attempt to align elements
        /// inside blocks that consists of elements from the first collection being replaced
        /// with elements from the second collection.
        /// </param>
        /// <param name="alignmentFilter">
        /// The <see cref="ISimilarityComparer{T}"/> that will be used to determine if
        /// two aligned elements are similar enough to be report them as a change from
        /// one to another, or to report them as one being deleted and the other added in
        /// its place.
        /// </param>
        /// <returns>
        /// A collection of <see cref="AlignedDiffChange{T}"/> objects, one for
        /// each element in the list in the first or second collection (sometimes one instance for a line
        /// from both, when lines are equal or similar.)
        /// </returns>
        public static IEnumerable<AlignedDiffChange<T>> CompareAndAlign<T>(IList<T> first, IList<T> second, ISimilarityComparer<T> similarityComparer, IAlignmentFilter<T> alignmentFilter)
        {
            return CompareAndAlign(first, second, EqualityComparer<T>.Default, similarityComparer, alignmentFilter);
        }

        /// <summary>
        /// Compares the two collections and generate a diff by attempting to align similar individual elements inside
        /// replace-blocks.
        /// </summary>
        /// <typeparam name="T">Type of the element in the list</typeparam>
        /// <param name="first">The first collection.</param>
        /// <param name="second">The second collection.</param>
        /// <param name="equalityComparer">
        /// The <see cref="IEqualityComparer{T}"/> that will be used to compare elements from
        /// <paramref name="first"/> with elements from <paramref name="second"/>.
        /// </param>
        /// <param name="similarityComparer">
        /// The <see cref="ISimilarityComparer{T}"/> that will be used to attempt to align elements
        /// inside blocks that consists of elements from the first collection being replaced
        /// with elements from the second collection.
        /// </param>
        /// <param name="alignmentFilter">
        /// The <see cref="ISimilarityComparer{T}"/> that will be used to determine if
        /// two aligned elements are similar enough to be report them as a change from
        /// one to another, or to report them as one being deleted and the other added in
        /// its place.
        /// </param>
        /// <returns>
        /// A collection of <see cref="AlignedDiffChange{T}"/> objects, one for
        /// each element in the list in the first or second collection (sometimes one instance for a line
        /// from both, when lines are equal or similar.)
        /// </returns>
        public static IEnumerable<AlignedDiffChange<T>> CompareAndAlign<T>(IList<T> first, IList<T> second, IEqualityComparer<T> equalityComparer,
            ISimilarityComparer<T> similarityComparer, IAlignmentFilter<T> alignmentFilter)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");
            if (equalityComparer == null) throw new ArgumentNullException("equalityComparer");
            if (similarityComparer == null) throw new ArgumentNullException("similarityComparer");
            if (alignmentFilter == null) throw new ArgumentNullException("alignmentFilter");

            var diff = new AlignedDiff<T>(first, second, equalityComparer, similarityComparer, alignmentFilter);
            return diff.Generate();
        }
    }
}
