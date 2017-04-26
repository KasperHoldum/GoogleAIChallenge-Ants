using System.Collections.Generic;
using System.Linq;

namespace Ants.Qua.TestBot
{
    public class MyBot : Bot
    {
        public MyBot() : base("TestBot")
        {
        }

        /// <summary>
        /// New method that iterates over food and find nearest ant.
        /// </summary>
        /// <param name="gameState"></param>
        public override void DoTurn(GameState gameState)
        {
            Log.Log("-- New Turn --");
            var pathFinding = new AStarPathFinding(gameState);

            var destinations = new List<Location>();
            List<Location> food = gameState.FoodTiles.ToList();

            Dictionary<AntLoc, bool> antSchedule = gameState.MyAnts.ToDictionary(a => a, a => false);
            foreach (Location foodLocation in food)
            {
                AntLoc closestAnt = pathFinding.FindClosestEntity(foodLocation,
                                                                  gameState.MyAnts.Except(
                                                                      antSchedule.Where(a => a.Value).Select(k => k.Key))
                                                                      .ToDictionary(a => new Location(a.Col, a.Row),
                                                                                    a => a));

                if (closestAnt == null)
                {
                    // there was no path to the food.
                    Log.Log("Could not access food located @ " + foodLocation);
                    continue;
                }

                // this should no longerhappen
                //if (antSchedule[closestAnt])
                //{
                //    Logging.Log("The ant," + closestAnt + " closest to the food " + foodLocation + " was busy. Skipping food.");
                //    continue;
                //}


                List<Location> fullPath = pathFinding.FindPath(closestAnt, foodLocation);

                // this should not happen either, as a path was found on line 88 using FindClosestEntity
                if (fullPath.Count == 0 || fullPath.Count == 1)
                {
                    // unreachable item or we are already next to it
                    Log.Log("Could not find path between " + closestAnt + " and " + foodLocation);
                    continue;
                }


                Location nextStep = fullPath[1];
                bool wasMoved = MoveAnt(gameState, nextStep, destinations, closestAnt);

                antSchedule[closestAnt] = wasMoved;
            }

            IEnumerable<AntLoc> idleAnts = antSchedule.Where(kv => !kv.Value).Select(kv => kv.Key);

            foreach (AntLoc idleAnt in idleAnts)
            {
                Log.Log("Ant " + idleAnt + " was idle and is moved randomly.");
                var directions = new[] {AntsParser.North, AntsParser.East, AntsParser.South, AntsParser.West};

                foreach (Location direction in directions)
                {
                    var nextStep = new Location(idleAnt.Col + direction.Col, idleAnt.Row+ direction.Row);
                    bool antWasMoved = MoveAnt(gameState, nextStep, destinations, idleAnt);

                    if (antWasMoved)
                    {
                        break; // only move ant in a single direction
                    }
                }
            }
        }

        private bool MoveAnt(GameState state, Location nextStep, List<Location> destinations, AntLoc ant)
        {
            bool issuedOrder = false;
            Location desti = state.Destination(ant, state.Direction(ant, nextStep).First());
            if (state.IsUnoccupied(desti) && !destinations.Any(l => l.Col == desti.Col && l.Row == desti.Row))
            {
                destinations.Add(desti);

                IssueOrder(ant, state.Direction(ant, nextStep).First());
                issuedOrder = true;
            }

            // ant is not moving, so mark location as occupied
            if (!issuedOrder)
            {
                destinations.Add(ant);
            }

            return issuedOrder;
        }
    }
}