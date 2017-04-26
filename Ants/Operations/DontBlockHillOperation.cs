using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ants.DataStructures.BinarySpacePartitioning;

namespace Ants.Operations
{
    public class DontBlockHillOperation : AntOperation
    {
        public DontBlockHillOperation(Bot bot)
            : base(bot)
        {
        }

        public override void ExecuteOperation(List<AntLoc> availableAnts)
        {
            var myants = new KdTree<AntLoc>(this.Bot.State.DonutDistances, this.Bot.State.MyAnts);

            foreach (Location myHill in this.Bot.State.MyHills)
            {
                var antsHere = myants.FindNodesInManhattenRange(myHill, 0).FirstOrDefault();

                if (antsHere != null && !this.Bot.HasAntMoved[antsHere])
                {

                    foreach (var f in AntsParser.Aim)
                    {
                        if (this.Bot.MoveAnt(antsHere, antsHere + f.Value))
                        {
                            break;
                        }
                    }
                }

            }
        }
    }
}
