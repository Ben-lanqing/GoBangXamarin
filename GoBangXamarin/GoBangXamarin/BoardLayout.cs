using GoBangCL.Standard.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace GoBangXamarin
{
    public class BoardLayout : AbsoluteLayout
    {
        int step = 0;
        const int COLS = 15;         // 16
        const int ROWS = 15;         // 16


        const int BoardWidth = 500;
        const int TileWidth = 30;
        const int BorderWidth = 30;
        const int LBorderWidth = 50;

        Tile[,] tiles = new Tile[ROWS, COLS];
        bool isGameInProgress;              // on first tap
        public event EventHandler GameStarted;
        public event EventHandler<bool> GameEnded;
        public event EventHandler<Tile> TileTaped;

        public int CurrentStep
        {
            set
            {
                if (step != value)
                {
                    step = value;
                    OnPropertyChanged();
                }
            }
            get
            {
                return (int)step;
            }
        }
        public Board CurrentBoard { get; private set; } = new Board();
        Tile lastTile;
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


        Image image = new Image();
        public BoardLayout()
        {

            image.Source = ImageSource.FromResource("GoBangXamarin.Image.board.jpg");
            Children.Add(image);

            lastTile = new Tile();
            for (int row = 0; row < ROWS; row++)
                for (int col = 0; col < COLS; col++)
                {
                    Tile tile = new Tile(row, col);
                    tile.TileStatusChanged += OnTileStatusChanged;
                    Children.Add(tile.TileView);
                    //Children.Add(tile.TileImage);
                    tiles[row, col] = tile;
                }

            SizeChanged += (sender, args) =>
            {
                double min = Math.Min(Width, Height);
                double rate = min / BoardWidth;
                //double tileWidth = this.Width / COLS;
                //double tileHeight = this.Height / ROWS;

                double tileWidth = TileWidth * rate;
                double tileHeight = tileWidth;
                SetLayoutBounds(image, new Rectangle(0, 0, min, min));

                foreach (Tile tile in tiles)
                {
                    double x = tile.X * tileWidth + (LBorderWidth - TileWidth / 2) * rate;
                    double y = tile.Y * tileHeight + (BorderWidth - TileWidth / 2) * rate;

                    Rectangle bounds = new Rectangle(x, y, tileWidth, tileHeight);
                    SetLayoutBounds(tile.TileView, bounds);
                    //SetLayoutBounds(tile.TileImage, bounds);
                }
            };

            NewGameInitialize();
        }
        public void NewGameInitialize()
        {
            // Clear all the tiles.
            foreach (Tile tile in tiles)
                tile.Initialize();
            isGameInProgress = false;
            CurrentStep = 0;
        }
        void OnTileStatusChanged(object sender, EventArgs args)
        {
            // With a first tile tapped, the game is now in progress.
            if (!isGameInProgress)
            {
                isGameInProgress = true;
                // Fire the GameStarted event.
                GameStarted?.Invoke(this, EventArgs.Empty);
            }

            // Get the tile whose status has changed.
            Tile changedTile = (Tile)sender;
            LastTile = changedTile;
            CurrentStep = (int)Application.Current.Properties["CurrentStep"] + 1;
            Application.Current.Properties["CurrentStep"] = CurrentStep;
            CurrentBoard = CurrentBoard.ChangeBoard(changedTile.X, changedTile.Y, CurrentStep);
            TileTaped?.Invoke(this, changedTile);
        }

    }
}
