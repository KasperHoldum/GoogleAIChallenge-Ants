using System;

namespace Ants
{
    public class DonutDistanceCalculator : IDistanceCalculator
    {
        private readonly int width;
        private readonly int height;

        public DonutDistanceCalculator(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public double Distance(Location loc1, Location loc2)
        {
            return Math.Sqrt(SquaredDistance(loc1, loc2));
        }

        public double SquaredDistance(Location loc1, Location loc2)
        {
            // calculate the closest distance between two locations
            int dRow = Math.Abs(loc1.Row - loc2.Row);
            dRow = Math.Min(dRow, this.height - dRow);

            int dCol = Math.Abs(loc1.Col - loc2.Col);
            dCol = Math.Min(dCol, this.width - dCol);

            return dRow * dRow + dCol * dCol;
        }

        public int ManhattenDistance(Location loc1, Location loc2)
        {
            int dRow = Math.Abs(loc1.Row - loc2.Row);
            dRow = Math.Min(dRow, this.height - dRow);

            int dCol = Math.Abs(loc1.Col - loc2.Col);
            dCol = Math.Min(dCol, this.width - dCol);

            return dCol + dRow;
        }
    }
}
