using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ants.HPA
{
    class Program
    {
        public static void Main(string[] args)
        {
            //var state = Map.Parse("maps/random_walk_06p_01.map");
            //var pathFinding = new HierarchicalPathFindingAStar(state);
            Application.EnableVisualStyles();

            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());

            //var st = Map.Parse("maps/maze_04p_02.map");
            //var hpa = new HierarchicalPathFindingAStar(st, 10);


            //var start = new Location(0, 0);
            //var goal = new Location(39, 34);
            //var roflmao2 = hpa.FindPath(start, goal);
        }
    }
}
