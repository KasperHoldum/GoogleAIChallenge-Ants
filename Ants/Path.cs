using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ants
{
    public class Path : List<Location>
    {
        public Path(List<Location> locations)
        {
            if (locations == null)
                throw new ArgumentNullException("locations");
            if (locations.Count <= 0)
                throw new ArgumentException("There must at least the starting position contained in the list of locations", "locations");

            this.AddRange(locations);
        }

        public Path(Location start)
        {
            if (start == null)
                throw new ArgumentNullException("start");

            this.Add(start);
        }

        public Location CurrentLocation
        {
            get { return this[0]; }
        }

        public bool IsFinished
        {
            get { return this.Count <= 1; }
        }

        public Location NextLocation
        {
            get { return this[1]; }
        }

        public Path NextPath()
        {
            if (!IsFinished)
            {
                return new Path(this.Skip(1).ToList());
            }

            throw new IndexOutOfRangeException("There are no more elements in this list");
        }
    }
}
