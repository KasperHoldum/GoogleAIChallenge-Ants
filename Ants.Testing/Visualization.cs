using System.Collections.Generic;
using System.Drawing;

namespace Ants
{
    public class Visualization
    {
        public static void DrawList(List<Location> locations, Color color, GameState state, string fileName )
        {
            var bitmap = new Bitmap(state.Width, state.Height);
            locations.ForEach(l => bitmap.SetPixel(l.Col, l.Row, color));
            bitmap.Save(fileName);
        }
    }
}
