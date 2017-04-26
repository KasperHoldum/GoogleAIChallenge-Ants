using System;
using System.Collections.Generic;

namespace Ants.DataStructures.HPA
{
    public class ClusterCollection
    {
        private readonly GameState state;

        public static DataStructures.Tuple<Location, Location> DetermineClusterSize(GameState state, int clusterCountPerAxis)
        {
            Location primarySize = new Location((int)Math.Floor((double)state.Width / (double)clusterCountPerAxis), (int)Math.Floor((double)state.Height / (double)clusterCountPerAxis));
            Location alternateSize = new Location(state.Width - clusterCountPerAxis * primarySize.Col + primarySize.Col, state.Height - clusterCountPerAxis * primarySize.Row + primarySize.Row);

            return new DataStructures.Tuple<Location, Location>(primarySize, alternateSize);
        }


        private Cluster[][] clusters;

        public int ClusterCount { get; private set; }

        public ClusterCollection(GameState state) : this(state, 10) { }
        public ClusterCollection(GameState state, int clusterCount)
        {
            this.state = state;
            ClusterCount = clusterCount;
            ClusterSizes = DetermineClusterSize(state, ClusterCount);

        }

        protected DataStructures.Tuple<Location, Location> ClusterSizes { get; set; }

        public Cluster this[int col, int row]
        {
            get { return clusters[col][row]; }
            set { clusters[col][row] = value; }
        }
        public Cluster this[Location loc]
        {
            get { return clusters[loc.Col][loc.Row]; }
            set { clusters[loc.Col][loc.Row] = value; }
        }

        public void Initialize(AStarPathFinding pathFinding)
        {
            clusters = new Cluster[ClusterCount][];
            for (int col = 0; col < ClusterCount; col++)
            {
                clusters[col] = new Cluster[ClusterCount];
                for (int row = 0; row < ClusterCount; row++)
                {
                    var clusterIndex = new Location(col, row);
                    var clusterSize = new Location(col == ClusterCount - 1 ? ClusterSizes.Item2.Col : ClusterSizes.Item1.Col, row == ClusterCount - 1 ? ClusterSizes.Item2.Row : ClusterSizes.Item1.Row);
                    clusters[col][row] = new Cluster(clusterIndex, clusterSize, new Location(col * ClusterSizes.Item1.Col, row * ClusterSizes.Item1.Row));
                }
            }

            foreach (Tuple<Cluster, Cluster, ClusterRelativePosition> pairOfCluster in PairsOfClusters())
            {
                ComputeTransitNodes(pairOfCluster);
            }

            foreach (var cluster in clusters)
            {
                foreach (var cluster1 in cluster)
                {
                    cluster1.ConnectInternalNodes(pathFinding);
                }
            }

        }

        public enum ClusterRelativePosition
        {
            Left,
            Right,
            Up,
            Down
        }

        private void ComputeTransitNodes(DataStructures.Tuple<Cluster, Cluster, ClusterRelativePosition> pairOfCluster)
        {
            DataStructures.Tuple<ClusterRelativePosition, ClusterRelativePosition> borders = DataStructures.Tuple < ClusterRelativePosition,ClusterRelativePosition>.Create(pairOfCluster.Item3, pairOfCluster.Item3.Opposite());


            var cluster1Border = pairOfCluster.Item1.GetBorder(borders.Item1);
            var cluster2Border = pairOfCluster.Item2.GetBorder(borders.Item2);

#if DEBUG
            if (cluster1Border.Count != cluster2Border.Count)
            {
                throw new InvalidOperationException("Y NO EQUAL?!?");
            }
#endif

            List<DataStructures.Tuple<int, int>> entrances = FindEntrances(cluster1Border, cluster2Border);

            PlaceTransitPoints(entrances, pairOfCluster.Item1, cluster1Border, pairOfCluster.Item2, cluster2Border);
        }

        private void PlaceTransitPoints(IEnumerable<DataStructures.Tuple<int, int>> entrances, Cluster c1, List<Location> c1Positions, Cluster c2, List<Location> c2Positions)
        {
            foreach (var entrance in entrances)
            {
                if (entrance.Item2 <= 5)
                {

                    var node1Location = c1Positions[entrance.Item1 + entrance.Item2/2];
                    TransitNode n1 = c1.GetTransitNode(node1Location) ??
                                     new TransitNode(node1Location.Col, node1Location.Row, c1);

                    var node2Location = c2Positions[entrance.Item1 + entrance.Item2/2];
                    TransitNode n2 = c2.GetTransitNode(node2Location) ??
                                     new TransitNode(node2Location.Col, node2Location.Row, c2);

                    n1.ConnectTo(n2);

                    c1.AddTransit(n1);
                    c2.AddTransit(n2);
                }
                else
                {
                    var node1Location = c1Positions[entrance.Item1+2];
                    TransitNode n1 = c1.GetTransitNode(node1Location) ??
                                     new TransitNode(node1Location.Col, node1Location.Row, c1);

                    var node2Location = c2Positions[entrance.Item1+2];
                    TransitNode n2 = c2.GetTransitNode(node2Location) ??
                                     new TransitNode(node2Location.Col, node2Location.Row, c2);

                    var node12Location = c1Positions[entrance.Item1 + entrance.Item2 - 3];
                    TransitNode n12 = c1.GetTransitNode(node12Location) ??
                                     new TransitNode(node12Location.Col, node12Location.Row, c1);

                    var node22Location = c2Positions[entrance.Item1 + entrance.Item2 -3];
                    TransitNode n22 = c2.GetTransitNode(node22Location) ??
                                     new TransitNode(node22Location.Col, node22Location.Row, c2);

                    n1.ConnectTo(n2);
                    n12.ConnectTo(n22);

                    c1.AddTransit(n1);
                    c2.AddTransit(n2);
                    c1.AddTransit(n12);
                    c2.AddTransit(n22);
                }
            }

        }

        private List<DataStructures.Tuple<int, int>> FindEntrances(List<Location> cluster1Border, List<Location> cluster2Border)
        {
            List<DataStructures.Tuple<int, int>> entrances = new List<DataStructures.Tuple<int, int>>();

            bool hasEntranceStarted = false;
            int lastSpottedLandIndex = -1;
            for (int i = 0; i < cluster1Border.Count; i++)
            {
                if (state[cluster1Border[i].Col, cluster1Border[i].Row] == Tile.Water || state[cluster2Border[i].Col, cluster2Border[i].Row] == Tile.Water)
                {
                    if (hasEntranceStarted)
                    {
                        hasEntranceStarted = false;
                        var entrance = Tuple<int, int>.Create(lastSpottedLandIndex, i - lastSpottedLandIndex);

                        entrances.Add(entrance);
                    }
                }
                else
                {
                    if (!hasEntranceStarted)
                    {
                        hasEntranceStarted = true;
                        lastSpottedLandIndex = i;
                    }
                }
            }

            if (hasEntranceStarted)
            {
                var entrance = Tuple<int, int>.Create(lastSpottedLandIndex, cluster1Border.Count - lastSpottedLandIndex);
                entrances.Add(entrance);
            }

            return entrances;
        }

        public IEnumerable<Tuple<Cluster, Cluster, ClusterRelativePosition>> PairsOfClusters()
        {
            return PairsOfClusters(this);
        }


        public static IEnumerable<Tuple<Cluster, Cluster, ClusterRelativePosition>> PairsOfClusters(ClusterCollection clusts)
        {
            List<Location> offsets = new List<Location>()
                                         {
                                             new Location(-1,0),
                                             new Location(0,-1),
                                             new Location(1,0),
                                             new Location(0,1),
                                         };

            Dictionary<Location, ClusterRelativePosition> relativeMappings = new Dictionary<Location, ClusterRelativePosition>()
                                                                                 {
                                                                                     { offsets[0], ClusterRelativePosition.Left },
                                                                                     { offsets[1], ClusterRelativePosition.Up },
                                                                                     { offsets[2], ClusterRelativePosition.Right },
                                                                                     { offsets[3], ClusterRelativePosition.Down },
                                                                                 };

            var pairs = new List<Tuple<Cluster, Cluster, ClusterRelativePosition>>();

            var wrap = new Location(clusts.ClusterCount, clusts.ClusterCount);
            for (int col = 0; col < clusts.ClusterCount; col++)
            {
                for (int row = (col % 2 == 0) ? 0 : 1; row < clusts.ClusterCount; row += 2)
                {
                    var loc = new Location(col, row);

                    foreach (var offset in offsets)
                    {
                        var loc2 = new Location(col + offset.Col, row + offset.Row).Wrap(wrap);
                        pairs.Add(new Tuple<Cluster, Cluster, ClusterRelativePosition>(clusts[loc], clusts[loc2], relativeMappings[offset]));

                    }
                }
            }

            return pairs;
        }

        public Cluster LookupCluster(Location location)
        {
            int col = location.Col/ClusterSizes.Item1.Col;
            int row = location.Row/ClusterSizes.Item1.Row;
            var clusterIndex = new Location(col == ClusterCount ? col -1 : col, row == ClusterCount ? row -1 : row);


            return clusters[clusterIndex.Col][clusterIndex.Row];
        }
    }
}
