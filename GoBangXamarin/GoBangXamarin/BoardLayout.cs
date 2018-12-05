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
        public Image ImageGB = new Image();
        //static ImageSource blackImageSource = ImageSource.FromResource("GoBangXamarin.Image.X.png");
        //static ImageSource whiteImageSource = ImageSource.FromResource("GoBangXamarin.Image.O.png");

        #endregion

        #region public
        public ColourEnum Player { set; get; }
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
            ImageGB.Source = ImageSource.FromResource("GoBangXamarin.Image.black.png");
            ImageGB.Opacity = 0;
            Children.Add(ImageGB);

            //imageboard.Source = ImageSource.FromResource("GoBangXamarin.Image.board.jpg");
            //Children.Add(imageboard);           
            IsGameStart = false;
            lastTile = new Tile();
            Player = ColourEnum.Empty;
            AddTileChildren();
            //NewGameInitialize();
        }
        private void AddTileChildren()
        {
            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLS; col++)
                {
                    Thread.Sleep(10);
                    Tile tile = new Tile(row, col);
                    tile.SingleTaped += Tile_SingleTaped; ;
                    Children.Add(tile.TileImage);
                    tiles[col, row] = tile;
                }
            }
            SizeChanged += BoardLayout_SizeChanged;
        }
        private void BoardLayout_SizeChanged(object sender, EventArgs e)
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
        }

        public void NewGameInitialize()
        {
            IsGameStart = false;
            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLS; col++)
                {
                    if (tiles[col, row] == null)
                    {
                        Thread.Sleep(10);
                        Tile tile = new Tile(row, col);
                        tile.SingleTaped += Tile_SingleTaped; ;
                        Children.Add(tile.TileImage);
                        tiles[col, row] = tile;
                    }
                    else
                    {
                        tiles[col, row].EmptyTile();
                    }
                }
            }

            IsGameStart = true;
        }
        public void BoardChange(int x, int y, TileStatus status, bool notice)
        {
            doBoardChange(tiles[x, y], status, false);
        }
        public void BoardChange(int x, int y)
        {
            doBoardChange(tiles[x, y], NextColour);
        }
        private void Tile_SingleTaped(object sender, EventArgs e)
        {
            Tile tile = sender as Tile;
            if (tile.Tilestatus != TileStatus.White && tile.Tilestatus != TileStatus.Black)
            {
                if ((Player == ColourEnum.Black && NextColour == TileStatus.Black)
                 || (Player == ColourEnum.White && NextColour == TileStatus.White)
                 || (Player == ColourEnum.Empty))
                {
                    doBoardChange(tile, NextColour);
                }
                else
                {
                    Debug.WriteLine($"BoardLayout Tile_SingleTaped: Err Player:{ Player} NextColour: {NextColour}");
                }
            }
            else
            {
                Debug.WriteLine($"BoardLayout Tile_SingleTaped: OnSingleTap Err [{tile.X},{tile.Y}] ButtonStatus:{tile.Tilestatus }");
                //tile.ChangeTileStatus(tile.Tilestatus);
            }

        }

        private void doBoardChange(Tile tile, TileStatus status, bool notice = true)
        {
            if (IsGameStart)
            {
                tile.ChangeTileStatus(status);
                if (notice)
                {
                    LastTile = tile;

                    //ImageGB.Opacity = 1;
                    //Rectangle bounds = new Rectangle(LastTile.TileImage.X, LastTile.TileImage.Y, LastTile.TileImage.Width, LastTile.TileImage.Height);
                    //SetLayoutBounds(ImageGB, bounds);

                    TileStatusChanged?.Invoke(this, tile);
                }
            }
        }

    }
}
