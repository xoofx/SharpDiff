using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SharpDiff
{
    /// <summary>
    /// This class holds a single collection from either the first or the second, or both,
    /// collections given to the <see cref="AlignedDiff{T}"/> class, along
    /// with the type of change that the elements produce.
    /// </summary>
    public sealed class AlignedDiffChange : IEquatable<AlignedDiffChange>
    {
        private readonly ChangeType change;
        private readonly int index1;
        private readonly int index2;

        /// <summary>
        /// Initializes a new instance of <see cref="AlignedDiffChange"/>.
        /// </summary>
        /// <param name="change">
        /// The <see cref="Change">type</see> of change this <see cref="AlignedDiffChange"/> details.
        /// </param>
        /// <param name="index1">
        /// The index of the element from the first collection. If <paramref name="change"/> is <see cref="ChangeType.Added"/>, then
        /// this parameter has no meaning.
        /// </param>
        /// <param name="index2">
        /// The index of the element from the second collection. If <paramref name="change"/> is <see cref="ChangeType.Deleted"/>, then
        /// this parameter has no meaning.
        /// </param>
        public AlignedDiffChange(ChangeType change, int index1, int index2)
        {
            this.change = change;
            this.index1 = index1;
            this.index2 = index2;
        }

        /// <summary>
        /// The <see cref="Change">type</see> of change this <see cref="AlignedDiffChange"/> details.
        /// </summary>
        public ChangeType Change
        {
            get
            {
                return change;
            }
        }

        /// <summary>
        /// The element from the first collection. If <see cref="System.Type"/> is <see cref="ChangeType.Added"/>, then
        /// the value of this property has no meaning.
        /// </summary>
        public int Index1
        {
            get
            {
                return index1;
            }
        }

        /// <summary>
        /// The element from the second collection. If <see cref="System.Type"/> is <see cref="ChangeType.Deleted"/>, then
        /// the value of this property has no meaning.
        /// </summary>
        public int Index2
        {
            get
            {
                return index2;
            }
        }

        #region IEquatable<AlignedDiffChange<T>> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(AlignedDiffChange other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(other.index1, index1) && Equals(other.index2, index2) && Equals(other.change, change);
        }

        #endregion

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof (AlignedDiffChange))
                return false;
            return Equals((AlignedDiffChange) obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = index1.GetHashCode();
                result = (result*397) ^ index2.GetHashCode();
                result = (result*397) ^ change.GetHashCode();
                return result;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            switch (Change)
            {
                case ChangeType.Same:
                    return "  " + index1;

                case ChangeType.Added:
                    return "+ " + index2;

                case ChangeType.Deleted:
                    return "- " + index1;

                case ChangeType.Changed:
                    return "* " + index1 + " --> " + index2;

                default:
                    return change + ": " + index1 + " --> " + index2;
            }
        }
    }
}