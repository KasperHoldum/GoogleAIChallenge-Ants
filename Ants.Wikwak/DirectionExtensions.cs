using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ants.Wikwak
{
    public static class DirectionExtensions
    {
        public static Direction GetRandomDirection()
        {
            var random = new Random();

            switch (random.Next(0,3))
            {
                case 0:
                    return Direction.East;
                case 1:
                    return Direction.North;
                case 2:
                    return Direction.South;
                case 3:
                    return Direction.West;
            }

            throw new Exception("Could not find random direction.");
        }
    }
}