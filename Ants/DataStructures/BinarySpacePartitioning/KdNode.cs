namespace Ants.DataStructures.BinarySpacePartitioning
{
    public class KdNode<T> where T :Location
    {
        public KdNode<T> Right { get; set; }
        public KdNode<T> Left { get; set; }
        public T Value { get; set; }
        public KdNode(T location)
        {
            Value = location;
        }
    }
}
