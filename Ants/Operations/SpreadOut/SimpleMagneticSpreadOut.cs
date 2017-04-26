using System;
using System.Collections.Generic;
using System.Linq;
using Ants.DataStructures.BinarySpacePartitioning;

namespace Ants.Operations.SpreadOut
{
    /// <summary>
    /// Uses the concept of magnetic powers to spread out ants
    /// </summary>
    public class SimpleMagneticSpreadOut : AntOperation
    {
        public SimpleMagneticSpreadOut(Bot bot)
            : base(bot)
        {
        }

        public override void ExecuteOperation(List<AntLoc> availableAnts)
        {
            var myAntsTree = new KdTree<AntLoc>(this.Bot.State.DonutDistances, this.Bot.State.MyAnts);
            IEnumerable<AntLoc> inactiveAnts = this.Bot.State.MyAnts.Except(this.Bot.HasAntMoved);

            const int power = 10;
            var radius = Math.Sqrt(this.Bot.State.ViewRadius2) * 2;

            foreach (var antLoc in inactiveAnts)
            {
                var finalDirection = new Location(0, 0);

                var foundAnts = myAntsTree.FindNodesInRange(antLoc, Math.Pow(radius, 2));


                foreach (var foundAnt in foundAnts)
                {
                    var distance = this.Bot.State.DonutDistances.Distance(foundAnt, antLoc);

                    var scaleFactor = (int)(power / distance);

                    var antToAntVector = antLoc - foundAnt;
                    var direction = antToAntVector * scaleFactor;

                    finalDirection += direction;
                }

                var targetDestination = new Location((antLoc.Col + Math.Max(finalDirection.Col, -this.Bot.State.Width) + this.Bot.State.Width) % this.Bot.State.Width,
                                                     (antLoc.Row + Math.Max(finalDirection.Row, -this.Bot.State.Height) + this.Bot.State.Height) % this.Bot.State.Height);

                if (targetDestination == antLoc)
                    continue;

                List<Location> fullPath = this.Bot.PathFinding.FindPath(antLoc, targetDestination);

                // this should not happen either, as a path was found on line 88 using FindClosestEntity
                if (fullPath == null || fullPath.Count == 1)
                {
                    // unreachable item or we are already next to it
                    this.Bot.Log.Log("Could not find path between " + antLoc + " and " + finalDirection);
                    continue;
                }

                Location nextStep = fullPath[1];

                var isSafeResult = Bot.IsSafe(antLoc, nextStep);
                if (isSafeResult.Item1)
                {
                    if (!Bot.MoveAnt(antLoc, nextStep))
                    {
                    }
                }
            }

        }
    }
}
