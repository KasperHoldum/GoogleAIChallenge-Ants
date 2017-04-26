using System.Collections.Generic;
using Ants.Operations.Attack;
using Ants.Operations.Defence;
using Ants.Operations.FindFood;
using Ants.Operations.SpreadOut;

namespace Ants.Qua.HillsHenningV1
{
    public class MyBot : Bot
    {
        public MyBot() 
            : base("HillsHenning V1")
        {
            this.Operations.Add(new SimpleHillDefence(this)); // defend
            this.Operations.Add(new ImprovedStableMarriageFindFood(this)); // eat
            this.Operations.Add(new MonitorOwnHills(this)); // monitor own hills
            this.Operations.Add(new SimpleCaptureHill(this)); // kill enemy hills
            this.Operations.Add(new VisibilitySpreadOut(this));
            this.Operations.Add(new FocusedAttack(this));
        }



    }

    public class FocusedAttack : AntOperation
    {
        public FocusedAttack(Bot myBot) : base(myBot)
        { 
            

        }

        public override void ExecuteOperation(List<AntLoc> availableAnts)
        {
            

        }
    }

    public class AttackEnemyHill : AntOperation
    {
        public AttackEnemyHill(Bot myBot)
            : base(myBot)
        {
            

        }

        public override void ExecuteOperation(List<AntLoc> availableAnts)
        {
        }
    }

    public class MonitorOwnHills : AntOperation
    {
        public MonitorOwnHills(Bot myBot)
            : base(myBot)
        {
            

        }

        public override void ExecuteOperation(List<AntLoc> availableAnts)
        {
        }
    }
}
