namespace Ants.DataStructures
{
    public class Tuple<T, TT>
    {
        public T Item1 { get; set; }
        public TT Item2 { get; set; }

        public Tuple(T item1, TT item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public static Tuple<T, TT> Create(T item1, TT item2)
        {
            return new Tuple<T, TT>(item1, item2);
        }

        public override string ToString()
        {
            return string.Format("Item1: {0}, Item2: {1}", Item1, Item2);
        }

        public bool Equals(Tuple<T, TT> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Item1, Item1) && Equals(other.Item2, Item2);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Tuple<T, TT>)) return false;
            return Equals((Tuple<T, TT>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Item1.GetHashCode()*397) ^ Item2.GetHashCode();
            }
        }

        public static bool operator ==(Tuple<T, TT> left, Tuple<T, TT> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Tuple<T, TT> left, Tuple<T, TT> right)
        {
            return !Equals(left, right);
        }
    }

    public class Tuple<T, TT, TTT>
    {
        public T Item1 { get; set; }
        public TT Item2 { get; set; }
        public TTT Item3 { get; set; }

        public Tuple(T item1, TT item2, TTT item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }

        public static Tuple<T, TT,TTT> Create(T item1, TT item2, TTT item3)
        {
            return new Tuple<T, TT,TTT>(item1, item2, item3);
        }


        public bool Equals(Tuple<T, TT, TTT> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Item1, Item1) && Equals(other.Item2, Item2) && Equals(other.Item3, Item3);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Tuple<T, TT, TTT>)) return false;
            return Equals((Tuple<T, TT, TTT>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = Item1.GetHashCode();
                result = (result*397) ^ Item2.GetHashCode();
                result = (result*397) ^ Item3.GetHashCode();
                return result;
            }
        }

        public static bool operator ==(Tuple<T, TT, TTT> left, Tuple<T, TT, TTT> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Tuple<T, TT, TTT> left, Tuple<T, TT, TTT> right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("Item1: {0}, Item2: {1}, Item3: {2}", Item1, Item2, Item3);
        }
    }
}
