using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ants.DataStructures.HPA;

namespace Ants.HPA
{
    public partial class Main : Form
    {
        private GameState state;
        private ClusterCollection cc;

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            pictureBox1.Paint += pictureBox1_Paint;
        }

        void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (cc != null)
            {
                var drawFactor = 8;
                e.Graphics.FillRectangle(Brushes.White, 0, 0, pictureBox1.Width, pictureBox1.Height);

                for (int i = 0; i < state.Width; i++)
                {
                    for (int j = 0; j < state.Height; j++)
                    {
                        e.Graphics.FillRectangle(state[i,j] == Tile.Water ? Brushes.DarkBlue : Brushes.LimeGreen, i * drawFactor, j*drawFactor, drawFactor, drawFactor);
                    }
                }

                for (int i = 0; i < cc.ClusterCount; i++)
                {
                    for (int j = 0; j < cc.ClusterCount; j++)
                    {
                        var cluster = cc[i, j];

                        e.Graphics.DrawRectangle(Pens.Black, cluster.Position.Col * drawFactor, cluster.Position.Row * drawFactor, cluster.ClusterSize.Col * drawFactor, cluster.ClusterSize.Row * drawFactor);

                        foreach (var transit in cluster.TransistPoints)
                        {
                            e.Graphics.FillRectangle(Brushes.Red, transit.Col * drawFactor, transit.Row * drawFactor, drawFactor, drawFactor);

                            foreach (TransitNode connection in transit.Edges)
                            {
                                if ((connection -transit).Length() < 50)
                                    e.Graphics.DrawLine(Pens.DodgerBlue, transit.Col * drawFactor + drawFactor / 2, transit.Row * drawFactor + drawFactor / 2, connection.Col * drawFactor + drawFactor / 2, connection.Row * drawFactor + drawFactor / 2);
                            }

                        }
                    }
                }

                if (path != null)
                {
                    for (int i = 1; i < path.Count; i++)
                    {
                        e.Graphics.DrawLine(Pens.Aqua, path[i - 1].Col * drawFactor + drawFactor / 2, path[i - 1].Row * drawFactor + drawFactor / 2,
                                            path[i].Col * drawFactor + drawFactor / 2, path[i].Row * drawFactor + drawFactor / 2);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            state = new GameState(72, 72, 200, 200, 200, 020, 200);

            x1NUD.Minimum = 0;
            y1NUD.Minimum = 0;
            y2NUD.Minimum = 0;
            x2NUD.Minimum = 0;


            x1NUD.Maximum = 72;
            x2NUD.Maximum = 72;
            y1NUD.Maximum = 72;
            y2NUD.Maximum = 72;

            AStarPathFinding pathFinding = new AStarPathFinding(state);
            state.AddWater(6, 3);
            state.AddWater(3, 0);
            state.AddWater(1, 0);
            state.AddWater(0, 1);
            state.AddWater(0, 0);


            for (int i = 34; i < 60; i++)
            {
                for (int j = 20; j < 25; j++)
                {
                    state.AddWater(i,j);
                }
            }

            cc = new ClusterCollection(state, 10);

            cc.Initialize(pathFinding);

            pictureBox1.Invalidate();
        }

        private List<Location> path; 
        private void button2_Click(object sender, EventArgs e)
        {
            var lol = new HierarchicalPathFindingAStar(state, 10);
            var goal = new Location((int)x1NUD.Value, (int)y1NUD.Value);
            var start = new Location((int)x2NUD.Value, (int)y2NUD.Value);
            path = lol.FindPath(start, goal);

            int counter = 0;
            while (!path.Contains(goal))
            {
                counter++;

                if( counter > 100)
                {
                    break;
                }
                path = path.Concat(lol.FindPath(path.Last(), goal)).ToList();
            }
            pictureBox1.Invalidate();
        }
    }
}
