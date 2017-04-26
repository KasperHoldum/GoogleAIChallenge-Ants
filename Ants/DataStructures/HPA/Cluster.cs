using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ants.DataStructures.HPA
{
    public class Cluster
    {
        public Location ClusterSize { get; private set; }
        public Location Position { get; set; }
        public Location ClusterIndex { get; private set; }

        private string id;
        public string Id
        {
            get { return id ?? (id = string.Format("{0}:{1}", ClusterIndex.Col, ClusterIndex.Row)); }
        }

        public List<TransitNode> TransistPoints { get; set; }

        private readonly Dictionary<DataStructures.Tuple<TransitNode, TransitNode>, double> internalDistances = new Dictionary<DataStructures.Tuple<TransitNode, TransitNode>, double>();

        public double InternalDistance(TransitNode n1, TransitNode n2)
        {
            var tuple = new Tuple<TransitNode, TransitNode>(n1, n2);
            if (!internalDistances.ContainsKey(tuple))
                return 50000;
            return internalDistances[tuple];
        }

        public void ConnectInternalNodes(AStarPathFinding pathFinding)
        {

            for (int i = TransistPoints.Count - 1; i >= 0; i--)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    var t1 = TransistPoints[i];
                    var t2 = TransistPoints[j];

                    Path path = pathFinding.FindPath(t1, t2, true);



                    if (path != null)
                    {
#if DEBUG
                        if (path.IsFinished)
                            throw new InvalidOperationException("Should never try to connect two transit points");
#endif

                        t1.ConnectTo(t2);
                        int internalDistance = path.Count;
                        internalDistances[new DataStructures.Tuple<TransitNode, TransitNode>(t1, t2)] = internalDistance;
                        internalDistances[new DataStructures.Tuple<TransitNode, TransitNode>(t2, t1)] = internalDistance;
                    }
                }
            }
        }

        public Cluster(Location clusterIndex, Location clusterSize, Location position)
        {
            TransistPoints = new List<TransitNode>();
            this.ClusterIndex = clusterIndex;
            this.ClusterSize = clusterSize;
            Position = position;
        }

        public List<Location> GetBorder(ClusterCollection.ClusterRelativePosition relative)
        {
            switch (relative)
            {
                case ClusterCollection.ClusterRelativePosition.Left:
                    return Enumerable.Range(0, ClusterSize.Row).Select(s => new Location(Position.Col, Position.Row + s)).ToList();
                case ClusterCollection.ClusterRelativePosition.Right:
                    return Enumerable.Range(0, ClusterSize.Row).Select(s => new Location(Position.Col + ClusterSize.Col - 1, Position.Row + s)).ToList();
                case ClusterCollection.ClusterRelativePosition.Up:
                    return Enumerable.Range(0, ClusterSize.Col).Select(s => new Location(Position.Col + s, Position.Row)).ToList();
                case ClusterCollection.ClusterRelativePosition.Down:
                    return Enumerable.Range(0, ClusterSize.Col).Select(s => new Location(Position.Col + s, Position.Row + ClusterSize.Row - 1)).ToList();
                default:
                    throw new ArgumentOutOfRangeException("relative");
            }

        }

        public void AddTransit(TransitNode n1)
        {
            if (!TransistPoints.Contains(n1))
                this.TransistPoints.Add(n1);
        }



        public TransitNode GetTransitNode(Location node1Location)
        {
            if (this.TransistPoints.Any(s => s == node1Location))
                return TransistPoints.First(s => s == node1Location);

            return null;
        }

        private readonly Dictionary<DataStructures.Tuple<TransitNode, TransitNode>, Path> cachePaths = new Dictionary<DataStructures.Tuple<TransitNode, TransitNode>, Path>();

        public bool HasCachedPath(DataStructures.Tuple<TransitNode, TransitNode> between)
        {
            return cachePaths.ContainsKey(between);
        }

        public Path GetCachePath(DataStructures.Tuple<TransitNode, TransitNode> between)
        {
            var path = cachePaths[between];

            if (between.Item1 != path.CurrentLocation)
            {
                var reversedPath = path.ToList();
                reversedPath.Reverse();
                return new Path(reversedPath);
            }

            return path;
        }

        public DataStructures.Tuple<TransitNode, bool> AddTemporaryInternalNode(Location start, AStarPathFinding pathFinding)
        {
            if (this.TransistPoints.Any(t => t == start))
            {
                return Tuple<TransitNode, bool>.Create(this.TransistPoints.First(t => t == start), false);
            }

            var n = new TransitNode(start.Col, start.Row, this);
            foreach (TransitNode transistPoint in TransistPoints)
            {
                var t1 = new DataStructures.Tuple<TransitNode, TransitNode>(n, transistPoint);
                var t2 = new DataStructures.Tuple<TransitNode, TransitNode>(transistPoint, n);



                Path findPath = cachePaths.ContainsKey(t1) ? cachePaths[t1] : pathFinding.FindPath(n, transistPoint);
                if ( findPath != null)
                {
#if DEBUG
                    if (findPath.IsFinished)
                        throw new InvalidOperationException("Should never try to connect two transit points");
#endif
                    n.ConnectTo(transistPoint);
                    cachePaths[t1] = findPath;
                    cachePaths[t2] = findPath;
                    internalDistances[t1] = findPath.Count;
                    internalDistances[t2] = internalDistances[t1];
                }
            }

            this.AddTransit(n);

            return DataStructures.Tuple<TransitNode, bool>.Create(n, true);
        }

        public void RemoveTemporaryInternalNode(Location start)
        {
            var transit = TransistPoints.First(t => t == start);
            transit.UnconnectEverything();
            foreach (TransitNode transistPoint in TransistPoints)
            {
                internalDistances.Remove(new DataStructures.Tuple<TransitNode, TransitNode>(transit, transistPoint));
                internalDistances.Remove(new DataStructures.Tuple<TransitNode, TransitNode>(transistPoint, transit));
            }

            TransistPoints.Remove(transit);
        }
    }
}