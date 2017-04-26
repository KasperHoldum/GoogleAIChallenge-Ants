using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ants.DataStructures.BinarySpacePartitioning;

namespace Ants.Operations.Attack
{
    public class SimpleCaptureHill : AntOperation
    {
        public SimpleCaptureHill(Bot bot)
            : base(bot)
        {
        }

        public override void ExecuteOperation(List<AntLoc> availableAnts)
        {
            var inactiveAnts = Bot.AvailableAnts();
            var enemyHills = this.Bot.State.EnemyHills;
            var enemyHillsTree = new KdTree<Location>(this.Bot.State.DonutDistances, enemyHills);

            var initalInactiveAntCount = inactiveAnts.Count;
            var viewFactor = Math.Max((int) (initalInactiveAntCount/50), 1);
            foreach (AntLoc inactiveAnt in inactiveAnts)
            {
                var closestEnemyHill = enemyHillsTree.FindNodesInRange(inactiveAnt, this.Bot.State.ViewRadius2 * viewFactor).FirstOrDefault();

                if (closestEnemyHill != null)
                {
                    var fullPath = this.Bot.PathFinding.FindPath(inactiveAnt, closestEnemyHill);

                    // this should not happen either, as a path was found on line 88 using FindClosestEntity
                    if (fullPath == null  || fullPath.IsFinished)
                    {
                        // unreachable item or we are already next to it
                        this.Bot.Log.Log("Could not find path between " + inactiveAnt + " and " + closestEnemyHill);
                        continue;
                    }


                    Location nextStep = fullPath[1];

                    if (initalInactiveAntCount > 50)
                    {
                        if (this.Bot.IsSafe(inactiveAnt, nextStep).Item1)
                        {
                            this.Bot.MoveAnt(inactiveAnt, nextStep);
                        }
                    }
                    else
                    {
                        this.Bot.MoveAnt(inactiveAnt, nextStep);
                    }
                }
            }
        }
    }
}
