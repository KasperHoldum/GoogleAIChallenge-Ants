using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Ants.Qua.ImprovedFindFood;

namespace Ants.Qua.Timeout
{
    public class MyBot : Bot
    {
        private readonly ImprovedFindFood.MyBot bot = new ImprovedFindFood.MyBot();

        public MyBot() : base("Timeout Bot")
        {

        }


        public override void DoTurn(GameState gameState)
        {
            if (gameState.Turn >= 50)
            {
                Thread.Sleep(5000);
            }

            bot.DoTurn(gameState);



        }
    }
}
