using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Ants.DataStructures.BinarySpacePartitioning;

namespace Ants.Testing
{


    /// <summary>
    ///This is a test class for VisibilityTest and is intended
    ///to contain all VisibilityTest Unit Tests
    ///</summary>
    [TestClass()]
    public class VisibilityTest
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
        public void SpawnOnDeadAntTest()
        {
            GameState state = new GameState(200, 100, 2000, 2000, 93, 6, 6);
            state.StartNewTurn();
            state.AddAnt(50, 50, 0);
            state.Update();
            state.DeadAnt(50, 50, 0);
            state.AddAnt(50, 50, 0);
            state.Update();
            Assert.IsTrue(state.Visibility.IsLocationVisible(new Location(50,50)));
        }


        /// <summary>
        ///A test for Update
        ///</summary>
        [TestMethod]
        public void UpdatePerformanceTest()
        {
            GameState state = new GameState(200, 100, 2000, 2000, 93, 6, 6);

            Visibility visibility = new Visibility(state); 
            Random r = new Random();
            for (int i = 0; i < 200; i++)
            {
                state.MyAnts.Add(new AntLoc(r.Next(0, state.Height), r.Next(0, state.Width), 0));
            }
            var before = DateTime.UtcNow;
            visibility.Update(state);
            var updateTime = DateTime.UtcNow - before;

            before = DateTime.UtcNow;
            KdTree<Location> tree = visibility.InvisibleSpotsTree;

            foreach (var inactiveAnt in state.MyAnts)
            {
                var closestInvisibleSpot = tree.FindNearestNeighbour(inactiveAnt);
                var distanceToClosest = state.DonutDistances.Distance(inactiveAnt,closestInvisibleSpot.Item1);

                var allClosest = tree.FindNodesInRange(inactiveAnt, distanceToClosest);
                //var closestSpot = visibility.InvisibleSpots.OrderBy(l => l.SquaredDistanceTo(inactiveAnt)).ThenByDescending(visibility.TurnsSinceLastSeen).First();
            }

            var spreadOutTime = DateTime.UtcNow - before;

            const int threshold = 400;
            Assert.IsTrue(spreadOutTime.TotalMilliseconds + updateTime.TotalMilliseconds < threshold, "It took " + spreadOutTime.TotalMilliseconds + "ms");
        }
        [TestMethod]
        public void ComputeVisibilityTest()
        {
            var state = new GameState(20, 20, 2000, 2000, 93, 6, 6); 

            state.MyAnts.Add(new AntLoc(4, 4, 0));

            var turnsSinceLastSeen = new Dictionary<Location, int>();
            for (int col = 0; col < state.Width; col++)
            {
                for (int row = 0; row < state.Height; row++)
                {
                    turnsSinceLastSeen[new Location(col, row)] = 1600;
                }
            }
            var invis = new List<Location>();
            var vis = new List<Location>();
            Visibility.ComputeVisibilityUsingKdTree(state, invis, vis, turnsSinceLastSeen);
            var bitmap = new Bitmap(state.Width, state.Height);

            invis.ForEach(l => bitmap.SetPixel(l.Col, l.Row, Color.Red));
            vis.ForEach(l => bitmap.SetPixel(l.Col, l.Row, Color.Green));
            bitmap.SetPixel(4,4, Color.Black);

            bitmap.Save("visibility.bmp");
        }

        [TestMethod]
        public void ComputeVisibility2Test()
        {
            var state = new GameState(100, 100, 2000, 2000, 93, 6, 6);

            state.MyAnts.Add(new AntLoc(4, 4, 0));

            var turnsSinceLastSeen = new Dictionary<Location, int>();
            for (int col = 0; col < state.Width; col++)
            {
                for (int row = 0; row < state.Height; row++)
                {
                    turnsSinceLastSeen[new Location(col, row)] = 1600;
                }
            }
            var invis = new List<Location>();
            var vis = new List<Location>();
            Visibility.ComputeVisibilityUsingKdTree(state, invis, vis, turnsSinceLastSeen);

            Assert.IsTrue(invis.Contains(new Location(50,50)));
            Assert.IsTrue(vis.Contains(new Location(0,0)));
        }

        [TestMethod]
        public void ComputeVisibilityUsingNewMethodTest()
        {
            var state = new GameState(100, 100, 2000, 2000, 93, 6, 6);
            var visibility = new Visibility(state);
            state.AddAnt(10, 10, 0);

            visibility.Update(state);

            Assert.IsTrue(visibility.IsLocationVisible(new Location(10,10)));
            Assert.IsTrue(visibility.IsLocationVisible(new Location(19,10)));
            Assert.IsTrue(!visibility.IsLocationVisible(new Location(60, 60)));
            Assert.IsTrue(!visibility.IsLocationVisible(new Location(0, 10)));


            visibility.UpdateAntMove(new Location(10,10),new Location(9,10));

            Assert.IsTrue(visibility.IsLocationVisible(new Location(0, 10)));
            Assert.IsTrue(!visibility.IsLocationVisible(new Location(19, 10)));
        }

        [TestMethod]
        public void ComputeVisibilityDeadAntsTest()
        {
            AntRegistry.Clear();
            var state = new GameState(100, 100, 2000, 2000, 93, 6, 6);
            var visibility = new Visibility(state);
            state.AddAnt(10, 10, 0);

            visibility.Update(state);

            Assert.IsTrue(visibility.IsLocationVisible(new Location(10, 10)));
            Assert.IsTrue(visibility.IsLocationVisible(new Location(19, 10)));
            Assert.IsTrue(!visibility.IsLocationVisible(new Location(60, 60)));
            Assert.IsTrue(!visibility.IsLocationVisible(new Location(0, 10)));


            visibility.UpdateAntMove(new Location(10, 10), new Location(9, 10));

            Assert.IsTrue(visibility.IsLocationVisible(new Location(0, 10)));
            Assert.IsTrue(!visibility.IsLocationVisible(new Location(19, 10)));

            state.StartNewTurn();

            state.DeadAnt(10,9, 0);


            visibility.Update(state);

            Assert.IsTrue(!visibility.IsLocationVisible(new Location(0, 10)));
        }

        [TestMethod]
        public void MoveAndKilledVisibilityTest()
        {
            AntRegistry.Clear();
            var state = new GameState(100, 100, 2000, 2000, 93, 6, 6);
            var visibility = new Visibility(state);
            state.AddAnt(10, 10, 0);

            visibility.Update(state);
            visibility.UpdateAntMove(new Location(10, 10), new Location(9, 10));

            state.StartNewTurn();
            state.DeadAnt(10, 9, 0);


            visibility.Update(state);

            Assert.IsTrue(!visibility.IsLocationVisible(new Location(0, 10)));
        }


        [TestMethod]
        public void GetViewFieldOffsetTest()
        {
             var state = new GameState(96, 60, 2000, 2000, 93, 6, 6);
            var visibility = new Visibility(state);

            visibility.Update(state);

            for (int i = 0; i < state.Width; i++)
            {
                for (int j = 0; j < state.Height; j++)
                {
                    var loc = new Location(i, j);
                    var offsets = visibility.GetViewFields(loc);

                    foreach (var location in offsets)
                    {
                        Assert.IsTrue(location.Col >= 0 && location.Col < state.Width, "Coloumns view offsets failed for " + loc);
                        Assert.IsTrue(location.Row >= 0 && location.Row < state.Height, "Row view offsets failed for " + loc);
                    }
                }
            }

        }

        [TestMethod]
        public void GenerateViewOffsetsTest()
        {
            var state = new GameState(96, 60, 2000, 2000, 93, 6, 6);
            var visibility = new Visibility(state);

            foreach (var visibilityOffset in visibility.VisibilityOffsets)
            {
                Assert.IsTrue(Math.Abs(visibilityOffset.Col) <= state.ViewRadius, "Col ({0}) was bigger than view radius ({1})", Math.Abs(visibilityOffset.Col), state.ViewRadius);
                Assert.IsTrue(Math.Abs(visibilityOffset.Row) <= state.ViewRadius, "Row ({0}) was bigger than view radius ({1})", Math.Abs(visibilityOffset.Row), state.ViewRadius);
            }
        }
    }
}
