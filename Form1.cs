using Dungeon_Generator;
using Dungeon_Generator.Generators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DungeonVisualizer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private static Color[] Palette = new Color[] { Color.Black, Color.LightGray, Color.White };
        private void button1_Click(object sender, EventArgs e)
        {
            DungeonFloor<int> Dungeon = new DungeonFloor<int>(256);
            CityRoadsGenerator<int> Generator = new CityRoadsGenerator<int>(Dungeon);
            Generator.DefaultBlockedTile = 0;
            Generator.DefaultOpenTile = 1;
            Generator.DefaultFeatureTile = 2;

            do
            {
                Generator.Generate();
            } while (Generator.Roads < 40 || Dungeon.EdgePierced(Generator.DefaultBlockedTile));


            label1.Text = String.Format("{0} roads", Generator.Roads);

            Bitmap T = new Bitmap(Dungeon.Size.Width, Dungeon.Size.Height, PixelFormat.Format32bppArgb);

            for (int X = 0; X < Dungeon.Size.Width; X++)
            {
                for (int Y = 0; Y < Dungeon.Size.Height; Y++)
                {
                    T.SetPixel(X, Y, Palette[Dungeon.GetTile(X, Y)]);
                }
            }

            pictureBox1.Image = T;
        }
    }
}
