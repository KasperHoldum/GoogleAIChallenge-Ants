using System.Collections.Generic;
using System.Linq;

namespace Ants.Operations.FindFood
{
    /// <summary>
    /// Uses stable marriage algorithm to match ants to food
    /// </summary>
    public class StableMarriageFindFood : AntOperation
    {
        public StableMarriageFindFood(Bot bot)
            : base(bot)
        {
        }

        public override void ExecuteOperation(List<AntLoc> availableAnts)
        {
            List<Location> foodList = Bot.State.FoodTiles.ToList();

            if (foodList.Count == 0)
                return;
            var freeAnts = Bot.AvailableAnts();

            // initialize preferences which are the distance between ant and food
            var preferences = new Dictionary<DataStructures.Tuple<Location, AntLoc>, double>();
            foodList.ForEach(l => freeAnts.ForEach(ant => preferences[new DataStructures.Tuple<Location, AntLoc>(l, ant)] = Bot.State.DonutDistances.ManhattenDistance(l, ant)));

            // initialize the list of already checked food
            var foodProposedTo = new Dictionary<AntLoc, List<Location>>();
            freeAnts.ForEach(a => foodProposedTo[a] = new List<Location>());

            var engagedCouples = new List<DataStructures.Tuple<Location, AntLoc>>();

            while (freeAnts.Count() > 0)
            {
                // select a free ant that hasn't proposed to all food
                var freeAnt = freeAnts.First();

                // select highest rated food that ant hasn't proposed to
                var highestRankedFood = preferences.Where(s => s.Key.Item2 == freeAnt).Where(s => !foodProposedTo[freeAnt].Contains(s.Key.Item1)).OrderBy(s => s.Value).First().Key.Item1;

                var freeAntAndFood = new DataStructures.Tuple<Location, AntLoc>(highestRankedFood, freeAnt);
                if (!engagedCouples.Any(s => s.Item1 == highestRankedFood))
                {
                    engagedCouples.Add(freeAntAndFood);
                    freeAnts.Remove(freeAnt);
                }
                else
                {
                    var currentPartner = engagedCouples.First(s => s.Item1 == highestRankedFood).Item2;
                    if (preferences[new DataStructures.Tuple<Location, AntLoc>(highestRankedFood, currentPartner)] < preferences[freeAntAndFood])
                    {
                        // ant remains free
                    }
                    else
                    {
                        engagedCouples.Remove(engagedCouples.First(s => s.Item1 == highestRankedFood));
                        if (foodProposedTo[currentPartner].Count != foodList.Count)
                            freeAnts.Add(currentPartner);
                        engagedCouples.Add(freeAntAndFood);
                        freeAnts.Remove(freeAnt);
                    }
                }

                foodProposedTo[freeAnt].Add(highestRankedFood);

                if (foodProposedTo[freeAnt].Count == foodList.Count)
                    freeAnts.Remove(freeAnt);
            }

            foreach (var engagedCouple in engagedCouples)
            {
                var ant = engagedCouple.Item2;
                var food = engagedCouple.Item1;

                var fullPath = Bot.PathFinding.FindPath(ant, food);

                // this should not happen either, as a path was found on line 88 using FindClosestEntity
                if (fullPath == null || fullPath.IsFinished)
                {
                    // unreachable item or we are already next to it
                    Bot.Log.Log("Could not find path between " + ant + " and " + food);
                    continue;
                }


                Location nextStep = fullPath[1];

                //ignore safety to get food!
                //IsSafe(ant, nextStep);

                bool wasMoved = Bot.MoveAnt(ant, nextStep);

                Bot.HasAntMoved[ant] = wasMoved;
            }

        }
    }
}
