using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ants.DataStructures.BinarySpacePartitioning;

namespace Ants.Operations.Attack
{
    public class StupidAttack : AntOperation
    {
        public StupidAttack(Bot bot) : base(bot)
        {
        }

        public override void ExecuteOperation(List<AntLoc> availableAnts)
        {
            var inactiveAnts = this.Bot.State.MyAnts.Except(this.Bot.HasAntMoved).ToList();
            var myants = new KdTree<AntLoc>(this.Bot.State.DonutDistances, inactiveAnts);

            var inactiveAntsWithEnemiesNearby = inactiveAnts.Where(a => this.Bot.Enemies.FindNearestNeighbour(a).Item2 < this.Bot.State.ViewRadius);


            foreach (AntLoc antLoc in inactiveAntsWithEnemiesNearby)
            {
                //var inactiveAntsNearby = myants.FindNodesInRange(antLoc, Math.Pow(this.Bot.State.AttackRadius + 2, 2));

                var dangerousEnemies = Bot.Enemies.FindNodesInRange(antLoc, Math.Pow(this.Bot.State.AttackRadius + 1, 2)).ToList();

                var myFocus = this.Bot.Attack.GenerateFocusMap(antLoc, dangerousEnemies.Cast<Location>());
                var enemiesFocus = new List<DataStructures.Tuple<AntLoc, int>>();
                foreach (var dangerousEnemy in dangerousEnemies)
                {
                    var enemyEnemiesAnts =
                        Bot.State.MyAnts.Where(a => a.Team != dangerousEnemy.Team).ToList();
                    var enemyEnemiesAntsTree = new KdTree<AntLoc>(Bot.State.DonutDistances, enemyEnemiesAnts);
                    var dangerousEnemiesEnemy = enemyEnemiesAntsTree.FindNodesInRange(antLoc, Math.Pow(this.Bot.State.AttackRadius + 1, 2));
                    var enemyFocus = this.Bot.Attack.GenerateFocusMap(antLoc, dangerousEnemiesEnemy.Cast<Location>());

                    enemiesFocus.Add(new DataStructures.Tuple<AntLoc, int>(dangerousEnemy, enemyFocus));
                }


                bool dieOnCurrentSpot = myFocus >= enemiesFocus.Min(a => a.Item2);
                
                // check stand still
                if (dieOnCurrentSpot)
                {
                    // check escape options

                }
                else 
                {
                    //if (dieGoingTowardsEnemy)
                    //{
                    //    // stand still
                    //}
                    //else
                    //{
                    //    // move towards enemies
                    //}
                }


            }
        }
    }
}
