using System;

namespace SharpDiff
{
    public struct Span : IEquatable<Span>
    {
        public static readonly Span Invalid = new Span(0, -1);

        public Span(int from, int to)
        {
            From = @from;
            To = to;
        }

        public readonly int From;

        public readonly int To;

        public int Length
        {
            get
            {
                return To - From + 1;
            }
        }

        public bool IsValid
        {
            get
            {
                return Length > 0;
            }
        }

        public bool CanMergeWith(Span against)
        {
            if (!IsValid || !against.IsValid) return true;
            return (against.From >= From && (against.From - 1) <= To && against.To >= From);
        }

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

        public bool Equals(Span other)
        {
            if (IsValid == other.IsValid && !IsValid)
                return true;

            return (IsValid && From == other.From && To == other.To);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Span && Equals((Span)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (From * 397) ^ To;
            }
        }

        public static bool operator ==(Span left, Span right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Span left, Span right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return IsValid ? String.Format("[{0}-{1}]", From, To) : "[X,X]";
        }
    }
}