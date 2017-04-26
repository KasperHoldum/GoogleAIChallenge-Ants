using System;
using System.Collections.Generic;
using System.Linq;
using Ants.DataStructures.BinarySpacePartitioning;
using Ants.Operations.FindFood;
using Ants.Operations.SpreadOut;

namespace Ants.Qua.HenningStableMatching
{
    public class MyBot : Bot
    {
        private readonly Logging timeLogging = new Logging("HenningStableMatching time");
        private readonly AntOperation spreadOutOperation;
        private readonly AntOperation findFoodOperation;
        public MyBot()
            : base("HenningStableMatching")
        {
            spreadOutOperation = new VisibilitySpreadOut(this);
            findFoodOperation = new StableMarriageFindFood(this);

            this.Operations.Add(findFoodOperation);
            this.Operations.Add(spreadOutOperation);
        }

        /// <summary>
        /// New method that iterates over food and find nearest ant.
        /// </summary>
        /// <param name="gameState"></param>
        public override void DoTurn(GameState gameState)
        {
            Log.Log("Starting turn " + gameState.Turn);
            timeLogging.Log(string.Format("[{0}] turn: {1}", DateTime.Now, gameState.Turn));

            DateTime before = DateTime.Now;
            Update(gameState);
            timeLogging.Log(string.Format("[{0} ms] update finished", (int)(DateTime.Now - before).TotalMilliseconds));

            before = DateTime.Now;
            this.findFoodOperation.ExecuteOperation(this.AvailableAnts());
            timeLogging.Log(string.Format("[{0} ms] findfood finished", (int)(DateTime.Now - before).TotalMilliseconds));


            before = DateTime.Now;
            spreadOutOperation.ExecuteOperation(this.AvailableAnts());
            timeLogging.Log(string.Format("[{0} ms] spreadout finished", (int)(DateTime.Now - before).TotalMilliseconds));
        }
    }
}