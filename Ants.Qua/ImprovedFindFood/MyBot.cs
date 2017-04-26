using System;
using System.Collections.Generic;
using System.Linq;
using Ants.DataStructures.BinarySpacePartitioning;
using Ants.Operations;
using Ants.Operations.Attack;
using Ants.Operations.FindFood;
using Ants.Operations.SpreadOut;

namespace Ants.Qua.ImprovedFindFood
{
    public class MyBot : Bot
    {
        private readonly AntOperation spreadOutOperation;
        private readonly AntOperation findFoodOperation;
        private readonly SimpleCaptureHill attackHillOperation;

        public MyBot()
            : base("ImrovedFindFood")
        {
            spreadOutOperation = new VisibilitySpreadOut(this);
            findFoodOperation = new ImprovedStableMarriageFindFood(this);
            attackHillOperation = new SimpleCaptureHill(this);
        }

        /// <summary>
        /// New method that iterates over food and find nearest ant.
        /// </summary>
        /// <param name="gameState"></param>
        public override void DoTurn(GameState gameState)
        {
        

#if DEBUG
            Log.Log("Starting turn " + gameState.Turn);
            TimeLogging.Log(string.Format("[{0}] turn: {1}", DateTime.Now, gameState.Turn));


            DateTime before = DateTime.Now;
#endif
            Update(gameState);
#if DEBUG
            TimeLogging.Log(string.Format("[{0} ms] update finished", (int)(DateTime.Now - before).TotalMilliseconds));
#endif 
#if DEBUG
            before = DateTime.Now;
#endif  
            this.findFoodOperation.ExecuteOperation(this.AvailableAnts());
#if DEBUG
            TimeLogging.Log(string.Format("[{0} ms] findfood finished", (int)(DateTime.Now - before).TotalMilliseconds));
#endif

            if (gameState.TimeRemaining <= 100)
            {
                TimeLogging.Log("Exiting after find food because of time being low");
                return;
            }

#if DEBUG
            before = DateTime.Now;
#endif
            this.attackHillOperation.ExecuteOperation(this.AvailableAnts());
#if DEBUG
            TimeLogging.Log(string.Format("[{0} ms] SimpleCaptureHill finished", (int)(DateTime.Now - before).TotalMilliseconds));
#endif
            if (gameState.TimeRemaining <= 100)
            {
                TimeLogging.Log("Exiting before spreadout due to timeremaining being low");
                return;
            }
#if DEBUG
            before = DateTime.Now;
#endif
            spreadOutOperation.ExecuteOperation(this.AvailableAnts());
#if DEBUG
            TimeLogging.Log(string.Format("[{0} ms] spreadout finished", (int)(DateTime.Now - before).TotalMilliseconds));
#endif
        }
    }
}