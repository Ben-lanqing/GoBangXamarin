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
        #region const\private

        const int COLS = 15;         // 16
        const int ROWS = 15;         // 16
        const int BoardWidth = 500;
        const int TileWidth = 30;
        const int BorderWidth = 30;
        const int LBorderWidth = 50;
        Tile lastTile;
        Image imageboard = new Image();
        static ImageSource blackImageSource = ImageSource.FromResource("GoBangXamarin.Image.X.png");
        static ImageSource whiteImageSource = ImageSource.FromResource("GoBangXamarin.Image.O.png");

        #endregion

        #region public
        public int Player { set; get; }
        public bool IsGameStart;
        public Tile[,] tiles = new Tile[COLS, ROWS];
        public event EventHandler<Tile> TileStatusChanged;
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
        public TileStatus NextColour
        {
            get
            {
                int nextStep = (int)Application.Current.Properties["CurrentStep"] + 1;

                if (nextStep % 2 == 1)
                    return TileStatus.Black;
                else
                    return TileStatus.White;
            }
        }
        #endregion

        public BoardLayout()
        {
            IsGameStart = false;
            lastTile = new Tile();
            Player = -1;
            imageboard.Source = ImageSource.FromResource("GoBangXamarin.Image.board.jpg");
            //Children.Add(image);
            for (int row = 0; row < ROWS; row++)
                for (int col = 0; col < COLS; col++)
                {
                    Tile tile = new Tile(row, col);
                    tile.SingleTaped += Tile_SingleTaped; ;
                    Children.Add(tile.TileImage);
                    tiles[col, row] = tile;
                }
            SizeChanged += (sender, args) =>
            {
                double min = Math.Min(Width, Height);
                double rate = min / BoardWidth;
                double tileWidth = TileWidth * rate;
                double tileHeight = tileWidth;
                SetLayoutBounds(imageboard, new Rectangle(0, 0, min, min));

                foreach (Tile tile in tiles)
                {
                    double x = tile.X * tileWidth + (LBorderWidth - TileWidth / 2) * rate;
                    double y = tile.Y * tileHeight + (BorderWidth - TileWidth / 2) * rate;

                    Rectangle bounds = new Rectangle(x, y, tileWidth, tileHeight);
                    //SetLayoutBounds(tile.TileView, bounds);
                    SetLayoutBounds(tile.TileImage, bounds);
                }
            };
            //NewGameInitialize();
        }


        public void NewGameInitialize()
        {
            IsGameStart = false;
            foreach (Tile tile in tiles)
            {
                Thread.Sleep(1);
                tile.Initialize();
            }
            IsGameStart = true;
        }
        public void BoardChange(int x, int y)
        {
            doBoardChange(tiles[x, y]);
        }

        private void Tile_SingleTaped(object sender, EventArgs e)
        {
            Tile tile = sender as Tile;
            if (tile.Tilestatus != TileStatus.White && tile.Tilestatus != TileStatus.Black)
            {
                if ((Player == 1 && NextColour == TileStatus.Black)
                 || (Player == 0 && NextColour == TileStatus.White)
                 || (Player == -1))
                {
                    doBoardChange(tile);
                }
            }
            else
            {
                Debug.WriteLine($"Tile: OnSingleTap Err [{X},{Y}] ButtonStatus:{tile.Tilestatus }");
            }

        }

        private void doBoardChange(Tile tile)
        {
            if (IsGameStart)
            {
                tile.ChangeTileStatus(NextColour);
                LastTile = tile;
                TileStatusChanged?.Invoke(this, tile);
                //int count = 0;
                //foreach (var t in tiles)
                //{
                //    CheckTileSource(t);
                //    count++;

                //}
                //Debug.WriteLine(count);
            }
        }
        private void CheckTileSource(Tile tile)
        {
            if (tile.Tilestatus == TileStatus.Black)
            {
                if (tile.TileImage.Source.Id != blackImageSource.Id)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        tile.TileImage.SetValue(Image.SourceProperty, blackImageSource);
                    });
                    Debug.WriteLine($"MainPage: checkTileSource [{tile.X},{tile.Y}] Tilestatus:{tile.Tilestatus }");
                }
            }
            if (tile.Tilestatus == TileStatus.White)
            {
                if (tile.TileImage.Source.Id != whiteImageSource.Id)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        tile.TileImage.SetValue(Image.SourceProperty, whiteImageSource);
                    });

                    Debug.WriteLine($"MainPage: checkTileSource [{tile.X},{tile.Y}] Tilestatus:{tile.Tilestatus }");

                }
            }
        }

    }
}
