using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
namespace Ants.Testing
{
    [TestClass]
    public class PathFindingTesting
    {
        [TestMethod]
        public void TestOptimatility()
        {
            GameState state = new GameState(10,10,1000, 1000, 19, 19, 19);
            PathFinding pathF = new AStarPathFinding(state);



            // add terrain
            state.AddWater(1,1);
            state.AddWater(1,2);
            state.AddWater(1,3);
            state.AddWater(2,4);

            //

            Location start = new Location(2,0);
            Location goal = new Location(2,3);
            var path = pathF.FindPath(start, goal);
            const int pathLength = 8;
            Assert.AreEqual(pathLength, path.Count);
        }

              [TestMethod]
        public void TestStartingBesidesGoal()
        {
            var state = new GameState(10, 10, 1000, 1000, 19, 19, 19);
            var pathF = new AStarPathFinding(state);


            Location start = new Location(2, 2);
            Location goal = new Location(2, 3);
            var path = pathF.FindPath(start, goal);
            path.Reverse();
            Assert.AreEqual(goal, path.First());
        }
    }
}
