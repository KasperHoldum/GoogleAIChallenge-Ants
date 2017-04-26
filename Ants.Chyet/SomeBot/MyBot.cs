using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ants.Chyet.SomeBot
{
    class MyBot : Bot
    {
        public MyBot()
            : base("Chyet1")
        {
        }

        public override void DoTurn(GameState gameState)
        {
            this.Update(gameState);
            var foods = gameState.FoodTiles.ToList();

            foreach (var antLoc in gameState.MyAnts)
            {
                Location to = null;
                foreach (Location food in foods)
                {
                    List<Location> path = this.PathFinding.FindPath(antLoc.ToLoc(), food);

                    if (path.Count != 0)
                    {
                        to = path[1];
                        continue;
                    }
                }

                if (to != null)
                {
                    var direction = gameState.Direction(antLoc, to).ToList()[0];
                    IssueOrder(antLoc, direction);
                    continue;
                }
            }
        }
    }
}
