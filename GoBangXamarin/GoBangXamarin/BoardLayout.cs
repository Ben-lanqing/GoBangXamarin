using GoBangCL.Standard.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        ImageSource gbImageSource = ImageSource.FromResource("GoBangXamarin.Image.gb.png");
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
        public List<Tile> DownTiles = new List<Tile>();
        #endregion

        public BoardLayout()
        {
            ImageGB.Source = gbImageSource;
            //ImageGB.Opacity = 0;
            //Children.Add(ImageGB);

            //imageboard.Source = ImageSource.FromResource("GoBangXamarin.Image.board.jpg");
            //Children.Add(imageboard);           
            IsGameStart = false;
            lastTile = new Tile();
            Player = ColourEnum.Empty;
            //AddTileChildren();
            NewGameInitialize();
            SizeChanged += BoardLayout_SizeChanged;
            IsGameStart = true;
        }
        //private void AddTileChildren()
        //{
        //    for (int row = 0; row < ROWS; row++)
        //    {
        //        for (int col = 0; col < COLS; col++)
        //        {
        //            Thread.Sleep(10);
        //            Tile tile = new Tile(row, col);
        //            tile.SingleTaped += Tile_SingleTaped; ;
        //            Children.Add(tile.EmptyImage);
        //            Children.Add(tile.BlackImage);
        //            Children.Add(tile.WhiteImage);
        //            Children.Add(tile.GbOImage);
        //            Children.Add(tile.GbXImage);
        //            tiles[col, row] = tile;
        //        }
        //    }
        //    SizeChanged += BoardLayout_SizeChanged;
        //}
        private void BoardLayout_SizeChanged(object sender, EventArgs e)
        {
            double min = Math.Min(Width, Height);
            double rate = min / BoardWidth;
            double tileWidth = TileWidth * rate;
            double tileHeight = tileWidth;
            SetLayoutBounds(imageboard, new Rectangle(0, 0, min, min));

            foreach (Tile tile in tiles)
            {
                if (tile == null) return;
                double x = tile.X * tileWidth + (LBorderWidth - TileWidth / 2) * rate;
                double y = tile.Y * tileHeight + (BorderWidth - TileWidth / 2) * rate;

                Rectangle bounds = new Rectangle(x, y, tileWidth, tileHeight);
                SetLayoutBounds(tile.TileImage, bounds);
                //SetLayoutBounds(tile.EmptyImage, bounds);
                //SetLayoutBounds(tile.BlackImage, bounds);
                //SetLayoutBounds(tile.WhiteImage, bounds);
                //SetLayoutBounds(tile.GbOImage, bounds);
                //SetLayoutBounds(tile.GbXImage, bounds);
            }
            foreach (Tile tile in DownTiles)
            {
                if (tile == null) return;
                double x = tile.X * tileWidth + (LBorderWidth - TileWidth / 2) * rate;
                double y = tile.Y * tileHeight + (BorderWidth - TileWidth / 2) * rate;

                Rectangle bounds = new Rectangle(x, y, tileWidth, tileHeight);
                SetLayoutBounds(tile.TileImage, bounds);
                //SetLayoutBounds(tile.EmptyImage, bounds);
                //SetLayoutBounds(tile.BlackImage, bounds);
                //SetLayoutBounds(tile.WhiteImage, bounds);
                //SetLayoutBounds(tile.GbOImage, bounds);
                //SetLayoutBounds(tile.GbXImage, bounds);
            }

        }
        //private void setBounds()
        //{
        //    double min = Math.Min(Width, Height);
        //    double rate = min / BoardWidth;
        //    double tileWidth = TileWidth * rate;
        //    double tileHeight = tileWidth;
        //    SetLayoutBounds(imageboard, new Rectangle(0, 0, min, min));

        //    foreach (Tile tile in tiles)
        //    {
        //        if (tile == null) return;
        //        double x = tile.X * tileWidth + (LBorderWidth - TileWidth / 2) * rate;
        //        double y = tile.Y * tileHeight + (BorderWidth - TileWidth / 2) * rate;

        //        Rectangle bounds = new Rectangle(x, y, tileWidth, tileHeight);
        //        SetLayoutBounds(tile.TileImage, bounds);
        //        //SetLayoutBounds(tile.EmptyImage, bounds);
        //        //SetLayoutBounds(tile.BlackImage, bounds);
        //        //SetLayoutBounds(tile.WhiteImage, bounds);
        //        //SetLayoutBounds(tile.GbOImage, bounds);
        //        //SetLayoutBounds(tile.GbXImage, bounds);
        //    }
        //}
        public void NewGameInitialize()
        {
            //IsGameStart = false;
            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLS; col++)
                {
                    if (tiles[col, row] == null)
                    {
                        Thread.Sleep(1);
                        Tile tile = new Tile(row, col);
                        tile.SingleTaped += Tile_SingleTaped;
                        Children.Add(tile.TileImage);

                        //Children.Add(tile.EmptyImage);
                        //Children.Add(tile.BlackImage);
                        //Children.Add(tile.WhiteImage);
                        //Children.Add(tile.GbOImage);
                        //Children.Add(tile.GbXImage);
                        tiles[col, row] = tile;
                    }
                    else
                    {
                        //tiles[col, row].EmptyTile();
                    }
                }
            }
            //if (tiles[1, 1].EmptyImage.Width <= 0)
            //{
            //    setBounds();
            //}
            //IsGameStart = true;
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

        public async Task SetGB(int x, int y)
        {
            var gbs = DownTiles.Where(a => a.Tilestatus == TileStatus.BlackGB || a.Tilestatus == TileStatus.WhiteGB);
            foreach (var gb in gbs)
            {
                gb.SetTileImage(gb.Tilestatus == TileStatus.BlackGB ? TileStatus.Black : TileStatus.White);
            }
            Tile last = DownTiles.FirstOrDefault(a => a.X == x && a.Y == y);
            if (last != null)
            {
                Thread.Sleep(50);
                last.SetTileImage(last.Tilestatus == TileStatus.Black ? TileStatus.BlackGB : TileStatus.WhiteGB);
            }
        }

        private void doBoardChange(Tile tile, TileStatus status, bool notice = true)
        {
            if (IsGameStart)
            {
                Tile tileDown = new Tile(tile.X, tile.Y, status);
                Children.Add(tileDown.TileImage);
                DownTiles.Add(tileDown);
                Rectangle bounds = new Rectangle(tile.TileImage.X, tile.TileImage.Y, tile.TileImage.Width, tile.TileImage.Height);
                SetLayoutBounds(tileDown.TileImage, bounds);

                //tile.ChangeTileStatus(status);
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
