using System;
using System.Collections.Generic;

namespace Ants
{
    public abstract class PathFinding
    {
        protected readonly Dictionary<Location, List<Location>> DemNeighbours = new Dictionary<Location, List<Location>>();
        public List<Location> GetNeighbours(Location loc)
        {
            return DemNeighbours[loc];
        }

        protected PathFinding(GameState gameState)
        {
            GameState = gameState;

            // cache neighbours
            for (int x = 0; x < gameState.Width; x++)
            {
                for (int y = 0; y < gameState.Height; y++)
                {
                    Location location = new Location(x, y);
                    var neighbours = new List<Location>();

                    Location north = GameState.Destination(location, 'n');
                    Location south = GameState.Destination(location, 's');
                    Location east = GameState.Destination(location, 'e');
                    Location west = GameState.Destination(location, 'w');
                    neighbours.Add(north);
                    neighbours.Add(south);
                    neighbours.Add(east);
                    neighbours.Add(west);
                    DemNeighbours[location] = neighbours;
                }
            }
        }

        public GameState GameState { get; private set; }

        public abstract Path FindPath(Location start, Location goal, bool canMoveOnAntsAndFood = false, double distanceToGoalEpsilon = 1.0000001);
        public virtual T FindClosestEntity<T>(Location l1, Dictionary<Location, T> entities)
        {
            throw new NotImplementedException();
        }

        public virtual void Update(GameState state)
        {
            GameState = state;
        }
    }
}