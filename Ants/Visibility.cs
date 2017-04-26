using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Ants.DataStructures.BinarySpacePartitioning;

namespace Ants
{
    public class Visibility
    {
        enum VisibilityChange
        {
            NowVisible,
            NoLongerVisible
        }

        //private readonly Logging internalLogger = new Logging("VisibilityLogging");

        private readonly Dictionary<Location, int> turnsSinceLastSeen;
        private GameState gameState;
        public readonly ReadOnlyCollection<Location> VisibilityOffsets;

        private readonly int[][] viewPressure;

        public KdTree<Location> InvisibleSpotsTree; 

        public Visibility(GameState state)
        {
            InvisibleSpotsTree =new KdTree<Location>(state.DonutDistances);

            var viewOffsets = new List<Location>();
            var kek = state.ViewRadius;
            var mid = new Location(kek, kek);
            for (int x = 0; x < state.ViewRadius *2+1; x++)
            {
                for (int y = 0; y < state.ViewRadius *2+1; y++)
                {
                    var loc = new Location(x, y);

                    if (state.DonutDistances.SquaredDistance(loc, mid) <= state.ViewRadius2)
                    {
                        viewOffsets.Add(mid - loc);
                    }
                }
            }

            VisibilityOffsets = viewOffsets.AsReadOnly();

            viewPressure = new int[state.Width][];
            for (int i = 0; i < state.Width; i++)
            {
                viewPressure[i] = new int[state.Height];
            }

            turnsSinceLastSeen = new Dictionary<Location, int>();
            for (int col = 0; col < state.Width; col++)
            {
                for (int row = 0; row < state.Height; row++)
                {
                    turnsSinceLastSeen[new Location(col, row)] = 5000;
                    InvisibleSpotsTree.Add(new Location(col,row));
                }
            }
        }

        private void UpdateFieldVisibility(Location loc, VisibilityChange change)
        {
            if (change == VisibilityChange.NoLongerVisible)
                viewPressure[loc.Col][loc.Row] -= 1;
            else
                viewPressure[loc.Col][loc.Row] += 1;

            var newPressure = viewPressure[loc.Col][loc.Row];
#if DEBUG
            if (newPressure < 0)
                throw new InvalidOperationException("VISILBBITY ISSUE PRESSURE < 0");
#endif

            if (newPressure == 1 && change == VisibilityChange.NowVisible)
            {
                InvisibleSpotsTree.Remove(loc);
            }
            else if (newPressure == 0 && change == VisibilityChange.NoLongerVisible)
            {
                InvisibleSpotsTree.Add(loc);
            }
        }

        public void Update(GameState state)
        {
            this.gameState = state;

            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            // foreach new ant
            foreach (var newAnt in gameState.NewAnts)
            {
                //internalLogger.Log(string.Format("[{1}]Updating ant spwan on {0}", newAnt, this.gameState.Turn));
                var visibleFields = GetViewFields(newAnt);

                foreach (var visibleField in visibleFields)
                {
                    UpdateFieldVisibility(visibleField, VisibilityChange.NowVisible);
                }
            }

            // remove vision from dead ants
            foreach (var deadAnt in gameState.DeadAnts.Where(d => d.Team == 0))
            {
               // internalLogger.Log(string.Format("[{1}]Updating ant death on {0}", deadAnt, this.gameState.Turn));
                var visibleFields = GetViewFields(deadAnt);

                foreach (var visibleField in visibleFields)
                {
                    UpdateFieldVisibility(visibleField, VisibilityChange.NoLongerVisible);
                }
            }

    

            // update turns since last seen
            for (int col = 0; col < gameState.Width; col++)
            {
                for (int row = 0; row < gameState.Height; row++)
                {
                    if (gameState[col,row] == Tile.Water) continue;
                    var loc = new AntLoc(row, col, 0);
                    var isVisible = viewPressure[col][row] > 0;
                    if (isVisible)
                    {
                        turnsSinceLastSeen[loc] = 0;

                    }
                    else
                    {
                        turnsSinceLastSeen[loc] += 1;
                    }
                }
            }
        }

        public static void ComputeVisibilityUsingKdTree(GameState state, List<Location> invisibleSpots, List<Location> visibleSpots, Dictionary<Location, int> turnsSinceLastSeen)
        {
            var tree = new KdTree<AntLoc>(state.DonutDistances, state.MyAnts);
            var squaredViewRadius = state.ViewRadius2;
            for (int col = 0; col < state.Width; col++)
            {
                for (int row = 0; row < state.Height; row++)
                {
                    if (state[col, row] == Tile.Water) continue;
                    var loc = new AntLoc(row, col,0);
                    var nearestNeighbour = tree.FindNearestNeighbour(loc,false);

                    var isVisible = nearestNeighbour.Item2 <= squaredViewRadius;
                    if (isVisible)
                    {
                        visibleSpots.Add(loc);
                        turnsSinceLastSeen[loc] = 0;

                    }
                    else
                    {
                        invisibleSpots.Add(loc);
                        turnsSinceLastSeen[loc] += 1;
                    }
                }
            }
        }

        public IEnumerable<Location> GetViewFields(Location loc)
        {
            return VisibilityOffsets.Select(offset => new Location((loc.Col + offset.Col + gameState.Width) % gameState.Width, (loc.Row + offset.Row + gameState.Height) % gameState.Height)).ToList();
        }

        public int TurnsSinceLastSeen(Location location)
        {
            return turnsSinceLastSeen[location];
        }

        public void UpdateAntMove(Location from,Location to)
        {
            //internalLogger.Log(string.Format("[{2}]Updating ant move from {0} to {1}", from, to, this.gameState.Turn));
            // update view pressure with movement
            var previousVisibleFields = GetViewFields(from).ToList();
            var currentVisibleFields = GetViewFields(to).ToList();

            var previousUnique = previousVisibleFields.Except(currentVisibleFields);
            var currentUnique = currentVisibleFields.Except(previousVisibleFields);

            foreach (var location in currentUnique)
            {
                UpdateFieldVisibility(location, VisibilityChange.NowVisible);
            }

            foreach (var location in previousUnique)
            {
                UpdateFieldVisibility(location, VisibilityChange.NoLongerVisible);
            }
        }

        public bool IsLocationVisible(Location location)
        {
            return this.viewPressure[location.Col][location.Row] > 0;
        }
    }
}