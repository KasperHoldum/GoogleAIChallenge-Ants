using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ants.DataStructures.BinarySpacePartitioning;

namespace Ants.Operations.SpreadOut
{
    public class NaiveMagnetSpreadOut : AntOperation
    {
        public NaiveMagnetSpreadOut(Bot bot)
            : base(bot)
        {

        }

        public override void ExecuteOperation(List<AntLoc> availableAnts)
        {
            var myAntsTree = new KdTree<AntLoc>(this.Bot.State.DonutDistances, this.Bot.State.MyAnts);
            IEnumerable<AntLoc> inactiveAnts = availableAnts;

            const int power = 10;
            var radius = this.Bot.State.ViewRadius * 1.5;

            foreach (var antLoc in inactiveAnts)
            {
                var finalDirection = new Location(0, 0);

                var foundAnts = myAntsTree.FindNodesInRange(antLoc, Math.Pow(radius, 2));


                foreach (var foundAnt in foundAnts)
                {
                    var distance = this.Bot.State.DonutDistances.Distance(foundAnt, antLoc);

                    var scaleFactor = (int)(power / (distance *2));

                    var antToAntVector = antLoc - foundAnt;
                    var direction = antToAntVector * scaleFactor;

                    finalDirection += direction;
                }

                var targetDestination = new Location((antLoc.Col + Math.Max(finalDirection.Col, -this.Bot.State.Width) + this.Bot.State.Width) % this.Bot.State.Width,
                                                     (antLoc.Row + Math.Max(finalDirection.Row, -this.Bot.State.Height) + this.Bot.State.Height) % this.Bot.State.Height);

                if (targetDestination == antLoc)
                    continue;

                ICollection<char> targetDirections = this.Bot.State.Direction(antLoc, targetDestination);

                var nextStep  = this.Bot.State.Destination(antLoc, targetDirections.OrderBy(c => Guid.NewGuid()).First());

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
