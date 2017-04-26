using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ants.DataStructures.BinarySpacePartitioning;

namespace Ants.Testing
{
    [TestClass]
    public class KdTreeTest
    {
        [TestMethod]
        public void Remove6Test()
        {
            var state = new GameState(90, 60, 2000, 2000, 55, 5, 1);
            var visibility = new Visibility(state);


            state.StartNewTurn();
            state.AddAnt(32, 65, 0);
            Assert.IsNotNull(visibility.InvisibleSpotsTree.FindLocation(new Location(74, 34)));
            visibility.Update(state);
            Assert.IsNotNull(visibility.InvisibleSpotsTree.FindLocation(new Location(74, 34)));


            AntRegistry.RegisterMove(new Location(65, 32), new Location(66, 32));
            visibility.UpdateAntMove(new Location(65, 32), new Location(66, 32));
            Assert.IsNotNull(visibility.InvisibleSpotsTree.FindLocation(new Location(74, 34)));
            state.StartNewTurn();
            state.AddAnt(32, 66, 0);
            visibility.Update(state);
            AntRegistry.RegisterMove(new Location(66, 32), new Location(67, 32));
            visibility.UpdateAntMove(new Location(66, 32), new Location(67, 32));
        }

        [TestMethod]
        public void Remove7Test()
        {
            const int seed = 15;
            var r = new Random(seed);

            var locations = new List<Location>();

            for (int j = 0; j < 300; j++)
            {
                var loc = new Location(r.Next(0, 100), r.Next(0, 100));

                if (!locations.Contains(loc))
                    locations.Add(loc);
            }

            var tree = new KdTree<Location>(new DonutDistanceCalculator(100, 100), locations);

            for (int i = locations.Count - 1; i >= 0; i--)
            {
                Assert.IsNotNull(tree.FindLocation(locations[i]));
                tree.Remove(locations[i]);
                KdNode<Location> kdNode = tree.FindLocation(locations[i]);
                Assert.IsNull(kdNode);
                locations.RemoveAt(i);
                foreach (var location in locations)
                {
                    KdNode<Location> findLocation = tree.FindLocation(location);

                    Assert.IsNotNull(findLocation, "A location was erranously removed: " + location.ToString());
                }
            }
        }

        [TestMethod]
        public void RemoveAllElementsTest()
        {
            var tree = new KdTree<Location>(new DonutDistanceCalculator(10, 10));
            tree.Add(new Location(0, 0));
            tree.Remove(new Location(0, 0));

            Assert.AreEqual(0, tree.Count);


            tree.Add(new Location(0, 1));
            tree.Add(new Location(2, 0));
            tree.Add(new Location(0, 3));

            tree.Remove(new Location(0, 1));
            tree.Remove(new Location(2, 0));
            tree.Remove(new Location(0, 3));

            Assert.AreEqual(0, tree.Count);

            List<Location> nodes = new List<Location>()
                                       {
                                           new Location(0, 0),
                                           new Location(-1, 0),
                                           new Location(-2, 0),
                                           new Location(-3, 0),
                                           new Location(-1, 1),
                                           new Location(-1, -1),
                                           new Location(-1, -2),
                                           new Location(1, -2),
                                           new Location(1, -3),
                                           new Location(1, 4),
                                           new Location(2, 4),
                                           new Location(5, 3)
                                       };

            for (int index = 0; index < nodes.Count; index++)
            {
                var location = nodes[index];
                tree.Add(location);
                GraphHelper.GenerateGraph(tree).Save("Add" + index + ". " + location.ToString() + ".dgml");
            }

            for (int index = 0; index < nodes.Count; index++)
            {
                var location = nodes[index];
                tree.Remove(location);
                GraphHelper.GenerateGraph(tree).Save("Remove" + index + ". " + location.ToString() + ".dgml");
            }

            Assert.AreEqual(0, tree.Count);
        }

        [TestMethod]
        public void ReturnOnLocationTest()
        {
            var tree = new KdTree<Location>(new DonutDistanceCalculator(10, 10));
            var loc = new Location(5, 5);
            tree.Add(loc);


            Assert.AreEqual(loc, tree.FindNearestNeighbour(loc).Item1);
            Assert.AreEqual(loc, tree.FindNodesInRange(loc, 10).First());
        }

        [TestMethod]
        public void AddTest()
        {
            var tree = new KdTree<Location>(new DonutDistanceCalculator(10, 10));

            tree.Add(new Location(0, 0));
            tree.Add(new Location(0, 1));
            tree.Add(new Location(5, 0));
            tree.Add(new Location(5, 3));
            tree.Add(new Location(5, 2));
            tree.Add(new Location(6, 3));
            tree.Add(new Location(0, 2));


            Assert.AreEqual(new Location(6, 3), tree.Root.Right.Right.Right.Right.Value);
            Assert.AreEqual(new Location(0, 2), tree.Root.Right.Right.Left.Value);
        }

        [TestMethod]
        public void RemoveTest()
        {
            var tree = new KdTree<Location>(new DonutDistanceCalculator(10, 10));

            tree.Add(new Location(0, 0));
            tree.Add(new Location(0, 1));
            tree.Add(new Location(5, 0));
            tree.Add(new Location(5, 3));
            tree.Add(new Location(5, 2));
            tree.Add(new Location(6, 3));
            tree.Add(new Location(0, 2));

            GraphHelper.GenerateGraph(tree).Save("RemoveTest.dgml");

            Assert.AreEqual(new Location(6, 3), tree.Root.Right.Right.Right.Right.Value);
            Assert.AreEqual(new Location(0, 2), tree.Root.Right.Right.Left.Value);


            tree.Remove(new Location(0, 0));

            Assert.IsFalse(tree.Root.Value == new Location(0, 0));


            tree.Remove(new Location(6, 3));

            Assert.IsTrue(tree.FindLocation(new Location(6, 3)) == null);

        }

        [TestMethod]
        public void Remove2Test()
        {
            var tree = new KdTree<Location>(new DonutDistanceCalculator(10, 10));

            tree.Add(new Location(0, 0));
            tree.Add(new Location(0, 1));

            tree.Remove(new Location(0, 1));

            Assert.IsTrue(tree.Root.Left == null);
            Assert.IsTrue(tree.Root.Value == new Location(0, 0));
        }

        [TestMethod]
        public void Remove3Test()
        {
            var tree = new KdTree<Location>(new DonutDistanceCalculator(10, 10));

            tree.Add(new Location(0, 0));
            tree.Add(new Location(0, 1));
            tree.Add(new Location(0, 2));
            tree.Add(new Location(3, 1));
            tree.Add(new Location(3, 0));
            tree.Add(new Location(3, 2));
            tree.Add(new Location(5, 1));
            tree.Add(new Location(1, 1));
            tree.Add(new Location(-2, 1));

            tree.Remove(new Location(3, 0));
            Assert.IsFalse(tree.FindNodesInManhattenRange(new Location(3, 0), 1).Contains(new Location(3, 0)));
        }
        [TestMethod]
        public void FindMinTest()
        {
            var tree = new KdTree<Location>(new DonutDistanceCalculator(1000, 1000));

            tree.Add(new Location(0, 6));
            tree.Add(new Location(0, 1));
            tree.Add(new Location(0, 2));
            tree.Add(new Location(3, 3));
            tree.Add(new Location(3, 6));
            tree.Add(new Location(3, 5));
            tree.Add(new Location(5, 31));
            tree.Add(new Location(1, 3));
            tree.Add(new Location(-2, 3));

            var min = tree.FindMinimum(tree.Root, 0, 0);
            Assert.IsTrue(min.Value == new Location(-2, 3), "Find min didn't work on x-axis");
            Assert.IsTrue(tree.FindMinimum(tree.Root, 1, 0).Value == new Location(0, 1), "FindMin failed on Y-axis");
        }

        [TestMethod]
        public void FindMaxTest()
        {
            var tree = new KdTree<Location>(new DonutDistanceCalculator(1000, 1000));

            tree.Add(new Location(0, 6));
            tree.Add(new Location(0, 1));
            tree.Add(new Location(0, 2));
            tree.Add(new Location(3, 3));
            tree.Add(new Location(3, 6));
            tree.Add(new Location(3, 5));
            tree.Add(new Location(5, 31));
            tree.Add(new Location(1, 3));
            tree.Add(new Location(-2, 3));

            var min = tree.FindMaximum(tree.Root, 0, 0);
            Assert.IsTrue(min.Value == new Location(5, 31));
            Assert.IsTrue(tree.FindMaximum(tree.Root, 1, 0).Value == new Location(5, 31));
        }

        [TestMethod]
        public void Remove4Test()
        {
            var tree = new KdTree<Location>(new DonutDistanceCalculator(10, 10));

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    tree.Add(new Location(i, j));
                }
            }


            var locationToRemove = new Location(5, 5);
            tree.Remove(locationToRemove);

            Assert.IsFalse(tree.FindNodesInManhattenRange(locationToRemove, 1).Contains(locationToRemove));
        }



        [TestMethod]
        public void FindLocationTest()
        {
            var tree = new KdTree<Location>(new DonutDistanceCalculator(10, 10));

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    tree.Add(new Location(i, j));
                }
            }


            var loc = tree.FindLocation(new Location(3, 6));

            Assert.IsTrue(loc.Value == new Location(3, 6));

            Assert.IsNull(tree.FindLocation(new Location(10, 10)));
        }



        [TestMethod]
        public void FindNodesInRangeTest()
        {
            KdTree<Location> tree = new KdTree<Location>(new DonutDistanceCalculator(10, 10), new List<Location>()
                                         {
                                             new Location(1,1),
                                             new Location(0,1),
                                             new Location(0,0),
                                             new Location(1,0),
                                         });

            var nodesInRange = tree.FindNodesInRange(new Location(0, 0), 1);

            Assert.IsTrue(nodesInRange.Count() == 3, "Didn't find all 3 nodes in range 1");
            Assert.IsTrue(tree.FindNodesInRange(new Location(0, 0), 10).Count() == 4, "Didn't find all nodes in the tree");

        }

        [TestMethod]
        public void FindNodesInDonutRangeTest()
        {
            KdTree<Location> tree = new KdTree<Location>(new DonutDistanceCalculator(50, 50), new List<Location>()
                                         {
                                             new Location(7, 5),
                                             new Location(0,0),
                                         });

            var nodesInRange = tree.FindNodesInRange(new Location(45, 2), 10);

            Assert.IsTrue(nodesInRange.Count() == 1, "Didn't find node in donut range");

        }

        [TestMethod]
        public void FindNearestNeighbourTest()
        {
            KdTree<Location> tree = new KdTree<Location>(new DonutDistanceCalculator(10, 10), new List<Location>()
                                         {
                                             new Location(1,1),
                                             new Location(0,1),
                                             new Location(0,0),
                                             new Location(1,0),
                                         });

            var neighbor = tree.FindNearestNeighbour(new Location(2, 1));

            Assert.IsTrue(neighbor.Item1 == new Location(1, 1));
        }

        [TestMethod]
        public void FindNearestNeighbour2Test()
        {
            KdTree<Location> tree = new KdTree<Location>(new DonutDistanceCalculator(100, 100));

            tree.Add(new Location(26, 78));
            tree.Add(new Location(24, 79));
            tree.Add(new Location(16, 75));
            tree.Add(new Location(2, 70));
            tree.Add(new Location(19, 59));
            tree.Add(new Location(5, 79));
            tree.Add(new Location(2, 79));

            var neighbor = tree.FindNearestNeighbour(new Location(2, 79));

            Assert.IsTrue(neighbor.Item1 == new Location(2, 79));
        }
        [TestMethod]
        public void FindNearestNeighbourVsFindNodesInManhattenRangeTest()
        {
            var r = new Random();
            for (int i = 0; i < 100000; i++)
            {
                var tree = new KdTree<Location>(new DonutDistanceCalculator(100, 100), new List<Location>() { new Location(r.Next(100), r.Next(100)) });
                var initial = new Location(r.Next(100), r.Next(100));
                var closestInvisibleSpot = tree.FindNearestNeighbour(initial);
                var distanceToClosest = closestInvisibleSpot.Item2;

                var allClosest = tree.FindNodesInManhattenRange(initial, (int)distanceToClosest).First();
            }
        }
    }
}
