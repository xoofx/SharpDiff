using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpDiff
{
    /// <summary>
    /// This class provides a diff algorithm between 2 collections.
    /// </summary>
    /// <typeparam name="T">
    /// The types of elements in the collections being compared.
    /// </typeparam>
    internal struct Diff2<T> : IEnumerable<Diff2Change>
    {
        private readonly int _Collection1Length;
        private readonly int _Collection2Length;
        private readonly LongestCommonSubstring<T> _LongestCommonSubstring;

        /// <summary>
        /// Initializes a new instance of <see cref="Diff2"/>
        /// using the default <see cref="IEqualityComparer{T}"/> instance for the
        /// <typeparamref name="T"/> type.
        /// </summary>
        /// <param name="collection1">
        /// The first collection of items.
        /// </param>
        /// <param name="collection2">
        /// The second collection of items.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="collection1"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="collection2"/> is <c>null</c>.</para>
        /// </exception>
        public Diff2(IEnumerable<T> collection1, IEnumerable<T> collection2)
            : this(collection1, collection2, EqualityComparer<T>.Default)
        {
            // Nothing here
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Diff2"/>.
        /// </summary>
        /// <param name="collection1">
        /// The first collection of items.
        /// </param>
        /// <param name="collection2">
        /// The second collection of items.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}"/> that will be used to compare elements from
        /// <paramref name="collection1"/> with elements from <paramref name="collection2"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="collection1"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="collection2"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="comparer"/> is <c>null</c>.</para>
        /// </exception>
        public Diff2(IEnumerable<T> collection1, IEnumerable<T> collection2, IEqualityComparer<T> comparer)
        {
            IList<T> randomAccess1 = collection1.ToRandomAccess();
            IList<T> randomAccess2 = collection2.ToRandomAccess();

            _Collection1Length = randomAccess1.Count;
            _Collection2Length = randomAccess2.Count;
            _LongestCommonSubstring = new LongestCommonSubstring<T>(randomAccess1, randomAccess2, comparer);
        }

        #region IEnumerable<Diff2Change> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<Diff2Change> GetEnumerator()
        {
            return Generate().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Generates the diff between the two collections.
        /// </summary>
        public IEnumerable<Diff2Change> Generate()
        {
            return GenerateSections(0, _Collection1Length, 0, _Collection2Length);
        }

        private IEnumerable<Diff2Change> GenerateSections(int lower1, int upper1, int lower2, int upper2)
        {
            if (lower1 == upper1 && lower2 == upper2)
                yield break;

            if (lower1 == upper1)
            {
                yield return new Diff2Change(false, 0, upper2 - lower2);
                yield break;
            }

            if (lower2 == upper2)
            {
                yield return new Diff2Change(false, upper1 - lower1, 0);
                yield break;
            }

            LongestCommonSubstringResult lcsr = _LongestCommonSubstring.Find(lower1, upper1, lower2, upper2);
            if (lcsr == null)
            {
                yield return new Diff2Change(false, upper1 - lower1, upper2 - lower2);
                yield break;
            }

            if (lower1 < lcsr.PositionInCollection1 || lower2 < lcsr.PositionInCollection2)
            {
                foreach (Diff2Change prevSection in GenerateSections(lower1, lcsr.PositionInCollection1, lower2, lcsr.PositionInCollection2)) yield return prevSection;
            }
            yield return new Diff2Change(true, lcsr.Length, lcsr.Length);
            if (lcsr.PositionInCollection1 + lcsr.Length < upper1 || lcsr.PositionInCollection2 + lcsr.Length < upper2)
            {
                foreach (Diff2Change nextSection in GenerateSections(lcsr.PositionInCollection1 + lcsr.Length, upper1, lcsr.PositionInCollection2 + lcsr.Length, upper2))
                    yield return nextSection;
            }
        }
    }
}