using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Authentication;

namespace Ants.DataStructures.BinarySpacePartitioning
{
    public class KdTree<T> where T : Location
    {
        private readonly IDistanceCalculator distanceCalculator;
        private const int Dimensions = 2;


        public KdNode<T> Root { get; set; }


        public int Count { get; private set; }

        public KdTree(IDistanceCalculator distanceCalculator)
        {
            this.distanceCalculator = distanceCalculator;
        }

        public KdTree(IDistanceCalculator distanceCalculator, List<T> initialLocations)
            : this(distanceCalculator)
        {
            Root = CreateTree(initialLocations, 0);//distanceCalculator);
        }

        private static KdNode<T> CreateTree(List<T> nodes, int axis)
        {
            if (nodes.Count == 0)
            {
                return null;
            }

            axis = axis % Dimensions;

            // NOTE: check perfomrance between this and .OrderBy(l => l[dimension]) if sorting performance this ever becomes an issue
            var ordered = nodes.OrderBy(t => t[axis]).ToList(); //.Sort(DimensionComparer.ComparerByDimension(depth)); // give it icomparer to sort by specific dimension
            var firstMedian = ordered.First(t => t[axis] == ordered[ordered.Count/2][axis]);
            var firstMedianIndex = ordered.IndexOf(firstMedian);

            var nodesBeforeMedian = ordered.Take(firstMedianIndex).ToList();

            //// hack so this code works for even as well as uneven sequences
            ////  for even elements, this will just take one less from the back (since we already took the median out)
            ////  for uneven the ceiling will round up the number, and then subtract 1, which means an equal amount of elements will be taken from both ends
            var nodesAfterMedian =  ordered.Skip(firstMedianIndex+1).ToList();

#if DEBUG
            if (nodesAfterMedian.Count + nodesBeforeMedian.Count != nodes.Count - 1)
                throw new InvalidOperationException("Didn't get all nodes in before/after medium lists");
#endif

            var node = new KdNode<T>(firstMedian)
                            {
                                Left = CreateTree(nodesBeforeMedian, axis + 1),
                                Right = CreateTree(nodesAfterMedian, axis + 1)
                            };

            return node;    
        }

        private static KdNode<T> CreateTree(List<T> nodes, IDistanceCalculator donutDistanceCalculator)
        {
            if (nodes.Count == 0)
            {
                return null;
            }

            //var depth = dimension % Dimensions;

            //// NOTE: check perfomrance between this and .OrderBy(l => l[dimension]) if sorting performance this ever becomes an issue
            //var ordered = nodes.OrderBy(t => t[0]).ToList(); //.Sort(DimensionComparer.ComparerByDimension(depth)); // give it icomparer to sort by specific dimension
            //var median = ordered[ordered.Count / 2];
            //var nodesBeforeMedian = ordered.Take(ordered.Count / 2).ToList();

            ////// hack so this code works for even as well as uneven sequences
            //////  for even elements, this will just take one less from the back (since we already took the median out)
            //////  for uneven the ceiling will round up the number, and then subtract 1, which means an equal amount of elements will be taken from both ends
            //var nodesAfterMedian = Enumerable.Reverse(ordered).Take((int)Math.Ceiling(ordered.Count / 2.0) - 1).ToList(); 

            //var node = new KdNode(median)
            //                {
            //                    Left = CreateTree(nodesBeforeMedian, depth + 1),
            //                    Right = CreateTree(nodesAfterMedian, depth + 1)
            //                };

            //return node;    
            var tree = new KdTree<T>(donutDistanceCalculator);
            foreach (var location in nodes)
            {
                tree.Add(location);
            }
            return tree.Root;
        }


        public void Add(T location)
        {

            Count += 1;
            if (Root == null)
            {
                Root = new KdNode<T>(location);
                return;
            }

            var nextNode = Root;
            var dimension = 0;

            while (true) // nextNode != null
            {
                if (dimension == 0)
                {
                    if (location.Col >= nextNode.Value.Col)
                    {
                        if (nextNode.Right != null)
                        {
                            nextNode = nextNode.Right;
                        }
                        else
                        {
                            nextNode.Right = new KdNode<T>(location);
                            break;
                        }
                    }
                    else
                    {
                        if (nextNode.Left != null)
                        {
                            nextNode = nextNode.Left;
                        }
                        else
                        {
                            nextNode.Left = new KdNode<T>(location);
                            break;
                        }
                    }
                }
                else
                {
                    if (location.Row >= nextNode.Value.Row)
                    {
                        if (nextNode.Right != null)
                        {
                            nextNode = nextNode.Right;
                        }
                        else
                        {
                            nextNode.Right = new KdNode<T>(location);
                            break;
                        }
                    }
                    else
                    {
                        if (nextNode.Left != null)
                        {
                            nextNode = nextNode.Left;
                        }
                        else
                        {
                            nextNode.Left = new KdNode<T>(location);
                            break;
                        }
                    }
                }



                dimension = dimension == 1 ? 0 : 1;
            }
        }

        public void Remove(T toRemove)
        {
            if (toRemove == null)
                throw new ArgumentNullException("toRemove");

            if (Root == null)
                throw new ArgumentException("There are no elemnts in this collection");

            Count -= 1;

            Root = Remove(toRemove, Root, 0);
        }

        private KdNode<T> Remove(T toRemove, KdNode<T> currentNode, int dimension)
        {
            if (currentNode == null) return null;

            if (currentNode.Value == toRemove)
            {
                // remove node

                // base case where location to remove is a leaf
                if (currentNode.Left == null && currentNode.Right == null)
                    return null;

                if (currentNode.Right != null)
                {
                    currentNode.Value = FindMinimum(currentNode.Right, dimension, (dimension + 1) % 2).Value;
                    currentNode.Right = Remove(currentNode.Value, currentNode.Right, (dimension + 1) % 2);
                }
                else
                {
                    KdNode<T> findMinimum = FindMinimum(currentNode.Left, dimension, (dimension + 1) % 2);
                    currentNode.Value = findMinimum.Value;
                    currentNode.Right = Remove(findMinimum.Value, currentNode.Left, (dimension + 1) % 2);
                    currentNode.Left = null;

                }
            }
            else
            {
                // search rest of tree for node
                bool isLocationValueBigger = toRemove[dimension].CompareTo(currentNode.Value[dimension]) >= 0;

                if (isLocationValueBigger)
                {
                    // search right tree
                    var newCurrentNode = currentNode.Right;
                    if (newCurrentNode == null)
                    {
                        throw new ArgumentException("Location not found " + toRemove, "toRemove");
                    }

                    currentNode.Right = Remove(toRemove, newCurrentNode, (dimension + 1) % Dimensions);
                }
                else
                {
                    // search left tree
                    var newCurrentNode = currentNode.Left;

                    if (newCurrentNode == null)
                    {
                        throw new ArgumentException("Location not found " + toRemove, "toRemove");
                    }

                    currentNode.Left = Remove(toRemove, newCurrentNode, (dimension + 1) % Dimensions);
                }
            }

            return currentNode;
        }

        public KdNode<T> FindMaximum(KdNode<T> node, int splittingDimension, int depth)
        {
            if (node == null)
                throw new ArgumentException("Retard");

            var dimension = depth % Dimensions;
            if (dimension == splittingDimension)
            {
                // Find maximum value in left sub-tree.
                if (node.Right == null)
                    return node;
                return FindMaximum(node.Right, splittingDimension, depth + 1);
            }

            // Find node with minimum value in sub-tree of current node.
            var nodeLocation = node.Value;
            var leftMinValue = node.Left != null ? FindMaximum(node.Left, splittingDimension, depth + 1) : null;
            var rightMinValue = node.Right != null ? FindMaximum(node.Right, splittingDimension, depth + 1) : null;

            if (leftMinValue != null && leftMinValue.Value[splittingDimension].CompareTo(nodeLocation[splittingDimension]) > 0)
                return leftMinValue;
            if (rightMinValue != null && rightMinValue.Value[splittingDimension].CompareTo(nodeLocation[splittingDimension]) > 0)
                return rightMinValue;
            return node;
        }


        public KdNode<T> FindMinimum(KdNode<T> node, int whichAxis, int cd)
        {
            if (node == null)
                throw new ArgumentException("Retard");

            var axis = cd % Dimensions;
            if (axis == whichAxis)
            {
                // Find minimum value in left sub-tree.
                if (node.Left == null)
                    return node;
                return FindMinimum(node.Left, whichAxis, cd + 1);
            }


            // Find node with minimum value in sub-tree of current node.
            var nodeLocation = node.Value;
            var leftMinValue = node.Left != null ? FindMinimum(node.Left, whichAxis, cd + 1) : null;
            var rightMinValue = node.Right != null ? FindMinimum(node.Right, whichAxis, cd + 1) : null;

            if (leftMinValue != null && leftMinValue.Value[whichAxis].CompareTo(nodeLocation[whichAxis]) < 0 && (rightMinValue == null || rightMinValue.Value[whichAxis].CompareTo(leftMinValue.Value[whichAxis]) >= 0))
                return leftMinValue;
            if (rightMinValue != null && rightMinValue.Value[whichAxis].CompareTo(nodeLocation[whichAxis]) < 0)
                return rightMinValue;
            return node;
        }



        public Tuple<T, double> FindNearestNeighbour(Location location, bool useManhatten = true)
        {
            if (Root == null)
                return null;

            var startingBestDistance = new DataStructures.Tuple<KdNode<T>, double>(Root, useManhatten ? distanceCalculator.ManhattenDistance(location, Root.Value) : distanceCalculator.SquaredDistance(location, Root.Value));
            var closestNeighborInfo = FindNearestNeighbour(location, Root, startingBestDistance, useManhatten);

            return new Tuple<T, double>(closestNeighborInfo.Item1.Value, closestNeighborInfo.Item2);
        }

        public KdNode<T> FindLocation(Location location)
        {
            if (Root == null)
            {
                return null;
            }

            var nextNode = Root;
            var dimension = 0;

            while (true) // nextNode != null
            {
                if (nextNode.Value == location)
                {
                    return nextNode;
                }

                if (dimension == 0)
                {
                    if (location.Col >= nextNode.Value.Col)
                    {
                        if (nextNode.Right != null)
                        {
                            nextNode = nextNode.Right;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        if (nextNode.Left != null)
                        {
                            nextNode = nextNode.Left;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                else
                {
                    if (location.Row >= nextNode.Value.Row)
                    {
                        if (nextNode.Right != null)
                        {
                            nextNode = nextNode.Right;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        if (nextNode.Left != null)
                        {
                            nextNode = nextNode.Left;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

                dimension = dimension == 1 ? 0 : 1;
            }
        }


        private DataStructures.Tuple<KdNode<T>, double> FindNearestNeighbour(Location location, KdNode<T> currentNode, DataStructures.Tuple<KdNode<T>, double> currentBestDistance, bool useManhatten, int depth = 0)
        {
            if (currentNode == null)
                return currentBestDistance;

            var dimension = depth % Dimensions;
            double squaredDistance = useManhatten ? distanceCalculator.ManhattenDistance(currentNode.Value, location) : distanceCalculator.SquaredDistance(currentNode.Value, location);


            // check if the current node is better than the current best node
            if (squaredDistance < currentBestDistance.Item2)
            {
                currentBestDistance = new DataStructures.Tuple<KdNode<T>, double>(currentNode, squaredDistance);
            }

            // check the subtree for nodes nearer
            var nearestChildNode = location[dimension] >= currentNode.Value[dimension]
                                       ? currentNode.Right
                                       : currentNode.Left;

            if (nearestChildNode != null)
            {
                // will return the current best if no better is found
                currentBestDistance = FindNearestNeighbour(location, nearestChildNode, currentBestDistance, useManhatten, dimension + 1);
            }

            if (currentBestDistance.Item2 > squaredDistance)
            {
                var farChild = nearestChildNode == currentNode.Left ? currentNode.Right : currentNode.Left;

                if (farChild != null)
                {
                    currentBestDistance = FindNearestNeighbour(location, farChild, currentBestDistance, useManhatten, dimension + 1);
                }
            }

            return new DataStructures.Tuple<KdNode<T>, double>(currentBestDistance.Item1, currentBestDistance.Item2); // working with squared distances internally
        }

        public IEnumerable<T> FindNodesInRange(Location location, double squaredRange)
        {
            if (location == null)
                throw new ArgumentNullException("location");

            if (Root == null)
                return new List<T>();


            return FindNodesInRange(location, squaredRange, Root, 0);
        }

        private IEnumerable<T> FindNodesInRange(Location location, double squaredRange, KdNode<T> currentNode, int depth)
        {
            var dimension = depth % Dimensions;
            var squaredDistance = distanceCalculator.SquaredDistance(location, currentNode.Value);

            List<T> nodesInRAnge = new List<T>();

            if (squaredDistance <= squaredRange)
            {
                nodesInRAnge.Add(currentNode.Value);
            }

            // check the subtree for nodes nearer
            var nearestChildNode = location[dimension] >= currentNode.Value[dimension]
                                       ? currentNode.Right
                                       : currentNode.Left;

            if (nearestChildNode != null)
            {
                // will return the current best if no better is found
                nodesInRAnge.AddRange(FindNodesInRange(location, squaredRange, nearestChildNode, dimension + 1));
            }

            var farestChildNode = nearestChildNode == currentNode.Left ? currentNode.Right : currentNode.Left;
            // check whether hypersphere collisdes with hyperplane
            if ( squaredRange >= Math.Abs(location[dimension] - currentNode.Value[dimension]))
            {
                

                if (farestChildNode != null)
                {
                    nodesInRAnge.AddRange(FindNodesInRange(location, squaredRange, farestChildNode, dimension + 1));
                }
            }

            return nodesInRAnge;
        }

        public IEnumerable<T> FindNodesInManhattenRange(Location location, int manhattenRange)
        {
            if (location == null)
                throw new ArgumentNullException("location");

            if (Root == null)
                return new List<T>();


            return FindNodesInManhattenRange(location, manhattenRange, Root, 0);
        }

        private IEnumerable<T> FindNodesInManhattenRange(Location location, int manhattenRange, KdNode<T> currentNode, int depth)
        {
            var dimension = depth % Dimensions;
            var squaredDistance = distanceCalculator.ManhattenDistance(location, currentNode.Value);

            List<T> nodesInRAnge = new List<T>();

            if (squaredDistance <= manhattenRange)
            {
                nodesInRAnge.Add(currentNode.Value);
            }

            // check the subtree for nodes nearer
            var nearestChildNode = location[dimension] >= currentNode.Value[dimension]
                                       ? currentNode.Right
                                       : currentNode.Left;

            if (nearestChildNode != null)
            {
                // will return the current best if no better is found
                nodesInRAnge.AddRange(FindNodesInManhattenRange(location, manhattenRange, nearestChildNode, dimension + 1));
            }

            // check whether hypersphere collisdes with hyperplane
            if (manhattenRange > Math.Abs(location[dimension] - currentNode.Value[dimension]))
            {
                var farestChildNode = nearestChildNode == currentNode.Left ? currentNode.Right : currentNode.Left;

                if (farestChildNode != null)
                {
                    nodesInRAnge.AddRange(FindNodesInManhattenRange(location, manhattenRange, farestChildNode, dimension + 1));
                }
            }

            return nodesInRAnge;
        }

        public List<KdNode<T>> GetAllNodes()
        {
            return GetAllNodes(this.Root);
        }

        public List<KdNode<T>> GetAllNodes(KdNode<T> currentNode)
        {
            List<KdNode<T>> nodes = new List<KdNode<T>>();

            if (currentNode != null)
            {
                nodes.Add(currentNode);

                if (currentNode.Left != null)
                {
                    nodes.AddRange(GetAllNodes(currentNode.Left));
                }

                if (currentNode.Right != null)
                {
                    nodes.AddRange(GetAllNodes(currentNode.Right));
                }
            }

            return nodes;
        }
    }
}
