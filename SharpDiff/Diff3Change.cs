using System;

namespace SharpDiff
{
    /// <summary>
    /// A diff3 change.
    /// </summary>
    public sealed class Diff3Change
    {
        internal bool? IsBaseEqualFrom1;
        internal bool? IsBaseEqualFrom2;

        /// <summary>
        /// Initializes a new instance of the <see cref="Diff3Change"/> class.
        /// </summary>
        public Diff3Change()
        {
        }

        /// <summary>
        /// The span for the changes from base.
        /// </summary>
        public Span Base;

        /// <summary>
        /// The span for the changes from list1.
        /// </summary>
        public Span From1;

        /// <summary>
        /// The span for the changes from list2.
        /// </summary>
        public Span From2;

        /// <summary>
        /// Gets or sets the type of the change.
        /// </summary>
        /// <value>The type of the change.</value>
        public Diff3ChangeType ChangeType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this diff3 can be merged.
        /// </summary>
        /// <value><c>true</c> if this diff3 can be merged; otherwise, <c>false</c>.</value>
        public bool CanMerge
        {
            get
            {
                return ChangeType != Diff3ChangeType.Conflict;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("{0}, Base = ({1}), [1] = {2}, [2] = {3}", ChangeType, Base, From1, From2);
        }
    }
}