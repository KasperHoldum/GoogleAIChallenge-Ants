using System.Collections.Generic;
using System.Linq;

namespace Ants.DataStructures.HPA
{
    public class HierarchicalPathFindingAStar : PathFinding
    {
        private readonly AStarPathFinding lowLevelPathFinding;
        private readonly ClusterCollection clusterCollection;

        public HierarchicalPathFindingAStar(GameState gameState, int clusterCount)
            : base(gameState)
        {
            lowLevelPathFinding = new AStarPathFinding(gameState);

            clusterCollection = new ClusterCollection(gameState, clusterCount);
            clusterCollection.Initialize(lowLevelPathFinding);
        }

        public override Path FindPath(Location start, Location goal, bool canMoveOnAntsAndFood = false, double distanceToGoalEpsilon = 1.0000001)
        {
            if (start == goal)
                return new Path(start);

            // look up cluster
            Cluster startCloster = clusterCollection.LookupCluster(start);
            Cluster endCloster = clusterCollection.LookupCluster(goal);

            var startTransit = startCloster.AddTemporaryInternalNode(start, this.lowLevelPathFinding);
            var endTransit = endCloster.AddTemporaryInternalNode(goal, this.lowLevelPathFinding);

            List<TransitNode> path = FindShortestPath(startTransit.Item1, endTransit.Item1);

            Path result = null;
            if (path.Count > 0)
            {
                TransitNode startNode = path[0];
                var between = new DataStructures.Tuple<TransitNode, TransitNode>(startNode, path[1]);

                result = clusterCollection.LookupCluster(startNode).HasCachedPath(between) ? clusterCollection.LookupCluster(startNode).GetCachePath(between) : lowLevelPathFinding.FindPath(startNode, path[1]);
            }

            if (startTransit.Item2) startCloster.RemoveTemporaryInternalNode(start);
            if (endTransit.Item2) endCloster.RemoveTemporaryInternalNode(goal);

            return result;
        }

        private List<TransitNode> FindShortestPath(TransitNode start, TransitNode goal)
        {
            if (start == goal)
                return new List<TransitNode>();

            var cameFrom = new Dictionary<TransitNode, TransitNode>(); // The map of navigated nodes.

            // The set of tentative nodes to be evaluated, initially containing the start node
            var gScore = new Dictionary<TransitNode, double>(); // Cost from start along best known path.
            var hScore = new Dictionary<TransitNode, double>();
            var fScore = new Dictionary<TransitNode, double>(); // Estimated total cost from start to goal through y.

            var unevaluatedCheck = new Dictionary<TransitNode, bool>();
            var evaluatedNodes = new Dictionary<TransitNode, bool>(); // The set of nodes already evaluated.
            var unevaluatedNodes = new SortList<TransitNode>(new AStarPathFinding.DistanceComparer<TransitNode>(goal, GameState, gScore)) { start };

            gScore[start] = 0;
            hScore[start] = HeuristicCostEstimate(start, goal);
            fScore[start] = hScore[start];

            while (unevaluatedNodes.Count > 0)
            {
                TransitNode x = unevaluatedNodes.First();

                if (x == goal || GameState.DonutDistances.SquaredDistance(x, goal) < 1.0000001)
                {
                    if (x != goal)
                    {
                        // we are right besides goal
                        cameFrom[goal] = x;
                    }
                    return ReconstructPath(cameFrom, goal);
                }

                unevaluatedCheck[x] = false;
                unevaluatedNodes.RemoveAt(0);
                evaluatedNodes[x] = true;

                // iterate over all neighbours
                foreach (TransitNode y in x.Edges)
                {
                    // if the neighbours is already evaluated then skip it
                    if (evaluatedNodes.ContainsKey(y))
                        continue;

                    // compute the total path from start to the neighbour
                    double tentativeGScore = gScore[x] + y.DistanceTo(x); //DistanceBetween(x, y);

                    bool isTentativeBetter = false;
                    bool firstTimeSeeingNode = false;


                    // check if we never saw this node before
                    if (!unevaluatedCheck.ContainsKey(y) || unevaluatedCheck[y] == false)
                    {
                        isTentativeBetter = true;
                        firstTimeSeeingNode = true;
                    }
                    else if (tentativeGScore < gScore[y]) // else check if this route is better than the previous
                        isTentativeBetter = true;

                    // if the path along x to y was better than previous then update
                    if (isTentativeBetter)
                    {

                        cameFrom[y] = x;
                        gScore[y] = tentativeGScore;
                        hScore[y] = HeuristicCostEstimate(y, goal);
                        fScore[y] = gScore[y] + hScore[y];

                        if (firstTimeSeeingNode)
                        {
                            unevaluatedNodes.Add(y);
                            unevaluatedCheck[y] = true;
                        }
                    }
                }
            }
            return new List<TransitNode>();
        }

        private static List<TransitNode> ReconstructPath(Dictionary<TransitNode, TransitNode> cameFrom, TransitNode currentNode)
        {
            if (!cameFrom.ContainsKey(currentNode))
            {
                return new List<TransitNode>() { currentNode };
            }

            List<TransitNode> previousPath = ReconstructPath(cameFrom, cameFrom[currentNode]);
            previousPath.Add(currentNode);
            return previousPath;
        }

        private double HeuristicCostEstimate(TransitNode p0, TransitNode goal)
        {
            return this.GameState.DonutDistances.SquaredDistance(p0, goal);
        }
    }
}
