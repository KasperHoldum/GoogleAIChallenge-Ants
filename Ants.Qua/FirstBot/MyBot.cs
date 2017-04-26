using System;
using System.Collections.Generic;
using System.Linq;

namespace Ants.Qua.FirstBot
{
    public class MyBot : Bot
    {
        public MyBot() : base("FirstBot")
        {
        }

        /// <summary>
        /// Old method that for each ant find nearest food and moves towards that.
        /// </summary>
        /// <param name="gameState"></param>
        public override void DoTurn(GameState gameState)
        {
            var pathFinding = new AStarPathFinding(gameState);

            var destinations = new List<Location>();
            List<Location> enemiesAndFood = gameState.FoodTiles.ToList();

            foreach (AntLoc ant in gameState.MyAnts.OrderBy(s => Guid.NewGuid()).Take(20))
            {
                Location closestTarget = null;
                double closestDistance = double.MaxValue;

                foreach (Location target in enemiesAndFood.OrderBy(s => Guid.NewGuid()).Take(30))
                {
                    double distance = gameState.DonutDistances.Distance(ant, target);
                    List<Location> kek = pathFinding.FindPath(ant, target);
                    if (distance < closestDistance && kek.Count >= 1)
                    {
                        closestDistance = distance;
                        closestTarget = target;
                    }
                }

                if (closestTarget == null)
                {
                    // no target found, mark ant as not moving so we don't run into it
                    Log.Log("No food found for ant " + ant);
                    continue;
                }
                bool issuedOrder = false;
                List<Location> fullPath = pathFinding.FindPath(ant, closestTarget);

                if (fullPath.Count == 0 || fullPath.Count == 1)
                {
                    // unreachable item or we are already next to it
                    continue;
                }

                Location nextStep = fullPath[1];

                Location desti = gameState.Destination(ant, gameState.Direction(ant, nextStep).First());
                if (gameState.IsUnoccupied(desti) && !destinations.Any(l => l.Col == desti.Col && l.Row == desti.Row))
                {
                    destinations.Add(desti);

                    IssueOrder(ant, gameState.Direction(ant, nextStep).First());
                    issuedOrder = true;
                }

                // ant is not moving, so mark location as occupied
                if (!issuedOrder)
                {
                    destinations.Add(ant);
                }
            }
        }
    }
}