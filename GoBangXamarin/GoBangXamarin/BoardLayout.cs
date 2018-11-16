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
        Board board = new Board();
        int? step = 0;
        const int COLS = 15;         // 16
        const int ROWS = 15;         // 16
        Tile[,] tiles = new Tile[ROWS, COLS];
        bool isGameEnded;
        bool isGameInProgress;              // on first tap
        public event EventHandler GameStarted;
        public event EventHandler<bool> GameEnded;
        public event EventHandler<Tile> TileTaped;

        public int CurrentStep
        {
            get
            {
                return (int)step;
            }
        }
        public Board CurrentBoard
        {
            get
            {
                return board;
            }
        }
        Tile lastTile;
        public Tile LastTile
        {

            get
            {
                return lastTile;
            }
        }


        public BoardLayout()
        {
            lastTile = new Tile();
            for (int row = 0; row < ROWS; row++)
                for (int col = 0; col < COLS; col++)
                {
                    Tile tile = new Tile(row, col);
                    tile.TileStatusChanged += OnTileStatusChanged;
                    Children.Add(tile.TileView);
                    tiles[row, col] = tile;
                }

            SizeChanged += (sender, args) =>
            {
                double tileWidth = this.Width / COLS;
                double tileHeight = this.Height / ROWS;

                foreach (Tile tile in tiles)
                {
                    Rectangle bounds = new Rectangle(tile.Col * tileWidth,
                                                     tile.Row * tileHeight,
                                                     tileWidth, tileHeight);
                    SetLayoutBounds(tile.TileView, bounds);
                }
            };

            NewGameInitialize();
        }
        public void NewGameInitialize()
        {
            // Clear all the tiles.
            foreach (Tile tile in tiles)
                tile.Initialize();


        }
        void OnTileStatusChanged(object sender, EventArgs args)
        {
            if (isGameEnded)
                return;

            // With a first tile tapped, the game is now in progress.
            if (!isGameInProgress)
            {
                isGameInProgress = true;

                // Fire the GameStarted event.
                GameStarted?.Invoke(this, EventArgs.Empty);
            }

            // Get the tile whose status has changed.
            Tile changedTile = (Tile)sender;
            int x = changedTile.Row;
            int y = changedTile.Col;
            step = changedTile.CurrentStep + 1;
            lastTile = changedTile;
            Application.Current.Properties["CurrentStep"] = step;
            TileTaped?.Invoke(this, changedTile);

            board = board.ChangeBoard(x, y, (int)step);

            Debug.WriteLine($"CurrentStep:{step} X:{x} Y:{y} is Changed");
        }

    }
}
