using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ants
{
    public class Attack
    {
        private readonly GameState state;
        public List<Location> AttackOffsets = new List<Location>(); 

        public Attack(GameState state)
        {
            this.state = state;
            var attackRadius = state.AttackRadius;
            var mid = new Location(attackRadius, attackRadius);
            for (int x = 0; x < state.AttackRadius * 2 + 1; x++)
            {
                for (int y = 0; y < state.AttackRadius * 2 + 1; y++)
                {
                    var loc = new Location(x, y);

                    if (state.DonutDistances.SquaredDistance(loc, mid) <= state.AttackRadius2)
                    {
                        AttackOffsets.Add(mid - loc);
                    }
                }
            }
        }

        public void AttackMove(Bot bot, Location from, Location to)
        {
            var potentialEnemies = bot.Enemies.FindNodesInRange(to, state.AttackRadius2).ToList();

            if (potentialEnemies.Count > 1)
            {
                bot.SafeMove(from, to);
            }
            else
            {
                
            }
        }

        public IEnumerable<Location> GetAttackOffsets(Location loc)
        {
            return AttackOffsets.Select(offset => new Location((loc.Col + offset.Col + state.Width) % state.Width, (loc.Row + offset.Row + state.Height) % state.Height)).Where(l => state[l.Col, l.Row] != Tile.Water).ToList();
        }

        public DataStructures.Tuple<int[,],Dictionary<Location, List<Location>>>  GeneratePotentialFocusMap(PathFinding pathFinding, List<AntLoc> ants)
        {
            Dictionary<Location, List<Location>> attackSources = new Dictionary<Location, List<Location>>();
            var focusMap = new int[state.Width, state.Height];

            foreach (AntLoc ant in ants)
            {
                var position = ant;
                var possibleMoveLocations = pathFinding.GetNeighbours(position).Concat(new[] {position}).Where(l => state[l.Col, l.Row] != Tile.Water);

                List<DataStructures.Tuple<Location, Location>> attackLocations = new List<DataStructures.Tuple<Location, Location>>();


                foreach (var possibleMoveLocation in possibleMoveLocations)
                {
                    var attackOFfsets = GetAttackOffsets(possibleMoveLocation);

                    attackLocations.AddRange(attackOFfsets.Select(attackOFfset => new DataStructures.Tuple<Location, Location>(possibleMoveLocation, attackOFfset)));
                }


                foreach (var attackPosition in attackLocations.Select(a => a.Item2).Distinct())
                {
                    focusMap[attackPosition.Col, attackPosition.Row] += 1;
                }


                foreach (var attackLocation in attackLocations)
                {

                    if (!attackSources.ContainsKey(attackLocation.Item2))
                    {
                        attackSources[attackLocation.Item2] = new List<Location>();
                    }

                    attackSources[attackLocation.Item2].Add(attackLocation.Item1);
                }
            }

            return new DataStructures.Tuple<int[,], Dictionary<Location, List<Location>>>(focusMap, attackSources);
        }


        public int GenerateFocusMap(Location location, IEnumerable<Location> ants)
        {
            return ants.Count(ant => state.DonutDistances.SquaredDistance(ant, location) <= state.AttackRadius2);
        }
    }
}
