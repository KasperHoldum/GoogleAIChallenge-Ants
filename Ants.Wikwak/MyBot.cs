using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ants.Wikwak
{
    public class MyBot : Bot
    {
        public MyBot()
            : base("WikwakBot")
        {
        }

        public override void DoTurn(GameState gameState)
        {
            foreach (var antLoc in gameState.MyAnts)
            {
                this.IssueOrder(antLoc, (char)DirectionExtensions.GetRandomDirection());
            }
        }
    }
}
