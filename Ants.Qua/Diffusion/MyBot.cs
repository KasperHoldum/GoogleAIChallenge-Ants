using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Ants.Qua.Diffusion
{
    public class MyBot : Bot
    {
        public MyBot()
            : base("Diffusion")
        {
        }


        public override void Initialize(GameState loadTime)
        {
            base.Initialize(loadTime);

            this.Operations.Add(new DiffusionOperation(this));
        }

    }

    public class DiffusionOperation : AntOperation
    {
        private const float FoodValue = 10000;
        private const float ExploreValue = 7500;

        private readonly Square[][] map;

        public DiffusionOperation(Bot bot)
            : base(bot)
        {
            map = new Square[this.Bot.State.Width][];
            for (int col = 0; col < this.Bot.State.Width; col++)
            {
                map[col] = new Square[this.Bot.State.Height];
                for (int row = 0; row < this.Bot.State.Height; row++)
                {
                    map[col][row] = new Square(new Location(col, row), bot);
                }
            }
        }

        public override void ExecuteOperation(List<AntLoc> availableAnts)
        {
            for (int i = 0; i < 15; i++)
            {
                for (int col = 0; col < this.Bot.State.Width; col++)
                {
                    for (int row = 0; row < this.Bot.State.Height; row++)
                    {
                        ComputeDiffusion(col, row);
                    }
                }
            }


            try
            {
               // PrintDiffusionMap();
            }
            catch (Exception)
            {
            }
            var directions = new[] { 'n', 's', 'e', 'w' };
            foreach (var availableAnt in availableAnts)
            {
                Square square = map[availableAnt.Col][availableAnt.Row];
                var foodValue = directions.Select(d => GetOffsetSquare(availableAnt, d)).OrderByDescending(s => s.Agents[GoalAgent.Food]).First().Agents[GoalAgent.Food];
                var exploreValue = square.OldAgents[GoalAgent.Explore];
                GoalAgent goal;
                if (foodValue * 2  > exploreValue)
                {
                    goal = GoalAgent.Food;
                }
                else
                    goal = GoalAgent.Explore;

                MoveTowardsGoal(availableAnt, goal);
            }

        }

        private void PrintDiffusionMap()
        {
            using (var bitmap = new Bitmap(this.Bot.State.Width, this.Bot.State.Height))
            {
                for (int i = 0; i < Bot.State.Width; i++)
                {
                    for (int j = 0; j < Bot.State.Height; j++)
                    {
                        if (this.Bot.State[i, j] == Tile.Water)
                        {
                            bitmap.SetPixel(i, j, Color.Pink);
                            continue;
                        }
                        if (this.Bot.State[i, j] == Tile.Food)
                        {
                            bitmap.SetPixel(i, j, Color.GreenYellow);
                            continue;
                        }
                        if (this.Bot.State[i, j] == Tile.Ant)
                        {
                            bitmap.SetPixel(i, j, Color.Cyan);
                            continue;
                        }

                        float explore = map[i][j].Agents[GoalAgent.Explore];
                        float food = map[i][j].Agents[GoalAgent.Food];

                        const double epsilon = 0.000000001;
                        int redValue = 0;
                        int blueValue = 0;
                        int greenValue = 0;

                        if (food > epsilon)
                        {
                            blueValue = Math.Min((int)((food / FoodValue) * 255), 255);
                        }
                        if (explore > epsilon)
                        {
                            redValue = Math.Min((int)((explore / ExploreValue) * 255), 255);
                        }

                        if (redValue == 0 && blueValue == 0 && greenValue == 0)
                        {
                            bitmap.SetPixel(i, j, Color.LightGoldenrodYellow);
                        }
                        else
                        {
                            bitmap.SetPixel(i, j, Color.FromArgb(redValue, greenValue, blueValue));
                        }


                    }
                }
                using (var gfx = Graphics.FromImage(bitmap))
                {
                    using (var font = new Font("Ariel", 6))
                    {
                        gfx.DrawString("Turn " + this.Bot.State.Turn, font, Brushes.HotPink, 10, 10);
                    }
                }
                bitmap.Save(string.Format("turn{0}.bmp", this.Bot.State.Turn));
            }
        }



        private void MoveTowardsGoal(AntLoc availableAnt, GoalAgent goal)
        {
            var directions = new[] { 'n', 's', 'e', 'w' };

            IEnumerable<KeyValuePair<char, float>> keyValuePairs = directions.Select(d => new KeyValuePair<char, float>(d, GetOffsetSquare(availableAnt, d).Agents[goal]));
            var direction = keyValuePairs.OrderByDescending(s => s.Value).First();

            this.Bot.MoveAnt(availableAnt, this.Bot.State.Destination(availableAnt, direction.Key));
        }

        private void ComputeDiffusion(int col, int row)
        {
            var square = map[col][row];
            square.StoreLastValue();
            var goalsToDiffuse = new List<GoalAgent>();
            // water
            if (this.Bot.State[col, row] == Tile.Water)
            {
                square.Nullify();
                return;
            }

            if (this.Bot.State[col, row] == Tile.Ant)
            {
                square.Nullify();
                return;
            }

            // food
            if (this.Bot.State[col, row] == Tile.Food)
            {
                square.Agents[GoalAgent.Food] = FoodValue;
            }
            else
            {
                goalsToDiffuse.Add(GoalAgent.Food);
            }

            // explore
            if (!square.IsVisible())
            {
                square.Agents[GoalAgent.Explore] = ExploreValue - (5001 - this.Bot.State.Visibility.TurnsSinceLastSeen(square.Location));
            }
            else
            {
                goalsToDiffuse.Add(GoalAgent.Explore);
            }

            foreach (var goal in goalsToDiffuse)
            {
                float up = GetOffsetSquare(col, row, 'n').OldAgents[goal];
                float down = GetOffsetSquare(col, row, 's').OldAgents[goal];
                float left = GetOffsetSquare(col, row, 'w').OldAgents[goal];
                float right = GetOffsetSquare(col, row, 'e').OldAgents[goal];

                square.Agents[goal] = 0.25f * (up + down + left + right);
            }
        }

        private Square GetOffsetSquare(int col, int row, char direction)
        {
            return GetOffsetSquare(new Location(col, row), direction);
        }

        private Square GetOffsetSquare(Location origin, char direction)
        {
            var loc = this.Bot.State.Destination(origin, direction);

            return this.map[loc.Col][loc.Row];
        }

        internal class Square
        {
            private readonly Bot bot;
            public Location Location { get; private set; }
            public readonly Dictionary<GoalAgent, float> Agents = new Dictionary<GoalAgent, float>();
            public readonly Dictionary<GoalAgent, float> OldAgents = new Dictionary<GoalAgent, float>();

            public Square(Location location, Bot bot)
            {
                this.bot = bot;
                this.Location = location;
                foreach (GoalAgent agent in Enum.GetValues(typeof(GoalAgent)))
                {
                    Agents[agent] = 0;
                }
                StoreLastValue();
            }

            public void Nullify()
            {
                foreach (GoalAgent key in Enum.GetValues(typeof(GoalAgent)))
                {
                    Agents[key] = 0;
                }

            }

            public bool IsVisible()
            {
                return this.bot.State.Visibility.IsLocationVisible(this.Location);
            }

            public void StoreLastValue()
            {
                foreach (var agent in Agents)
                {
                    OldAgents[agent.Key] = agent.Value;
                }
            }
        }

        internal enum GoalAgent
        {
            Food,
            Explore,
            Attack
        }
    }
}
