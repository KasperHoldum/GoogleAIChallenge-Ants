using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Ants.DataStructures;
using Ants.DataStructures.BinarySpacePartitioning;
using Ants.DataStructures.HPA;

namespace Ants
{
    public abstract class Bot
    {
        public Logging Log { get; private set; }
        public ContainsList<Location> HasAntMovedToThisLocation { get; set; }
        public ContainsList<AntLoc> HasAntMoved { get; set; }
        public PathFinding PathFinding { get; set; }
        public GameState State { get; set; }
        public KdTree<AntLoc> Enemies { get; set; }
        public List<AntOperation> Operations = new List<AntOperation>();
        public Attack Attack { get; set; }

        public readonly Logging TimeLogging;

        public virtual void DoTurn(GameState gameState)
        {
            TimeLogging.Log(string.Format("[{0}] turn: {1}", DateTime.Now, gameState.Turn));
            Update(gameState);

            foreach (var antOperation in Operations)
            {
                DateTime before = DateTime.UtcNow;
                var availableAnts = this.AvailableAnts();
                antOperation.ExecuteOperation(availableAnts);
                var tookTime = DateTime.UtcNow - before;

                TimeLogging.Log(string.Format("[{0} ms] {1} finished", (int)tookTime.TotalMilliseconds, antOperation.GetType().Name));
            }
        }
        protected Bot(string name)
        {
            this.HasAntMovedToThisLocation = new ContainsList<Location>();
            this.Log = new Logging(name);
            this.TimeLogging = new Logging(name + " - time");
        }

        protected void IssueOrder(Location loc, char direction)
        {
            Log.Log("Issuing order: " + string.Format("o {0} {1} {2}", loc.Row, loc.Col, direction));
            Console.Out.WriteLine(string.Format(CultureInfo.InvariantCulture, "o {0} {1} {2}", loc.Row, loc.Col,
                                                direction));
        }

        public virtual void Initialize(GameState loadTime)
        {
            this.State = loadTime;
        }

        public List<AntLoc> AvailableAnts()
        {
            return State.MyAnts.Except(HasAntMoved).ToList();
        }

        public virtual void Update(GameState gameState)
        {
            State = gameState;

            if (PathFinding == null)
            {
                PathFinding = new AStarPathFinding(State); //new HierarchicalPathFindingAStar(State, 10);
            }
            else
            {
                PathFinding.Update(State);
            }

            HasAntMoved = new ContainsList<AntLoc>();
            HasAntMovedToThisLocation = new ContainsList<Location>();

            Enemies = new KdTree<AntLoc>(State.DonutDistances, State.EnemyAnts);
        }



        public bool MoveAnt(AntLoc from, Location to)
        {
#if DEBUG
            if (State.DonutDistances.ManhattenDistance(from, to) > 1)
            {
                throw new ArgumentException("An invalid order was made. Move from " + from + " to " + to + " was invalid", "to");
            }

            if (HasAntMoved.Contains(from))
                throw new ArgumentException("ANT HAS ALREADY MOVED WTF?!?!?!", "from");
#endif


            bool issuedOrder = false;
            Location desti = State.Destination(from, State.Direction(from, to).First());
            if (State.IsUnoccupied(desti) && !HasAntMovedToThisLocation[desti])
            {
                issuedOrder = true;
                HasAntMovedToThisLocation[desti] = true;
                State[from.Col, from.Row] = Tile.Land;
                IssueOrder(from, State.Direction(from, to).First());
                State.Visibility.UpdateAntMove(from,to);
                AntRegistry.RegisterMove(from, to);
                HasAntMoved[from] = true;

            }
            return issuedOrder;
        }

        public DataStructures.Tuple<bool, Location> IsSafe(AntLoc closestAnt, Location nextStep)
        {
            var dangerZone = Math.Pow(Math.Sqrt(State.AttackRadius2) + 1, 2);
            bool isNewLocationSafe = Enemies.FindNodesInRange(nextStep, dangerZone).Count() == 0;

            if (isNewLocationSafe)
                return new DataStructures.Tuple<bool, Location>(true, nextStep);

            //bool isCurrentLocationSafe = Enemies.FindNodesInRange(closestAnt, this.State.AttackRadius2).Count() == 0;
            var newPossibleLocations = this.PathFinding.GetNeighbours(closestAnt).Where(l => this.State.IsUnoccupied(l));

            var newSafeLocation =
                newPossibleLocations.FirstOrDefault(loc => Enemies.FindNodesInRange(loc, dangerZone).Count() == 0);

            if (newSafeLocation != null)
            {
                var succcesfullyMoved = MoveAnt(closestAnt, newSafeLocation);
                return new DataStructures.Tuple<bool, Location>(false, succcesfullyMoved ? newSafeLocation : null);
            }

            return new DataStructures.Tuple<bool, Location>(false, null);
        }

        public void PathFindMove(AntLoc from, Location to, bool canMoveOnOwnAnts = false, bool moveSafe = false)
        {
            Path fullPath = PathFinding.FindPath(from, to, canMoveOnOwnAnts);

            // this should not happen either, as a path was found on line 88 using FindClosestEntity
            if (fullPath == null || fullPath.IsFinished)
            {
                // unreachable item or we are already next to it
                Log.Log("Could not find path between " + from + " and " + to);
                return;
            }

            Location nextStep = fullPath[1];

            if (!moveSafe || IsSafe(from, nextStep).Item1)
            {
                MoveAnt(from, nextStep);
            }
        }

        public void SafeMove(Location @from, Location to)
        {
            throw new NotImplementedException();
        }

        public void OccupyAnt(AntLoc antLoc)
        {
            HasAntMovedToThisLocation[antLoc] = true;
            HasAntMoved[antLoc] = true;
        }
    }
}