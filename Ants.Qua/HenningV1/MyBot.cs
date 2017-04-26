using System;
using System.Collections.Generic;
using System.Linq;
using Ants.DataStructures;
using Ants.DataStructures.BinarySpacePartitioning;

namespace Ants.Qua.HenningV1
{
    public class MyBot : Bot
    {
        private readonly Logging timeLogging = new Logging("HenningV1 time");

        public MyBot() : base("HenningV1")
        {
        }

        /// <summary>
        /// New method that iterates over food and find nearest ant.
        /// </summary>
        /// <param name="gameState"></param>
        public override void DoTurn(GameState gameState)
        {
            Log.Log("Starting turn " + gameState.Turn);
            timeLogging.Log(string.Format("[{0}] turn: {1}", DateTime.Now, gameState.Turn));

            DateTime before = DateTime.Now;
            Update(gameState);
            timeLogging.Log(string.Format("[{0} ms] update finished", (int)(DateTime.Now - before).TotalMilliseconds));

            before = DateTime.Now;
            FindFood();
            timeLogging.Log(string.Format("[{0} ms] findfood finished", (int)(DateTime.Now - before).TotalMilliseconds));


            before = DateTime.Now;
            SpreadOut();
            timeLogging.Log(string.Format("[{0} ms] spreadout finished", (int)(DateTime.Now - before).TotalMilliseconds));
        }

        private bool IsAttackMode()
        {
            return false;
        }

      
        private void SpreadOut()
        {
            var inactiveActs = this.State.MyAnts.Except(HasAntMoved);
            var tree = State.Visibility.InvisibleSpotsTree;

            foreach (var inactiveAnt in inactiveActs)
            {
                var closestInvisibleSpot = tree.FindNearestNeighbour(inactiveAnt);
                var distanceToClosest = closestInvisibleSpot.Item2;

                var allClosest = tree.FindNodesInRange(inactiveAnt, distanceToClosest);
                var closestSpot = allClosest.OrderByDescending(s => this.State.Visibility.TurnsSinceLastSeen(s)).First();

                if (closestSpot == null)
                {
                    // there are no more invisble spots
                    return;
                }

                List<Location> fullPath = PathFinding.FindPath(inactiveAnt, closestSpot);
                // this should not happen either, as a path was found on line 88 using FindClosestEntity
                if (fullPath.Count == 0 || fullPath.Count == 1)
                {
                    // unreachable item or we are already next to it
                    Log.Log("Could not find path between " + inactiveAnt + " and " + closestSpot);
                    continue;
                }


                Location nextStep = fullPath[1];

                SafetyCheck(inactiveAnt, nextStep);

                bool wasMoved = MoveAnt(inactiveAnt, nextStep);

                HasAntMoved[inactiveAnt] = wasMoved;

                this.State.Visibility.UpdateAntMove(inactiveAnt,nextStep);
            }
        }

        private void FindFood()
        {
            List<Location> food = State.FoodTiles.ToList();

            foreach (Location foodLocation in food)
            {
                var nonIdleAnts = State.MyAnts.Except(HasAntMoved).ToList();

                if (nonIdleAnts.Count() == 0)
                {
                    Log.Log("Stopping find food algorithm: No more idle ants.");
                    break;
                }

                AntLoc closestAnt = PathFinding.FindClosestEntity(foodLocation,
                                                                  nonIdleAnts.ToDictionary(a => new Location(a.Col, a.Row),a => a));

                if (closestAnt == null)
                {
                    // there was no path to the food.
                    Log.Log("No path found between food located @ " + foodLocation + " and closest ant");
                    continue;
                }

                List<Location> fullPath = PathFinding.FindPath(closestAnt, foodLocation);

                // this should not happen either, as a path was found on line 88 using FindClosestEntity
                if (fullPath.Count == 0 || fullPath.Count == 1)
                {
                    // unreachable item or we are already next to it
                    Log.Log("Could not find path between " + closestAnt + " and " + foodLocation);
                    continue;
                }


                Location nextStep = fullPath[1];

                SafetyCheck(closestAnt, nextStep);

                bool wasMoved = MoveAnt(closestAnt, nextStep);

                HasAntMoved[closestAnt] = wasMoved;
            }
        }

        private void SafetyCheck(AntLoc closestAnt, Location nextStep)
        {
        }
    }
}