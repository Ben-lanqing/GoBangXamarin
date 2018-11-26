using GoBangCL.Standard.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GoBangXamarin
{
    public class BoardLayout : AbsoluteLayout
    {
        #region 
        const int COLS = 15;         // 16
        const int ROWS = 15;         // 16
        const int BoardWidth = 500;
        const int TileWidth = 30;
        const int BorderWidth = 30;
        const int LBorderWidth = 50;
        Tile lastTile;
        Image image = new Image();

        #endregion
        public Tile[,] tiles = new Tile[ROWS, COLS];

        public event EventHandler<Tile> TileTaped;
        public bool IsGameStart;

        public Tile LastTile
        {
            set
            {
                if (lastTile != value)
                {
                    lastTile = value;
                    OnPropertyChanged();
                }
            }
            get
            {
                return lastTile;
            }
        }


        public BoardLayout()
        {
            IsGameStart = false;
            image.Source = ImageSource.FromResource("GoBangXamarin.Image.board.jpg");
            //Children.Add(image);

            lastTile = new Tile();
            for (int row = 0; row < ROWS; row++)
                for (int col = 0; col < COLS; col++)
                {
                    Tile tile = new Tile(row, col);
                    //tile.TileStatusChanged += OnTileStatusChanged;
                    tile.SingleTaped += Tile_SingleTaped; ;
                    //Children.Add(tile.TileView);
                    Children.Add(tile.TileImage);
                    tiles[row, col] = tile;
                }

            SizeChanged += (sender, args) =>
            {
                double min = Math.Min(Width, Height);
                double rate = min / BoardWidth;
                double tileWidth = TileWidth * rate;
                double tileHeight = tileWidth;
                SetLayoutBounds(image, new Rectangle(0, 0, min, min));

                foreach (Tile tile in tiles)
                {
                    double x = tile.X * tileWidth + (LBorderWidth - TileWidth / 2) * rate;
                    double y = tile.Y * tileHeight + (BorderWidth - TileWidth / 2) * rate;

                    Rectangle bounds = new Rectangle(x, y, tileWidth, tileHeight);
                    //SetLayoutBounds(tile.TileView, bounds);
                    SetLayoutBounds(tile.TileImage, bounds);
                }
            };

            NewGameInitialize();
        }
        public void NewGameInitialize()
        {
            // Clear all the tiles.
            foreach (Tile tile in tiles)
            {
                Thread.Sleep(1);
                tile.Initialize();
            }
            IsGameStart = true;
        }
        public void DownPiece(int x, int y, int step)
        {
            doBoardChange(tiles[y, x], step);
        }

        private void Tile_SingleTaped(object sender, EventArgs e)
        {
            Tile tile = sender as Tile;
            int nextStep = (int)Application.Current.Properties["CurrentStep"] + 1;
            doBoardChange(tile, nextStep);
        }
        private void doBoardChange(Tile tile, int step)
        {
            if (IsGameStart)
            {
                if (step % 2 == 1)
                    tile.Tilestatus = TileStatus.Black;
                else
                    tile.Tilestatus = TileStatus.White;
            }
            LastTile = tile;
            TileTaped?.Invoke(this, tile);
            checkTileSource(tile);
        }
        static ImageSource blackImageSource = ImageSource.FromResource("GoBangXamarin.Image.X.png");
        static ImageSource whiteImageSource = ImageSource.FromResource("GoBangXamarin.Image.O.png");

        private void checkTileSource(Tile tile)
        {
            if (tile.Tilestatus == TileStatus.Black)
            {
                if (tile.TileImage.Source.Id != blackImageSource.Id)
                {
                    tile.TileImage.SetValue(Image.SourceProperty, blackImageSource);
                    Debug.WriteLine($"MainPage: checkTileSource [{tile.X},{tile.Y}] Tilestatus:{tile.Tilestatus }");
                }
            }
            if (tile.Tilestatus == TileStatus.White)
            {
                if (tile.TileImage.Source.Id != whiteImageSource.Id)
                {
                    tile.TileImage.SetValue(Image.SourceProperty, whiteImageSource);
                    Debug.WriteLine($"MainPage: checkTileSource [{tile.X},{tile.Y}] Tilestatus:{tile.Tilestatus }");

                }
            }
        }

    }
}
