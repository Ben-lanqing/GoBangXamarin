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
        public List<Board> BoardList;
        public int CurrentStep;
        public GoBangHepler hepler;
        public Board CurrentBoard;
        public ObservableCollection<Piece> downPList;
        //SKCanvas sKCanvas;
        //SKBitmap bitmap;
        Dictionary<Guid, Point> btDic;
        bool isGameInProgress;
        DateTime gameStartTime;
        const string timeFormat = @"%m\:ss";

        public MainPage()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            boardLayout.TileTaped += BoardLayout_TileTaped;
            CurrentStep = 0;
            BoardList = new List<Board>();
            CurrentBoard = new Board();
            downPList = new ObservableCollection<Piece>();
            //PieceList.ItemsSource = downPList;
            btDic = new Dictionary<Guid, Point>();
            //for (int x = 1; x <= 15; x++)
            //{
            //    for (int y = 0; y < 15; y++)
            //    {
            //        Button button = new Button();
            //        //button.Text = $"{x},{y}";
            //        button.Clicked += Button_Clicked;
            //        button.WidthRequest = 25;
            //        button.HeightRequest = 25;
            //        btDic.Add(button.Id, new Point(x, y));
            //        GridTable.Children.Add(button, x, y);

            //    }
            //}
            //for (int i = 0; i < 15; i++)
            //{
            //    Label labelx = new Label();
            //    labelx.Text = (i + 1).ToString();
            //    Label labely = new Label();
            //    labely.Text = (i + 1).ToString();
            //    GridTable.Children.Add(labelx, 0, i);
            //    GridTable.Children.Add(labely, i + 1, 15);

            //}
            boardLayout.GameStarted += (sender, args) =>
            {
                isGameInProgress = true;
                gameStartTime = DateTime.Now;

                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    timeLabel.Text = (DateTime.Now - gameStartTime).ToString(timeFormat);
                    return isGameInProgress;
                });
            };


        }

        private void BoardLayout_TileTaped(object sender, Tile e)
        {
            var board = sender as Board;
            if (board == null) return;
            CurrentStep = e.CurrentStep ?? +1;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var p = btDic[button.Id];
            int x = (int)p.X;
            int y = (int)p.Y;

            bool down = DownPiece(x - 1, y);
            if (down)
            {
                button.BackgroundColor = CurrentStep % 2 == 1 ? Color.Black : Color.White;
            }
            else
            {
                //msg.Text = $"{x},{y} Not Empty";
            }
        }
        //private void DownButton_Clicked(object sender, EventArgs e)
        //{
        //    int x = Convert.ToInt32(PX.Text);
        //    int y = Convert.ToInt32(PY.Text);
        //    DownPiece(x, y);
        //}
        private bool DownPiece(int x, int y)
        {
            if (CurrentBoard.DownPieces.Exists(a => a.X == x && a.Y == y)) return false;
            CurrentStep++;
            var newBoard = CurrentBoard.ChangeBoard(x, y, CurrentStep);
            CurrentBoard = newBoard;
            BoardList.Add(newBoard);
            downPList.Clear();
            foreach (var p in newBoard.DownPieces)
            {
                downPList.Add(p);
            }
            string str = $"DownButton:x:{x} y:{y} Step:{CurrentStep}";
            //msg.Text = str;
            UpdateCurrent();

            return true;
        }
        private void NextButton_Clicked(object sender, EventArgs e)
        {
            hepler = new GoBangHepler(CurrentBoard);
            PieceInfo pieceInfo = hepler.AIGetNext(CurrentBoard);
            string str = $"NextPiece:  x:{pieceInfo.X} y:{pieceInfo.Y} ";

            //msg.Text = str;
        }

        private void UpdateCurrent()
        {
            Application.Current.Properties["CurrentStep"] = CurrentStep;
            //Application.Current.Properties["BoardList"] = BoardList;

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

        // Maintains a square aspect ratio for the board.
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
    }
}
