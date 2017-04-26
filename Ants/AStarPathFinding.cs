using System;
using System.Collections.Generic;
using System.Linq;
using Ants.DataStructures;

namespace Ants
{
    public class AStarPathFinding : PathFinding
    {
        public AStarPathFinding(GameState gameState)
            : base(gameState)
        {


        }

        public override Path FindPath(Location start, Location goal, bool canMoveOnAntsAndFood = false, double distanceToGoalEpsilon = 1.0000001)
        {
            if (start == goal)
                return new Path(start);

            var cameFrom = new Dictionary<Location, Location>(); // The map of navigated nodes.

            // The set of tentative nodes to be evaluated, initially containing the start node
            var gScore = new Dictionary<Location, double>(); // Cost from start along best known path.
            var hScore = new Dictionary<Location, double>();
            var fScore = new Dictionary<Location, double>(); // Estimated total cost from start to goal through y.

            var unevaluatedCheck = new Dictionary<Location, bool>();
            var evaluatedNodes = new Dictionary<Location, bool>(); // The set of nodes already evaluated.
            var unevaluatedNodes = new SortList<Location>(new DistanceComparer<Location>(goal, GameState, gScore)) { start };

            gScore[start] = 0;
            hScore[start] = HeuristicCostEstimate(start, goal);
            fScore[start] = hScore[start];

            while (unevaluatedNodes.Count > 0)
            {
                Location x = unevaluatedNodes.First();

                if (x == goal || DistanceBetween(x, goal) < distanceToGoalEpsilon)
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
                foreach (Location y in DemNeighbours[x])
                {
                    if (canMoveOnAntsAndFood)
                    {
                        if (!GameState.IsWalkable(y))
                        {
                            continue;
                        }
                    }
                    else if (!GameState.IsUnoccupied(y))
                    {
                        continue;
                    }


                    // if the neighbours is already evaluated then skip it
                    if (evaluatedNodes.ContainsKey(y))
                        continue;

                    // compute the total path from start to the neighbour
                    double tentativeGScore = gScore[x] + 1; //DistanceBetween(x, y);

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
            return null;
        }

        private int DistanceBetween(Location location, Location location1)
        {
            return GameState.DonutDistances.ManhattenDistance(location, location1);
        }

        private static Path ReconstructPath(IDictionary<Location, Location> cameFrom, Location currentNode)
        {
            if (!cameFrom.ContainsKey(currentNode))
            {
                return new Path(currentNode);
            }


            Path previousPath = ReconstructPath(cameFrom, cameFrom[currentNode]);
            previousPath.Add(currentNode);
            return previousPath;
        }

        private double HeuristicCostEstimate(Location start, Location goal)
        {
            return DistanceBetween(start, goal);
        }

        #region Nested type: DistanceComparer

        public class DistanceComparer<T> : IComparer<T> where T : Location
        {
            private readonly T goal;
            private readonly GameState state;
            private readonly Dictionary<T, double> gScore;

            public DistanceComparer(T goal, GameState state, Dictionary<T, double> gScore)
            {
                this.goal = goal;
                this.state = state;
                this.gScore = gScore;
            }

            #region IComparer<Location> Members

            public int Compare(T x, T y)
            {
                double distanceX = state.DonutDistances.ManhattenDistance(x, goal) + gScore[x];
                double distanceY = state.DonutDistances.ManhattenDistance(y, goal) + gScore[y];

                return distanceX.CompareTo(distanceY);
            }

            #endregion
        }

        #endregion
    }
}