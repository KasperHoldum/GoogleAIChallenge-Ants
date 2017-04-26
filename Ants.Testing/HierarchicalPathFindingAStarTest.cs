using System.Diagnostics;
using Ants.DataStructures.HPA;
using Ants.HPA;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Ants;
using System.Collections.Generic;

namespace Ants.Testing
{
    
    
    /// <summary>
    ///This is a test class for HierarchicalPathFindingAStarTest and is intended
    ///to contain all HierarchicalPathFindingAStarTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HierarchicalPathFindingAStarTest
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

        [TestMethod]
        public void SimpleFindPathTest()
        {
            var st = Map.Parse("maps/maze_04p_02.map");
            var hpa = new HierarchicalPathFindingAStar(st, 10);

            var start = new Location(0, 14);
            var goal = new Location(15, 14);
        }

        [TestMethod]
        public void LookupClusterTest()
        {
            var st = Map.Parse("maps/maze_04p_02.map");
            var star = new AStarPathFinding(st);
            var hpa = new HierarchicalPathFindingAStar(st, 10);

            ClusterCollection cc = new ClusterCollection(st);
            cc.Initialize(star);
            for (int col = 0; col < st.Width; col++)
            {
                for (int row = 0; row < st.Height; row++)
                {
                    try
                    {
                        var cluster = cc.LookupCluster(new Location(col, row));
                    }
                    catch (Exception)
                    {
                        
                        Assert.Fail("Failed to lookup cluster located at " + new Location(col, row));
                    }
                }
            }

            try
            {
                cc.LookupCluster(new Location(st.Width, st.Height));

                Assert.Fail("Could look up a  cluster outside range");
            }
            catch (Exception)
            {
                
            }
        }

        /// <summary>
        ///A test for PairsOfClusters
        ///</summary>
        [TestMethod()]
        public void PairsOfClustersTest()
        {
            //const int clusterCount = 10;
            //var gameState = new GameState(50,50, 2000, 2000, 99, 5, 1); 
            //var clusters = new ClusterCollection(clusterCount);
            //clusters.Initialize(gameState);
            //var actual = HierarchicalPathFindingAStar.PairsOfClusters(gameState,clusters);
            //Assert.AreEqual(clusterCount * clusterCount / 2 * 4, actual.Count());
        }
               [TestMethod()]
        public void PathfindingDuelPerformanceTest()
               {
                   var st = Map.Parse("maps/maze_04p_02.map");
            var star = new AStarPathFinding(st);   
            var hpa = new HierarchicalPathFindingAStar(st, 10);


            var start = new Location(0,0);
            var goal = new Location(39,34);

                   DateTime before = DateTime.UtcNow;
                   var roflmao2 = hpa.FindPath(start, goal);
                   var hpaWinner = (DateTime.UtcNow - before).TotalMilliseconds;
            

                   before = DateTime.UtcNow;
                   var roflmao = star.FindPath(start, goal);
                   var astarNub = (DateTime.UtcNow - before).TotalMilliseconds;


                  before = DateTime.UtcNow;
                    roflmao2 = hpa.FindPath(start, goal);
                  hpaWinner = (DateTime.UtcNow - before).TotalMilliseconds;


                   before = DateTime.UtcNow;
                    roflmao = star.FindPath(start, goal);
                    astarNub = (DateTime.UtcNow - before).TotalMilliseconds;
            Assert.IsTrue(hpaWinner < astarNub );
        }
    }
}
