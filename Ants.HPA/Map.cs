using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ants.HPA
{
    public static class Map
    {
        public static GameState Parse(string file)
        {
            var lines = File.ReadAllLines(file);
            var height = int.Parse(lines[0].Split(' ')[1]);
            var width = int.Parse(lines[1].Split(' ')[1]);

            GameState st = new GameState(width, height, 2000, 2000, 93, 5, 1);

            for (int col = 0; col < width; col++)
            {
                for (int row = 0; row < height; row++)
                {
                    var symbol = lines[3 + row][col + 2];
                    if (symbol == '%')
                     st.AddWater(row, col);
                }
            }

            return st;
        }
    }
}
