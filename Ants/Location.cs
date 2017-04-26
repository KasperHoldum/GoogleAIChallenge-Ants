using System;
using System.Globalization;

namespace Ants
{
    public class Location
    {
        public Location(int col, int row)
        {
            Row = row;
            Col = col;
        }

        public int Row { get; private set; }
        public int Col { get; private set; }

        public int this[int index]
        {
            get
            {
                if (index == 0)
                {
                    return Col;
                }
                if (index == 1)
                {
                    return Row;
                }

                throw new ArgumentOutOfRangeException("index", index, "Must be within the interval [0:1].");
            }
        }

        public Location Wrap(Location wrap)
        {
            return new Location((wrap.Col + this.Col) % wrap.Col, (wrap.Row + this.Row) % wrap.Row);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0};{1})", Col, Row);
        }

        public static Location operator *(Location loc, int scalar)
        {
            return new Location(loc.Col * scalar,loc.Row * scalar);
        }

        public static Location operator +(Location loc, Location adds)
        {
            return new Location(loc.Col + adds.Col, loc.Row + adds.Row);
        }

        public static Location operator -(Location loc, Location adds)
        {
            return new Location(loc.Col - adds.Col, loc.Row - adds.Row);
        }

        #region Equality Members

        public bool Equals(Location other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Row == Row && other.Col == Col;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            var test = obj as Location;
            return test != null && Equals(test);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Row*397) ^ Col;
            }
        }

        public static bool operator ==(Location left, Location right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Location left, Location right)
        {
            return !Equals(left, right);
        }
        #endregion

        public double Length()
        {
            return Math.Sqrt(Col*Col + Row*Row);
        }
    }
}