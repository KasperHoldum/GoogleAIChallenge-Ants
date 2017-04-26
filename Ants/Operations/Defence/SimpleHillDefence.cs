using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ants.DataStructures.BinarySpacePartitioning;

namespace Ants.Operations.Defence
{
    public class SimpleHillDefence : AntOperation
    {
        //private int criticalZone = 10;
        private const int DangerousZone = 10;


        public SimpleHillDefence(Bot bot)
            : base(bot)
        {
        }

        public override void ExecuteOperation(List<AntLoc> availableAnts)
        {
            var availableAntsTree = new KdTree<AntLoc>(Bot.State.DonutDistances, availableAnts.ToList());

            foreach (var myHill in this.Bot.State.MyHills)
            {
                var dangerousAnts = this.Bot.Enemies.FindNodesInManhattenRange(myHill, DangerousZone);
                foreach (var dangerousAnt in dangerousAnts)
                {
                    if (availableAntsTree.Root == null)
                        return;

                    var nearest = availableAntsTree.FindNearestNeighbour(myHill).Item1;

                    //if (this.Bot.State.DonutDistances.ManhattenDistance(dangerousAnt, myHill) <= criticalZone)
                    //{
                    //    this.Bot.PathFindMove(nearest, dangerousAnt);

                    //}
                    //else
                    //{
                        this.Bot.PathFindMove(nearest, myHill);

                    //}
                    availableAntsTree.Remove(nearest);
                }
            }
        }
    }
}
