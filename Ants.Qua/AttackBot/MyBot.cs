using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ants.DataStructures;
using Ants.DataStructures.BinarySpacePartitioning;
using Ants.Operations;
using Ants.Operations.Defence;
using Ants.Operations.FindFood;
using Ants.Operations.SpreadOut;

namespace Ants.Qua.AttackBot
{
    public class MyBot : Bot
    {
        public MyBot()
            : base("AttackBot")
        {

            this.Operations.Add(new DieNowOperation(this));
            this.Operations.Add(new NoTimeoutOperation(this));
            this.Operations.Add(new CapThemHills(this));
            this.Operations.Add(new SimpleHillDefence(this));
            this.Operations.Add(new ImprovedStableMarriageFindFood(this));
            this.Operations.Add(new VisibilitySpreadOut(this));
            this.Operations.Add(new DontBlockHillOperation(this));

        }
    }

    public class NoTimeoutOperation : AntOperation
    {
        public NoTimeoutOperation(MyBot myBot)
            : base(myBot)
        {


        }

        public override void ExecuteOperation(List<AntLoc> availableAnts)
        {
            Random r = new Random();

            const int maxAntControl = 80;

            int antsToRemove = Math.Max(availableAnts.Count - maxAntControl, 0);
            for (int i = 0; i < antsToRemove; i++)
            {
                this.Bot.HasAntMoved[availableAnts[i]] = true;
                this.Bot.HasAntMovedToThisLocation[availableAnts[i]] = true;
            }

        }
    }

    public class CapThemHills : AntOperation
    {
        public CapThemHills(MyBot myBot)
            : base(myBot)
        {


        }

        public override void ExecuteOperation(List<AntLoc> availableAnts)
        {

            var enemyHills = this.Bot.State.EnemyHills;
            var enemyHillsTree = new KdTree<Location>(this.Bot.State.DonutDistances, enemyHills);

            var initalInactiveAntCount = availableAnts.Count;
            var viewFactor = Math.Max(initalInactiveAntCount / 10.0, 1);
            foreach (AntLoc inactiveAnt in availableAnts)
            {
                var closestEnemyHill = enemyHillsTree.FindNodesInRange(inactiveAnt, this.Bot.State.ViewRadius2 * viewFactor).FirstOrDefault();

                if (closestEnemyHill != null)
                {
                    var fullPath = this.Bot.PathFinding.FindPath(inactiveAnt, closestEnemyHill);

                    if (fullPath == null || fullPath.IsFinished)
                    {
                        // unreachable item or we are already next to it
                        this.Bot.Log.Log("Could not find path between " + inactiveAnt + " and " + closestEnemyHill);
                        continue;
                    }


                    Location nextStep = fullPath.NextLocation;

                    if (this.Bot.Enemies.FindNodesInManhattenRange(closestEnemyHill, 5).Count() < 3)
                    {
                        this.Bot.MoveAnt(inactiveAnt, nextStep);
                    }

                    else
                    {
                        if (this.Bot.IsSafe(inactiveAnt, nextStep).Item1)
                        {
                            this.Bot.MoveAnt(inactiveAnt, nextStep);
                        }
                    }
                }
            }
        }
    }

    public class DieNowOperation : AntOperation
    {
        private Attack attack;

        public DieNowOperation(MyBot myBot)
            : base(myBot)
        {
        }

        public override void ExecuteOperation(List<AntLoc> availableAnts)
        {
            var myAntsTree = new KdTree<AntLoc>(this.Bot.State.DonutDistances, availableAnts);

            if (attack == null)
            {
                attack = new Attack(this.Bot.State);
            }

            var poentialResult = attack.GeneratePotentialFocusMap(this.Bot.PathFinding, this.Bot.State.EnemyAnts);
            var potentialPressure = poentialResult.Item1;
            var attackSource = poentialResult.Item2;

            var blockedAnts = new Dictionary<AntLoc, Location>();

            foreach (var antLoc in this.Bot.State.MyAnts)
            {
                // ant already moved, try to assist it?
                if (blockedAnts.ContainsKey(antLoc))
                {
                    //var assistance = GetAssistance(myAntsTree, blockedAnts[antLoc], antLoc, attackSource);

                    //if (assistance != null)
                    //{
                    //    this.Bot.MoveAnt(assistance.Item1, assistance.Item2);

                    //    blockedAnts.Add(assistance.Item1, assistance.Item2);
                    //    myAntsTree.Remove(assistance.Item1);
                    //}
                    continue;
                }

                if (potentialPressure[antLoc.Col, antLoc.Row] == 1)
                {
                    var assistance = GetAssistance(myAntsTree, antLoc, antLoc, attackSource, potentialPressure);

                    if (assistance != null)
                    {
                        if (assistance.Item2 != antLoc)
                        {
                            this.Bot.MoveAnt(assistance.Item1, assistance.Item2);
                            this.Bot.OccupyAnt(antLoc);
                            blockedAnts.Add(assistance.Item1, assistance.Item2);
                            myAntsTree.Remove(antLoc);
                            myAntsTree.Remove(assistance.Item1);
                        }
                    }
                }
                else
                {
                    var newLocations = this.Bot.PathFinding.GetNeighbours(antLoc).Where(l => this.Bot.State.IsWalkable(l));

                    foreach (var newLocation in newLocations)
                    {
                        if (potentialPressure[newLocation.Col, newLocation.Row] == 1)
                        {
                            var assistance = GetAssistance(myAntsTree, newLocation, antLoc, attackSource, potentialPressure);

                            if (assistance != null)
                            {
                                if (assistance.Item2 == antLoc)
                                {
                                    this.Bot.MoveAnt(antLoc, newLocation);
                                    this.Bot.MoveAnt(assistance.Item1, assistance.Item2);
                                }
                                else
                                {
                                    this.Bot.MoveAnt(assistance.Item1, assistance.Item2);
                                    this.Bot.MoveAnt(antLoc, newLocation);
                                }

                                blockedAnts.Add(assistance.Item1, assistance.Item2);
                                myAntsTree.Remove(antLoc);
                                myAntsTree.Remove(assistance.Item1);
                                break;
                            }
                        }
                    }
                }

            }
        }


        private Tuple<AntLoc, Location> GetAssistance(KdTree<AntLoc> myAntsTree, Location newLocation, AntLoc ant, Dictionary<Location, List<Location>> attackSource, int[,] potentialPressure)
        {

            foreach (var sourceField in attackSource[newLocation])
            {
                var possibleAnts = myAntsTree.FindNodesInManhattenRange(sourceField, 4).ToList();

                foreach (AntLoc possibleAnt in possibleAnts.Where(s => s != sourceField))
                {
                    if (possibleAnt == ant)
                        continue;



                    var desti = this.Bot.State.Destination(possibleAnt,
                                                           this.Bot.State.Direction(possibleAnt, sourceField).First());



                    if (desti != newLocation && this.Bot.State.IsWalkable(desti))
                    {
                        return new DataStructures.Tuple<AntLoc, Location>(possibleAnt, desti);
                    }
                }
            }

            return null;
        }
    }
}
