using System;
using System.Collections.Generic;
using System.Linq;
using Ants.DataStructures.BinarySpacePartitioning;
    
namespace Ants.Operations.SpreadOut
{
    /// <summary>
    /// Makes each ant move to the closest invisible spot that we haven't seen in the longest time.
    /// </summary>
    public class VisibilitySpreadOut : AntOperation
    {
        public VisibilitySpreadOut(Bot myBot)
            : base(myBot)
        {
        }

        public override void ExecuteOperation(List<AntLoc> availableAnts)
        {
            // added fix so that it doesn't crash if the tree doesn't contain any invisible spots.
            if (Bot.State.Visibility.InvisibleSpotsTree.Root == null)
                return;

            var inactiveActs = availableAnts.ToList();

            AntRegistry.RemoveUnusedData(inactiveActs.Cast<Location>(), "spreadoutPath");

            var invisibleSpotsTree = Bot.State.Visibility.InvisibleSpotsTree;
            foreach (var inactiveAnt in inactiveActs.Take(80))
            {
                Location nextStep;

                var data = AntRegistry.GetData(inactiveAnt, "spreadoutPath") as DataStructures.Tuple<int, List<Location>>;
                if (data != null && ValidatePath(data.Item2))
                {
                    nextStep = data.Item2[0];
                    var enumerable = data.Item2.Skip(1).ToList();

                    if (enumerable.Count > 1)
                        AntRegistry.AddData(inactiveAnt, "spreadoutPath", new DataStructures.Tuple<int, List<Location>>(this.Bot.State.Turn, enumerable));
                    else
                        AntRegistry.RemoveData(inactiveAnt, "spreadoutPath");
                }
                else
                {
                    if (data != null)
                    {
                        AntRegistry.RemoveData(inactiveAnt, "spreadoutPath");
                    }

                    var closestSpot = FindSpotToExplore(invisibleSpotsTree, inactiveAnt);

                    if (closestSpot == null)
                    {
                        continue;
                    }

                    Path fullPath = Bot.PathFinding.FindPath(inactiveAnt, closestSpot, false, 3);

                    // this should not happen either, as a path was found on line 88 using FindClosestEntity
                    if (fullPath == null || fullPath.IsFinished)
                    {
                        // unreachable item or we are already next to it
                        Bot.Log.Log("Could not find path between " + inactiveAnt + " and " + closestSpot);
                        continue;
                    }
                    nextStep = fullPath.NextLocation;

                    Path restOfPath = fullPath.NextPath();

                    if (!restOfPath.IsFinished)
                    {
                        AntRegistry.AddData(inactiveAnt, "spreadoutPath", new DataStructures.Tuple<int, Path>(this.Bot.State.Turn, restOfPath));
                    }
                }

#if DEBUG
                if (this.Bot.State.DonutDistances.ManhattenDistance(inactiveAnt, nextStep) > 1)
                {
                    throw new ArgumentException("An invalid order was made. Move from " + inactiveAnt + " to " + nextStep + " was invalid", "nextStep");
                }
#endif


                var isSafeResult = Bot.IsSafe(inactiveAnt, nextStep);
                if (isSafeResult.Item1)
                {
                    if (!Bot.MoveAnt(inactiveAnt, nextStep))
                    {
                        AntRegistry.RemoveData(inactiveAnt, "spreadoutPath");
                    }
                }
                else
                {
                    AntRegistry.RemoveData(isSafeResult.Item2 ?? inactiveAnt, "spreadoutPath");
                }
            }
        }

        private Location FindSpotToExplore(KdTree<Location> invisibleSpotsTree, AntLoc inactiveAnt)
        {
//            IEnumerable<Location> findNodesInManhattenRange = invisibleSpotsTree.FindNodesInManhattenRange(inactiveAnt,this.Bot.State.ViewRadius * 4);
//            var spotToExplore = findNodesInManhattenRange.
//                    Where(l => this.Bot.State[l.Col, l.Row] != Tile.Water && Bot.State.Visibility.TurnsSinceLastSeen(l) > 5).FirstOrDefault();

//#if DEBUG
//            if (spotToExplore == inactiveAnt)
//                throw new InvalidProgramException("Visibility issue! ");
//#endif
            //return spotToExplore;


            var closestInvisibleSpot = invisibleSpotsTree.FindNearestNeighbour(inactiveAnt);
            var distanceToClosest = closestInvisibleSpot.Item2;

            var allClosest = invisibleSpotsTree.FindNodesInManhattenRange(inactiveAnt, (int) distanceToClosest);
            var closestSpot = allClosest.OrderByDescending(s => Bot.State.Visibility.TurnsSinceLastSeen(s)).FirstOrDefault();
            return closestSpot;
        }

        private bool ValidatePath(IEnumerable<Location> item2)
        {
            return item2.All(l => this.Bot.State.IsWalkable(l));
        }
    }
}
