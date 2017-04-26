using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace Ants
{
    public class AntsParser
    {
        private readonly Logging logger;

        private const string Ready = "ready";
        private const string Go = "go";
        private const string End = "end";
        public static readonly Location North = new Location(0,-1);
        public static readonly Location South = new Location(0,1);
        public static readonly Location West = new Location(-1,0);
        public static readonly Location East = new Location(1,0);

        public static IDictionary<char, Location> Aim = new Dictionary<char, Location>
                                                            {
                                                                {'n', North},
                                                                {'e', East},
                                                                {'s', South},
                                                                {'w', West}
                                                            };

        private GameState state;

        public AntsParser(string logName)
        {
            
            logger = new Logging(logName);
        }


        public void PlayGame(Bot bot)
        {
            string line = "";
            var lines = new List<string>();

            int c;
            
            while ((c = Console.Read()) >= 0)
            {
               
                if (c == '\n')
                    logger.LogInput('\r');

                logger.LogInput((char)c);
                if (c == '\n')
                {
                    if (line.StartsWith(Ready, StringComparison.InvariantCultureIgnoreCase))
                    {
                        ParseSetup(lines);
                        FinishTurn();
                        lines.Clear();
                        bot.Initialize(state);
                    }
                    else if (line.StartsWith(Go, StringComparison.InvariantCultureIgnoreCase))
                    {
                        logger.LogTurnEnd();
                        state.StartNewTurn();
                        ParseUpdate(lines);
                        bot.DoTurn(state);
                        FinishTurn();
                        lines.Clear();
                    }
                    else if (line.StartsWith(End, StringComparison.InvariantCultureIgnoreCase))
                    {
                        ParseEndInformation();
                        lines.Clear();
                    }
                    else
                    {
                        lines.Add(line);
                    }
                    line = string.Empty;
                }
                else
                {
                    // fix to handle \r\n new line based systems (ie windows)
                    line += ((char) c).ToString().Replace("\r", "");
                }
            }
        }

        private void ParseEndInformation()
        {
            FinishTurn();
        }

        // parse initial input and setup starting game state
        private void ParseSetup(IEnumerable<string> input)
        {
            int width = 0;
            int height = 0;
            int turntime = 0;
            int loadtime = 0;
            int viewradius2 = 0;
            int attackradius2 = 0;
            int spawnradius2 = 0;

            int parametersLoaded = 0;

            foreach (string line in input)
            {
                if (string.IsNullOrEmpty(line)) continue;

                string[] tokens = line.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                string key = tokens[0];

                if (key.Equals(@"cols"))
                {
                    width = int.Parse(tokens[1], CultureInfo.InvariantCulture);
                    parametersLoaded++;
                }
                else if (key.Equals(@"rows"))
                {
                    height = int.Parse(tokens[1], CultureInfo.InvariantCulture);
                    parametersLoaded++;
                }
                else if (key.Equals(@"turntime"))
                {
                    turntime = int.Parse(tokens[1], CultureInfo.InvariantCulture);
                    parametersLoaded++;
                }
                else if (key.Equals(@"loadtime"))
                {
                    loadtime = int.Parse(tokens[1], CultureInfo.InvariantCulture);
                    parametersLoaded++;
                }
                else if (key.Equals(@"viewradius2"))
                {
                    viewradius2 = int.Parse(tokens[1], CultureInfo.InvariantCulture);
                    parametersLoaded++;
                }
                else if (key.Equals(@"attackradius2"))
                {
                    attackradius2 = int.Parse(tokens[1], CultureInfo.InvariantCulture);
                    parametersLoaded++;
                }
                else if (key.Equals(@"spawnradius2"))
                {
                    spawnradius2 = int.Parse(tokens[1], CultureInfo.InvariantCulture);
                    parametersLoaded++;
                }
            }

            if (parametersLoaded != 7)
            {
                throw new InvalidOperationException("Not enough parameters was supplied during initialization");
            }

            state = new GameState(width, height,
                                  turntime, loadtime,
                                  viewradius2, attackradius2, spawnradius2);
        }

        // parse engine input and update the game state
        private void ParseUpdate(IEnumerable<string> input)
        {
            // do some stuff first

            foreach (string line in input.OrderByDescending(s => s[0])) // important that we see dead ants before adding new ants
            {
                if (line.Length <= 0) continue;

                string[] tokens = line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                if (tokens.Length > 4 || tokens.Length < 3)
                {
                    continue;
                }
                int row = int.Parse(tokens[1], CultureInfo.InvariantCulture);
                int col = int.Parse(tokens[2], CultureInfo.InvariantCulture);

                if (tokens[0].Equals("a"))
                {
                    state.AddAnt(row, col, int.Parse(tokens[3], CultureInfo.InvariantCulture));
                }
                else if (tokens[0].Equals("f"))
                {
                    state.AddFood(row, col);
                }
                else if (tokens[0].Equals("r"))
                {
                    state.RemoveFood(row, col);
                }
                else if (tokens[0].Equals("w"))
                {
                    state.AddWater(row, col);
                }
                else if (tokens[0].Equals("d"))
                {
                    state.DeadAnt(row, col, int.Parse(tokens[3], CultureInfo.InvariantCulture));
                }
                else if (tokens[0].Equals("h"))
                {
                    state.AddHill(row, col, int.Parse(tokens[3], CultureInfo.InvariantCulture));
                }
            }

            state.Update();
        }
        private static void FinishTurn()
        {
            Console.Out.WriteLine(Go);
            Console.Out.Flush();
        }
    }
}
