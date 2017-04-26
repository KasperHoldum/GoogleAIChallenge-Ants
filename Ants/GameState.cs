using System;
using System.Linq;
using System.Collections.Generic;

namespace Ants
{
    public class GameState
    {
        private readonly Tile[][] map;
        private DateTime turnStart;
        public readonly Visibility Visibility;
        public readonly DonutDistanceCalculator DonutDistances;

        public Tile this[int col,int row]
        {
            get { return map[col][row]; }
            set { map[col][row] = value; }
        }

        public GameState(int width, int height,
                         int turnTime, int loadTime,
                         int viewRadius2, int attackRadius2, int spawnRadius2)
        {

            Width = width;
            Height = height;

            this.DonutDistances = new DonutDistanceCalculator(this.Width, this.Height);

            LoadTime = loadTime;
            TurnTime = turnTime;

            ViewRadius2 = viewRadius2;
            AttackRadius2 = attackRadius2;
            SpawnRadius2 = spawnRadius2;
            ViewRadius = (int)Math.Sqrt(viewRadius2);
            AttackRadius = (int)Math.Sqrt(attackRadius2);
            SpawnRadius = (int)Math.Sqrt(spawnRadius2);

            MyAnts = new List<AntLoc>();
            EnemyAnts = new List<AntLoc>();
            DeadAnts = new List<AntLoc>();
            FoodTiles = new List<Location>();
            PreviousFood = new List<Location>();
            EnemyTeams = new List<int>();
            MyHills = new List<Location>();
            EnemyHills = new List<Location>();
            PreviousEnemyHills = new List<Location>();
            NewAnts = new List<AntLoc>();
            map = new Tile[width][];
            for (int col = 0; col < width; col++)
            {
                map[col] = new Tile[this.Height];
                for (int row = 0; row < height; row++)
                {
                    map[col][row] = Tile.Land;
                }
            }

            this.Visibility = new Visibility(this);

        }

        protected List<Location> PreviousEnemyHills { get; set; }

        public List<Location> EnemyHills { get; set; }

        public void Update()
        {
            Visibility.Update(this);

            // check which food has fallen out of line of sight
            var vanishedFood = PreviousFood.Except(FoodTiles);

            foreach (var location in vanishedFood)
            {
                if (!Visibility.IsLocationVisible(location))
                {
                    FoodTiles.Add(location);
                }

            }

            // check which food has fallen out of line of sight
            var vanishedHills = PreviousEnemyHills.Except(EnemyHills);

            foreach (var location in vanishedHills)
            {
                if (!Visibility.IsLocationVisible(location))
                {
                    EnemyHills.Add(location);
                }

            }

            foreach (var deadTile in DeadAnts.Where(deadTile => deadTile.Team == 0))
            {
                AntRegistry.RemoveAllData(deadTile);
            }
        }

        public List<AntLoc> DeadAnts { get; set; }
        public List<AntLoc> EnemyAnts { get; set; }
        public List<Location> FoodTiles { get; set; }
        public List<AntLoc> MyAnts { get; set; }

        public List<int> EnemyTeams { get; set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public int LoadTime { get; private set; }
        public int TurnTime { get; private set; }

        public int Turn { get; set; }

        public double TimeRemaining
        {
            get { return TurnTime - (DateTime.UtcNow - turnStart).TotalMilliseconds; }
        }

        public int ViewRadius2 { get; private set; }
        public int ViewRadius { get; private set; }
        public int AttackRadius2 { get; private set; }
        public int AttackRadius { get; private set; }
        public int SpawnRadius2 { get; private set; }
        public int SpawnRadius { get; private set; }

        public List<Location> PreviousFood { get; private set; }
        public void StartNewTurn()
        {
            // start timer
            turnStart = DateTime.UtcNow;

            // clear ant data
            DeadAnts.ForEach(t => map[t.Col][t.Row] = Tile.Land);
            EnemyAnts.ForEach(t => map[t.Col][t.Row] = Tile.Land);
            MyAnts.ForEach(t => map[t.Col][t.Row] = Tile.Land);

            MyAnts.Clear();
            EnemyAnts.Clear();
            DeadAnts.Clear();
            EnemyHills.Clear();
            MyHills.Clear();
            NewAnts.Clear();

            // save food from last round, so we can figure out which food is no longer visible
            PreviousFood.Clear();
            PreviousFood.AddRange(FoodTiles);

            PreviousEnemyHills.Clear();
            PreviousEnemyHills.AddRange(EnemyHills);

            PreviousEnemyHills.Clear();

            // set all known food to unseen
            FoodTiles.ForEach(t => map[t.Col][t.Row] = Tile.Land);
            FoodTiles.Clear();

            Turn++;
        }

        public List<AntLoc> NewAnts { get; set; }

        public void AddAnt(int row, int col, int team)
        {
            map[col][row] = Tile.Ant;

            var ant = new AntLoc(row, col, team);
            if (team == 0)
            {
                MyAnts.Add(ant);
                if (AntRegistry.RegisterAnt(ant) == AntRegistry.RegisterStatus.AntRegistered || DeadAnts.Any(a => a == ant))
                {
                    NewAnts.Add(ant);
                }
            }
            else
            {
                if (!EnemyTeams.Contains(team))
                    EnemyTeams.Add(team);
                EnemyAnts.Add(ant);
            }
        }

        public void AddFood(int row, int col)
        {
            FoodTiles.Add(new Location(col, row));
            map[col][row] = Tile.Food;
        }

        public void RemoveFood(int row, int col)
        {
            map[col][row] = Tile.Land;
            FoodTiles.Remove(new Location(col, row));
        }

        public void AddWater(int row, int col)
        {
            WaterCount += 1;
            map[col][row] = Tile.Water;
        }

        public int WaterCount { get; set; }

        public void DeadAnt(int row, int col, int owner)
        {
            // food could spawn on a spot where an ant just died
            // don't overwrite the space unless it is land
            if (map[col][row] == Tile.Land)
            {
                map[col][row] = Tile.Dead;
            }

            // but always add to the dead list
            var location = new AntLoc(row, col, owner);
            DeadAnts.Add(location);

            if (owner == 0)
                AntRegistry.RemoveAllData(location);
        }


        public bool IsWalkable(Location loc)
        {
            // true if not water
            return map[loc.Col][loc.Row] != Tile.Water;
        }

        public bool IsUnoccupied(Location loc)
        {
            // true if no ants are at the location
            return IsWalkable(loc) && map[loc.Col][loc.Row] != Tile.Ant && map[loc.Col][loc.Row] != Tile.Food;
        }

        public Location Destination(Location loc, char direction)
        {
            // calculate a new location given the direction and wrap correctly
            Location delta = AntsParser.Aim[direction];
            int row = (loc.Row + delta.Row + Height) % Height;
            int col = (loc.Col + delta.Col + Width) % Width;
            return new Location(col, row);
        }

        public ICollection<char> Direction(Location @from, Location to)
        {
            // determine the 1 or 2 fastest (closest) directions to reach a location
            var directions = new List<char>();

            if (@from.Row < to.Row)
            {
                if (to.Row - @from.Row >= Height / 2)
                    directions.Add('n');
                if (to.Row - @from.Row <= Height / 2)
                    directions.Add('s');
            }
            if (to.Row < @from.Row)
            {
                if (@from.Row - to.Row >= Height / 2)
                    directions.Add('s');
                if (@from.Row - to.Row <= Height / 2)
                    directions.Add('n');
            }

            if (@from.Col < to.Col)
            {
                if (to.Col - @from.Col >= Width / 2)
                    directions.Add('w');
                if (to.Col - @from.Col <= Width / 2)
                    directions.Add('e');
            }
            if (to.Col < @from.Col)
            {
                if (@from.Col - to.Col >= Width / 2)
                    directions.Add('e');
                if (@from.Col - to.Col <= Width / 2)
                    directions.Add('w');
            }

            return directions;
        }

        public List<Location> MyHills { get; set; }

        public void AddHill(int row, int col, int team)
        {
            var hill = new Location(col, row);
            if (team == 0)
            {
                MyHills.Add(hill);
            }
            else
            {
                EnemyHills.Add(hill);
            }
        }
    }
}