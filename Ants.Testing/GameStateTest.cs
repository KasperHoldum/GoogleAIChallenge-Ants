using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ants.Testing
{
    [TestClass]
    public class GameStateTest
    {
        [TestMethod]
        public void HillsCountTest()
        {
            GameState state = new GameState(20,20, 500,500,100, 2,5);

            for (int i = 0; i < 100; i++)
            {
                state.StartNewTurn();

                state.AddHill(10,10, 0);
                state.AddHill(10,10, 1);
            }

            Assert.IsTrue(state.MyHills.Count == 1);
            Assert.IsTrue(state.EnemyHills.Count == 1);
        }
    }
}
