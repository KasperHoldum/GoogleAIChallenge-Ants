using Ants;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Ants.Testing
{


    /// <summary>
    ///This is a test class for AttackTest and is intended
    ///to contain all AttackTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AttackTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for GeneratePotentialFocusMap
        ///</summary>
        [TestMethod()]
        public void GeneratePotentialFocusMapTest()
        {
            GameState state = new GameState(100, 100, 200, 200, 93, 5, 1);
            Attack target = new Attack(state);
            AStarPathFinding pathFinding = new AStarPathFinding(state);

            List<AntLoc> ants = new List<AntLoc>(new []{new AntLoc(50, 50, 0), });
            int[,] expected = new int[state.Width,state.Height];
            int[,] actual;
            actual = target.GeneratePotentialFocusMap(pathFinding, ants).Item1;
            
        }

        /// <summary>
        ///A test for GetAttackOffsets
        ///</summary>
        [TestMethod()]
        public void GetAttackOffsetsTest()
        {

            var state = new GameState(100, 100, 200, 200, 93, 5, 1);
            var target = new Attack(state);
            var loc = new Location(50, 50);
            IEnumerable<Location> actual = target.GetAttackOffsets(loc);

            Assert.IsTrue(actual.Count() == 21);
        }
    }
}
