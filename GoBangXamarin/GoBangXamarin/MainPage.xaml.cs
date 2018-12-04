using GoBangCL.Standard.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using GoBangCL.Standard;
using System.Collections.ObjectModel;
using System.Reflection;
using System.IO;
using System.Threading;

namespace GoBangXamarin
{
    public partial class MainPage : ContentPage
    {
        #region 
        public List<Board> BoardList;
        public GoBangHepler hepler;
        public Board CurrentBoard;
        //public ObservableCollection<Piece> downPList;
        //SKCanvas sKCanvas;
        //SKBitmap bitmap;
        //Dictionary<Guid, Point> btDic;
        bool isGameInProgress;
        bool isGameStart;
        DateTime gameStartTime;

        #endregion
        public MainPage()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            Debug.WriteLine($"MainPage Start Init");
            CurrentBoard = new Board();
            BoardList = new List<Board>();
            isGameInProgress = false;
            isGameStart = false;
            boardLayout.Player = ColourEnum.Black;
            boardLayout.TileStatusChanged += BoardLayout_TileStatusChanged;
            // PrepareForNewGame();
        }
        private void PrepareForNewGame()
        {
            try
            {
                Debug.WriteLine($"MainPage Start PrepareForNewGame~~~~~~~~~~~~~~");
                CurrentBoard = new Board();
                BoardList.Clear();
                isGameInProgress = false;
                isGameStart = false;
                boardLayout.NewGameInitialize();

                timeLabel.Text = new TimeSpan(0).ToString(ConstClass.TimeFormat);
                msgLb.Text = "";
                msgLb.TextColor = Color.Black;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(StaticClass.LogException("PrepareForNewGame", ex));
            }
        }

        private void NewGame()
        {
            string[] games = ConstClass.GameStr.Split(';');
            int count = games.Length;
            Random rd = new Random();
            int i = rd.Next(0, count);

            string[] points = games[i].Split(':');
            int length = points.Length;
            for (int j = 0; j < length; j++)
            {
                Thread.Sleep(50);
                string[] point = points[j].Split(',');
                //Debug.WriteLine($"MainPage NewGame [{point[0]},{point[1]}] step:{CurrentBoard.Step}");

                boardLayout.BoardChange(int.Parse(point[0]), int.Parse(point[1]));
            }
        }
        #region PageEvent

        void BoardLayout_TileStatusChanged(object sender, Tile e)
        {
            //if (!isGameStart)
            //{
            //    BoardLayout_GameStarted();
            //}
            var boardLayout = sender as BoardLayout;
            BoardTileChanged(boardLayout, e);
        }

        void BoardTileChanged(BoardLayout boardLayout, Tile e)
        {
            try
            {
                if (boardLayout == null) return;
                var board = CurrentBoard.ChangeBoard(e.X, e.Y, CurrentBoard.Step + 1);
                if (!BoardList.Exists(a => a.Step == board.Step))
                {
                    Application.Current.Properties["CurrentStep"] = board.Step;
                    CurrentBoard = board;
                    BoardList.Add(CurrentBoard);
                    ChangeLastImage();
                    //判断是否胜
                    if (CurrentBoard.Step > 5 && CurrentBoard.IsWin)
                    {
                        boardLayout.IsGameStart = isGameStart = false;
                        msgLb.Text = $"[{CurrentBoard.Colour}] IsWin! Start A New Game.";
                        msgLb.TextColor = Color.Red;
                    }
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DoAI();
                    });
                }
                else
                {
                    Debug.WriteLine($"MainPage: BoardTileChanged Err [{e.X},{e.Y}] TileStep:{CurrentBoard.Step}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(StaticClass.LogException("BoardTileChanged", ex));
            }
        }
        private void ChangeLastImage()
        {
            int count = CurrentBoard.DownPieces.Count();
            if (count > 2)
            {
                var lastP1 = CurrentBoard.DownPieces[count - 1];
                boardLayout.BoardChange(lastP1.X, lastP1.Y, lastP1.Colour == ColourEnum.Black ? TileStatus.BlackGB : TileStatus.WhiteGB, false);
                var lastP2 = CurrentBoard.DownPieces[count - 2];
                boardLayout.BoardChange(lastP2.X, lastP2.Y, lastP2.Colour == ColourEnum.Black ? TileStatus.Black : TileStatus.White, false);

            }
        }

        private void NewGameButton_Clicked(object sender, EventArgs e)
        {
            BoardLayout_GameStarted();

            PrepareForNewGame();
            //Thread.Sleep(1000);
            NewGame();
        }
        private void NextButton_Clicked(object sender, EventArgs e)
        {
            GetNextStep(CurrentBoard);
        }
        void OnMainContentViewSizeChanged(object sender, EventArgs args)
        {
            ContentView contentView = (ContentView)sender;
            double width = contentView.Width;
            double height = contentView.Height;

            bool isLandscape = width > height;

            if (isLandscape)
            {
                mainGrid.RowDefinitions[0].Height = 0;
                mainGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);

                mainGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                mainGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);

                Grid.SetRow(textStack, 1);
                Grid.SetColumn(textStack, 0);
            }
            else // portrait
            {
                mainGrid.RowDefinitions[0].Height = new GridLength(3, GridUnitType.Star);
                mainGrid.RowDefinitions[1].Height = new GridLength(5, GridUnitType.Star);

                mainGrid.ColumnDefinitions[0].Width = 0;
                mainGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);

                Grid.SetRow(textStack, 0);
                Grid.SetColumn(textStack, 1);
            }
        }
        void OnBoardContentViewSizeChanged(object sender, EventArgs args)
        {
            ContentView contentView = (ContentView)sender;
            double width = contentView.Width;
            double height = contentView.Height;
            double dimension = Math.Min(width, height);
            double horzPadding = (width - dimension) / 2;
            double vertPadding = (height - dimension) / 2;
            contentView.Padding = new Thickness(horzPadding, vertPadding);
        }
        bool UpdateTimerLabel()
        {
            if (isGameStart)
            {
                timeLabel.Text = (DateTime.Now - gameStartTime).ToString(ConstClass.TimeFormat);
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
        private void BoardLayout_GameStarted()
        {
            Debug.WriteLine($"MainPage: GameStart ");

            isGameStart = true;
            gameStartTime = DateTime.Now;
            Device.StartTimer(TimeSpan.FromSeconds(1), UpdateTimerLabel);

        }
        private void DoAI()
        {
            //AI取点
            int CurrentStep = CurrentBoard.Step;
            if (CurrentStep >= 3 && CurrentBoard.Colour == boardLayout.Player)
            {
                var point = GetNextStep(CurrentBoard);
                //Debug.WriteLine($"MainPage: DoAI [{point.X},{point.Y}] TileStep:{CurrentStep }");
                boardLayout.BoardChange((int)point.X, (int)point.Y);
            }
        }
        private Point GetNextStep(Board board)
        {
            Point point = new Point();
            try
            {
                hepler = new GoBangHepler(board);
                PieceInfo pieceInfo = hepler.AIGetNext(board);
                string str = $"NextPiece:  x:{pieceInfo.X + 1} y:{pieceInfo.Y + 1} ";

                msgLb.Text = str;
                point.X = pieceInfo.X;
                point.Y = pieceInfo.Y;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(StaticClass.LogException("GetNextStep", ex));

            }
            return point;
        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {
            BackOneStep();
            if (CurrentBoard.Colour == boardLayout.Player)
            {
                BackOneStep();
            }
        }

        private void BackOneStep()
        {
            if (BoardList.Count < 5) return;
            var board = BoardList.LastOrDefault();
            BoardList.Remove(board);
            var lastP = board.DownPieces.Last();
            boardLayout.BoardChange(lastP.X, lastP.Y, TileStatus.Empty, false);
            //boardLayout.tiles[lastP.X, lastP.Y].ChangeTileStatus(TileStatus.Empty);
            var newboard = BoardList.LastOrDefault();
            CurrentBoard = newboard;
            ChangeLastImage();
        }
    }

}

