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
using SkiaSharp;
using System.Reflection;
using System.IO;

namespace GoBangXamarin
{
    public partial class MainPage : ContentPage
    {
        #region 
        public List<Board> BoardList;
        public GoBangHepler hepler;
        public Board CurrentBoard;
        public ObservableCollection<Piece> downPList;
        //SKCanvas sKCanvas;
        //SKBitmap bitmap;
        Dictionary<Guid, Point> btDic;
        bool isGameInProgress;
        bool isGameStart;
        DateTime gameStartTime;
        const string timeFormat = @"%m\:ss";
        string gameStr = "7,7:7,6:7,9"
                      + ";7,7:7,6:8,6"
                      + ";7,7:7,6:7,5"
                      + ";7,7:8,6:9,7"
                      + ";7,7:8,6:9,5"
                      + ";7,7:7,6:9,6"
                      + ";7,7:7,6:8,7"
                      + ";7,7:7,6:9,7"
                      + ";7,7:8,6:7,8"
                      + ";7,7:7,6:9,8"
                      + ";7,7:8,6:6,8"
                      + ";7,7:8,6:9,6"
                      + ";7,7:8,6:8,5"
                      + ";7,7:7,6:7,8"
                      + ";7,7:8,6:9,8"
                      + ";7,7:7,6:9,5"
                      + ";7,7:7,6:8,9"
                      + ";7,7:7,6:8,8"
                      + ";7,7:8,6:8,8"
                      + ";7,7:8,6:7,9"
                      + ";7,7:8,6:6,9"
                      + ";7,7:8,6:9,9"
                      + ";7,7:8,6:8,9"
                      + ";7,7:8,6:8,7"
                      + ";7,7:8,6:5,9"
                      + ";7,7:7,6:9,9";

        #endregion
        public MainPage()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            Debug.WriteLine($"MainPage Start Init");
            BoardList = new List<Board>();
            CurrentBoard = new Board();
            downPList = new ObservableCollection<Piece>();
            btDic = new Dictionary<Guid, Point>();
            boardLayout.TileTaped += BoardLayout_TileTaped;
            PrepareForNewGame();
            //boardLayout.GameStarted += BoardLayout_GameStarted;
            //boardLayout.GameStarted += (sender, args) =>
            //{
            //    //isGameInProgress = true;

            //    gameStartTime = DateTime.Now;
            //    Device.StartTimer(TimeSpan.FromSeconds(1), UpdateTimerLabel);

            //    //Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            //    //{
            //    //    timeLabel.Text = (DateTime.Now - gameStartTime).ToString(timeFormat);
            //    //    return isGameInProgress;
            //    //});
            //};

        }
        private void PrepareForNewGame()
        {
            Debug.WriteLine($"MainPage Start PrepareForNewGame");

            isGameInProgress = false;
            isGameStart = false;
            boardLayout.NewGameInitialize();
            timeLabel.Text = new TimeSpan(0).ToString(timeFormat);
            msgLb.Text = "";
            Debug.WriteLine($"MainPage Start PrepareForNewGame");

            BoardLayout_GameStarted();

            string[] games = gameStr.Split(';');
            int count = games.Length;
            Random rd = new Random();
            int i = rd.Next(0, count);

            string[] points = games[i].Split(':');
            int length = points.Length;
            for (int j = 0; j < length; j++)
            {
                string[] point = points[j].Split(',');
                Debug.WriteLine($"MainPage DownPiece {point[0]},{point[1]}");

                boardLayout.DownPiece(int.Parse(point[0]), int.Parse(point[1]), j + 1);
            }
            //if ((int)Application.Current.Properties["CurrentStep"] % 2 == 1)
            //{
            //    DoAI();
            //}
        }

        bool UpdateTimerLabel()
        {
            if (isGameStart)
            {
                timeLabel.Text = (DateTime.Now - gameStartTime).ToString(timeFormat);
                return true;
            }
            else
            {
                //return false;
            }
            return !isGameStart;
        }

        #region PageEvent

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
        void BoardLayout_TileTaped(object sender, Tile e)
        {
            if (!isGameStart)
            {
                BoardLayout_GameStarted();
            }
            Debug.WriteLine($"MainPage: BoardLayout_TileTaped {e.X} {e.Y} ");

            var boardLayout = sender as BoardLayout;
            if (boardLayout == null) return;
            CurrentBoard = boardLayout.CurrentBoard;
            if (!BoardList.Exists(a => a.Step == CurrentBoard.Step))
            {
                BoardList.Add(CurrentBoard);
                //判断是否胜
                if (CurrentBoard.IsWin)
                {
                    boardLayout.IsGameStart = isGameStart = false;
                    msgLb.Text = $"{CurrentBoard.DownPieces.LastOrDefault().Colour} IsWin! Start A New Game.";
                }
                DoAI();
            }
        }

        private void BoardLayout_GameStarted()
        {
            Debug.WriteLine($"MainPage: GameStart ");

            isGameStart = true;
            gameStartTime = DateTime.Now;
            Device.StartTimer(TimeSpan.FromSeconds(1), UpdateTimerLabel);

        }

        private void NewGameButton_Clicked(object sender, EventArgs e)
        {
            PrepareForNewGame();
        }
        private void NextButton_Clicked(object sender, EventArgs e)
        {
            GetNextStep(CurrentBoard);
        }

        #endregion

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
            catch (Exception e)
            {

            }
            return point;
        }

        private void DoAI()
        {
            //AI取点
            int CurrentStep = (int)Application.Current.Properties["CurrentStep"];
            if (CurrentStep >= 3 && CurrentStep % 2 == 1)
            {
                var point = GetNextStep(CurrentBoard);
                boardLayout.DownPiece((int)point.X, (int)point.Y, CurrentStep + 1);

            }

        }
    }

}

