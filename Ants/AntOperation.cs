// Ants -> IAntOperation.cs
//  2011

using System.Collections.Generic;

namespace Ants
{
    public abstract class AntOperation
    {
        protected AntOperation(Bot bot)
        {

            this.Bot = bot;
        }

        protected Bot Bot { get; set; }

        public abstract void ExecuteOperation(List<AntLoc> availableAnts);

    }
}