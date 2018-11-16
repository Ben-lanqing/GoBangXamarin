using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace GoBangXamarin
{
    public class Tile
    {
        public int Row { private set; get; }

        public int Col { private set; get; }
        public ContentView TileView { private set; get; }

        Label hiddenLabel = new Label
        {
            Text = " ",
            TextColor = Color.Yellow,
            BackgroundColor = Color.Yellow
        };
        static ImageSource blackImageSource = ImageSource.FromResource("GoBangXamarin.Image.black.png");
        static ImageSource whiteImageSource = ImageSource.FromResource("GoBangXamarin.Image.white.png");
        static ImageSource gbImageSource = ImageSource.FromResource("GoBangXamarin.Image.gb.png");
        Image blackImage = new Image
        {
            Source = blackImageSource
        };
        Image whiteImage = new Image
        {
            Source = whiteImageSource
        };
        Image gbImage = new Image
        {
            Source = gbImageSource
        };
        PieceButtonStatus buttonStatus = PieceButtonStatus.Empty;
        public PieceButtonStatus ButtonStatus
        {
            set
            {
                if (buttonStatus != value)
                {
                    buttonStatus = value;

                    switch (buttonStatus)
                    {
                        case PieceButtonStatus.Empty:
                            TileView.Content = gbImage;
                            break;

                        case PieceButtonStatus.Black:
                            TileView.Content = blackImage;
                            break;

                        case PieceButtonStatus.White:
                            TileView.Content = whiteImage;

                            break;
                    }

                    if (!doNotFireEvent)
                    {
                        TileStatusChanged?.Invoke(this, null);
                    }
                }
            }
            get
            {
                return buttonStatus;
            }
        }

        bool doNotFireEvent;
        public int? CurrentStep = 0;

        public event EventHandler TileStatusChanged;
        public Tile(int row = 0, int col = 0)
        {
            Row = row;
            Col = col;

            TileView = new Frame
            {
                Content = hiddenLabel,
                BackgroundColor = Color.Black,
                OutlineColor = Color.Black,
                Padding = new Thickness(1)
            };

            TapGestureRecognizer singleTap = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1
            };
            singleTap.Tapped += OnSingleTap;
            TileView.GestureRecognizers.Add(singleTap);


        }
        public void Initialize()
        {
            //doNotFireEvent = true;
            buttonStatus = PieceButtonStatus.Empty;
            //doNotFireEvent = false;
        }

        void OnSingleTap(object sender, object args)
        {
            if (buttonStatus == PieceButtonStatus.Empty)
            {
                CurrentStep = Application.Current.Properties["CurrentStep"] as int?;
                int? nextStep = CurrentStep == null ? 1 : CurrentStep + 1;
                if (nextStep % 2 == 1)
                {
                    ButtonStatus = PieceButtonStatus.Black;
                }
                else
                {
                    ButtonStatus = PieceButtonStatus.White;
                }
            }
        }


    }
    public enum PieceButtonStatus
    {
        Empty,
        Black,
        White
    }
    public class PieceButtonTapEventArgs : EventArgs
    {
        public Tile TapedTile { get; set; }
    }
}
