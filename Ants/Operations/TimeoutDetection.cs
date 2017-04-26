using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ants.Operations
{
    public class TimeoutDetection : AntOperation
    {
        private List<IGrouping<int, AntLoc>> previousEnemiesGroupedByTeam;
        private readonly Dictionary<int, bool> isTimeout = new Dictionary<int, bool>();
        public bool IsAnyoneTimeout { get;private set; }
        private const double Threshold = 0.9;

        public TimeoutDetection(Bot bot) : base(bot)
        {

        }

        public bool IsTimeout(int team)
        {
            return isTimeout.ContainsKey(team) && isTimeout[team];
        }

        public override void ExecuteOperation(List<AntLoc> availableAnts)
        {
            DetectTimeout();

            KillTimeouts();
        }

        private void KillTimeouts()
        {
            var teamsWithTimeout = isTimeout.Where(k => k.Value).Select(k => k.Key);
            var antsWithTimeout = this.Bot.State.EnemyAnts.Where(ant => teamsWithTimeout.Contains(ant.Team));

        }



        private void DetectTimeout()
        {
            bool firstRound = previousEnemiesGroupedByTeam == null;
            var enemies = this.Bot.State.EnemyAnts;
            var currentEnemiesGroupedByTeam = enemies.GroupBy(e => e.Team).ToList();

            if (!firstRound)
            {
                foreach (IGrouping<int, AntLoc> grouping in previousEnemiesGroupedByTeam)
                {
                    var newGroup = currentEnemiesGroupedByTeam.FirstOrDefault(f => f.Key == grouping.Key);

                    // enemy just disappeared
                    if (newGroup == null)
                        continue;

                    double totalAnts = grouping.Count();
                    double antsInSameSpot = 0;// grouping.Count(s => (newGroup as IQueryable<AntLoc>).Contains<AntLoc>(s)) + Bot.State.DeadTiles.Count(s => (newGroup as IQueryable<AntLoc>).Contains<Location>(s));

                    foreach (AntLoc antLoc in grouping)
                    {
                        if (newGroup.Contains(antLoc))
                        {
                            antsInSameSpot++;
                        }
                        if (Bot.State.DeadAnts.Contains(antLoc))
                            antsInSameSpot++;
                    }
                    

                    if (antsInSameSpot/totalAnts > Threshold)
                    {
                        isTimeout[grouping.Key] = true;
                        IsAnyoneTimeout = true;
                    }
                    else
                    {
                        isTimeout[grouping.Key] = false;
                    }
                }
            }

            previousEnemiesGroupedByTeam = currentEnemiesGroupedByTeam;
        }
    }
}
