using System;

namespace Ants.DataStructures.HPA
{
    public static class ClusterRelativePositionExtensions
    {
        public static ClusterCollection.ClusterRelativePosition Opposite(this ClusterCollection.ClusterRelativePosition rr)
        {
            switch (rr)
            {
                case ClusterCollection.ClusterRelativePosition.Left:
                    return ClusterCollection.ClusterRelativePosition.Right;
                case ClusterCollection.ClusterRelativePosition.Right:
                    return ClusterCollection.ClusterRelativePosition.Left;
                case ClusterCollection.ClusterRelativePosition.Up:
                    return ClusterCollection.ClusterRelativePosition.Down;
                case ClusterCollection.ClusterRelativePosition.Down:
                    return ClusterCollection.ClusterRelativePosition.Up;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
