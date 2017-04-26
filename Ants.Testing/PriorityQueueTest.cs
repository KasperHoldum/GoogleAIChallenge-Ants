using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Ants.DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ants.Testing
{
    [TestClass]
    public class PriorityQueueTest
    {
        [TestMethod]
        public void PerformanceTest()
        {
            Location goal = new Location(50,50);
            GameState gameState  =new GameState(200, 200, 2000, 2000, 93, 5, 3);
            var gScore = new Dictionary<Location,double>();
            PriorityQueue<Location, Location> sortedList = new PriorityQueue<Location, Location>(new AStarPathFinding.DistanceComparer<Location>(goal, gameState, gScore));
            SortList<Location> sortList = new SortList<Location>(new AStarPathFinding.DistanceComparer<Location>(goal, gameState, gScore));

            List<Location> randomLocations = new List<Location>();
            Random random = new Random();
            for (int i = 0; i < 1000; i++)
            {
                var randLoc = new Location(random.Next(0, 200), random.Next(0, 200));
                if (!randomLocations.Contains(randLoc))
                {
                    randomLocations.Add(randLoc);
                    gScore[randLoc] = 0;
                }
            }

            bool hasSame = randomLocations.Any(l => randomLocations.Where(ll => ll.GetHashCode() == l.GetHashCode()).Count() > 1);

            var before = DateTime.UtcNow;
            randomLocations.ForEach(l => sortedList.Enqueue(l,l));
            var sortedListTime = DateTime.UtcNow - before;

            before = DateTime.UtcNow;
            randomLocations.ForEach(l => sortList.Add(l));
            var sortListTime = DateTime.UtcNow - before;

            Assert.IsTrue(sortListTime < sortedListTime);
        }

        public class PriorityQueue<P, V>
        {
            public PriorityQueue(IComparer<P> comparer)
            {
                list = new SortedDictionary<P, Queue<V>>(comparer);
            }

            private SortedDictionary<P, Queue<V>> list = new SortedDictionary<P, Queue<V>>();
            public void Enqueue(P priority, V value)
            {
                Queue<V> q;
                if (!list.TryGetValue(priority, out q))
                {
                    q = new Queue<V>();
                    list.Add(priority, q);
                }
                q.Enqueue(value);
            }
            public V Dequeue()
            {
                // will throw if there isn’t any first element!
                var pair = list.First();
                var v = pair.Value.Dequeue();
                if (pair.Value.Count == 0) // nothing left of the top priority.
                    list.Remove(pair.Key);
                return v;
            }
            public bool IsEmpty
            {
                get { return !list.Any(); }
            }
        }
    }
}
