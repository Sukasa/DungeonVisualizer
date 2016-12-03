using DungeonGenerator;
using DungeonGenerator.Generators;
using DungeonGenerator.Transformers;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace DungeonVisualizer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private enum CityTile : int
        {
            Grass,
            Road,
            Building,
            Wall,
            TreeTrunk,
            Foliage,
            Water
        }

        private static Color[] Palette = new Color[] { Color.Black, Color.LightGray, Color.White };
        private void button1_Click(object sender, EventArgs e)
        {
            DungeonFloor<int> Dungeon = new DungeonFloor<int>((int)DungeonSize.Value);
            CityRoadsGenerator<int> Generator = new CityRoadsGenerator<int>(Dungeon);
            Generator.DefaultBlockedTile = 0;
            Generator.DefaultOpenTile = 1;
            Generator.DefaultFeatureTile = 2;
            int Attempts = 0;
            do
            {
                Generator.Generate();
                Attempts++;
            } while (Dungeon.EdgePierced(Generator.DefaultBlockedTile));

            DungeonFloor<Color> ColourMap = new DungeonFloor<Color>(Dungeon.Size);
            DungeonFloor<CityTile> TileMap = new DungeonFloor<CityTile>(Dungeon.Size);

            LambdaTransformer<int, CityTile> TileTransformer = new LambdaTransformer<int, CityTile>(Dungeon, TileMap);
            LambdaTransformer<CityTile, CityTile> FoliageTransformer = new LambdaTransformer<CityTile, CityTile>(TileMap, TileMap);
            LambdaTransformer<CityTile, Color> MapTransformer = new LambdaTransformer<CityTile, Color>(TileMap, ColourMap);

            //--------------
            // Do the tile transforms

            // Roads
            TileTransformer.Lambdas.Add((D, Point, Random) =>
            {
                if (D.GetTile(Point) == 1)
                    return TileTransformer.CreateTransform(CityTile.Road, 2);

                return null;
            });

            // Building
            TileTransformer.Lambdas.Add((D, Point, Random) =>
            {
                if (D.GetTile(Point) == 2)
                    return TileTransformer.CreateTransform(CityTile.Building, 3);

                return null;
            });

            // Grass
            TileTransformer.Lambdas.Add((D, Point, Random) =>
            {
                if (D.GetTile(Point) == 0)
                    return TileTransformer.CreateTransform(CityTile.Grass, 1);

                return null;
            });

            // Wall
            TileTransformer.Lambdas.Add((D, Point, Random) =>
            {
                if (D.GetTile(Point) == 0 && D.Adjacent4(Point, 2))
                    return TileTransformer.CreateTransform(CityTile.Wall, 3);

                return null;
            });

            // Tree Trunk
            TileTransformer.Lambdas.Add((D, Point, Random) =>
            {
                if (Random.NextByte() < 5 && D.NumWithin(Point, 6, 1, 2) == 0)
                    return TileTransformer.CreateTransform(CityTile.TreeTrunk, 2);

                return null;
            });


            // ----------------
            // Foliage Transforms
            FoliageTransformer.Lambdas.Add((D, Point, Random) =>
            {
                if (D.GetTile(Point) != CityTile.TreeTrunk && D.NumWithin(Point, 2, CityTile.TreeTrunk) > 0)
                    return FoliageTransformer.CreateTransform(CityTile.Foliage, 2);

                return null;
            });

            FoliageTransformer.Lambdas.Add((D, Point, Random) =>
            {
                return FoliageTransformer.CreateTransform(D.GetTile(Point), 1);
            });


            //--------------
            // Set up the lambdas that draw the thing

            // Roads
            MapTransformer.Lambdas.Add((D, Point, Random) =>
            {
                if (D.GetTile(Point) == CityTile.Road)
                    return MapTransformer.CreateTransform(Color.SandyBrown, 3);

                return null;
            });

            // Grass
            MapTransformer.Lambdas.Add((D, Point, Random) =>
            {
                if (D.GetTile(Point) == CityTile.Grass)
                    return MapTransformer.CreateTransform(Color.ForestGreen, 1);

                return null;
            });

            // Foliage
            MapTransformer.Lambdas.Add((D, Point, Random) =>
            {
                if (D.GetTile(Point) == CityTile.Foliage)
                    return MapTransformer.CreateTransform(Color.DarkGreen, 1);

                return null;
            });

            // Tree Trunk
            MapTransformer.Lambdas.Add((D, Point, Random) =>
            {
                if (D.GetTile(Point) == CityTile.TreeTrunk)
                    return MapTransformer.CreateTransform(Color.OrangeRed, 1);

                return null;
            });

            // Building Walls
            MapTransformer.Lambdas.Add((D, Point, Random) =>
            {
                if (D.GetTile(Point) == CityTile.Wall)
                    return MapTransformer.CreateTransform(Color.Black, 3);

                return null;
            });

            // Building Floor
            MapTransformer.Lambdas.Add((D, Point, Random) =>
            {
                if (D.GetTile(Point) == CityTile.Building)
                    return MapTransformer.CreateTransform(Color.SlateGray, 2);

                return null;
            });

            //-------------

            TileTransformer.Transform();
            FoliageTransformer.Transform();
            MapTransformer.Transform();

            label1.Text = String.Format("{0} roads, {1} rooms, {2} attempts", Generator.Roads, Generator.Rooms, Attempts);
            Bitmap T = new Bitmap(Dungeon.Size.Width, Dungeon.Size.Height, PixelFormat.Format32bppArgb);

            for (int X = 0; X < Dungeon.Size.Width; X++)
            {
                for (int Y = 0; Y < Dungeon.Size.Height; Y++)
                {
                    T.SetPixel(X, Y, ColourMap.GetTile(X, Y));
                }
            }

            pictureBox1.Image = T;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
