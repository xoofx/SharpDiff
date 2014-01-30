using System;

namespace SharpDiff
{
    /// <summary>
    /// A span defining a region in a list.
    /// </summary>
    public struct Span : IEquatable<Span>
    {
        /// <summary>
        /// An invalid span.
        /// </summary>
        public static readonly Span Invalid = new Span(0, -1);

        /// <summary>
        /// Initializes a new instance of the <see cref="Span"/> struct.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        public Span(int from, int to)
        {
            From = @from;
            To = to;
        }

        /// <summary>
        /// The index "from" in the list (inclusive).
        /// </summary>
        public readonly int From;

        /// <summary>
        /// The index "to" in the list (inclusive).
        /// </summary>
        public readonly int To;

        /// <summary>
        /// Gets the length of this span.
        /// </summary>
        /// <value>The length.</value>
        public int Length
        {
            get
            {
                return To - From + 1;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this span is valid (a length > 0)
        /// </summary>
        /// <value><c>true</c> if this span is valid; otherwise, <c>false</c>.</value>
        public bool IsValid
        {
            get
            {
                return Length > 0;
            }
        }

        /// <summary>
        /// Determines whether this instance can be merged with the specified span.
        /// </summary>
        /// <param name="against">The against.</param>
        /// <returns><c>true</c> if this instance can be merged with the specified span; otherwise, <c>false</c>.</returns>
        public bool CanMergeWith(Span against)
        {
            if (!IsValid || !against.IsValid) return true;
            return (against.From >= From && (against.From - 1) <= To && against.To >= From);
        }

        /// <summary>
        /// Merges the specified spans.
        /// </summary>
        /// <param name="source">The source span.</param>
        /// <param name="against">The against span.</param>
        /// <returns>A merged span or an <see cref="Invalid"/> span if merge is impossible.</returns>
        public static Span Merge(Span source, Span against)
        {
            if (!source.CanMergeWith(against))
            {
                return Invalid;
            }

            if (source.IsValid == against.IsValid)
            {
                return new Span(Math.Min(source.From, against.From) , Math.Max(source.To, against.To));    
            }

            return source.IsValid ? source : against;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(Span other)
        {
            if (IsValid == other.IsValid && !IsValid)
                return true;

            return (IsValid && From == other.From && To == other.To);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Span && Equals((Span)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (From * 397) ^ To;
            }
        }

        /// <summary>
        /// Implements the ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Span left, Span right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Span left, Span right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return IsValid ? String.Format("[{0}-{1}]", From, To) : "[X,X]";
        }
    }
}