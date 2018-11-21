using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace GoBangXamarin
{
    public class Tile
    {
        public int Y { private set; get; }

        public int X { private set; get; }
        public ContentView TileView { private set; get; }
        //public Image TileImage { private set; get; }

        Label hiddenLabel = new Label
        {
            Text = " ",
            TextColor = Color.Yellow,
            BackgroundColor = Color.Yellow
        };
        static ImageSource blackImageSource = ImageSource.FromResource("GoBangXamarin.Image.black.png");
        static ImageSource whiteImageSource = ImageSource.FromResource("GoBangXamarin.Image.white.png");
        static ImageSource gbImageSource = ImageSource.FromResource("GoBangXamarin.Image.gb.png");
        static ImageSource emptyImageSource = ImageSource.FromResource("GoBangXamarin.Image.empty.png");
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
        Image emptyImage = new Image
        {
            Source = emptyImageSource
        };
        TileStatus buttonStatus = TileStatus.Empty;
        public TileStatus Tilestatus
        {
            set
            {
                if (buttonStatus != value)
                {
                    buttonStatus = value;

                    switch (buttonStatus)
                    {
                        case TileStatus.Empty:
                            TileView.Content = emptyImage;
                            //TileImage = emptyImage;
                            break;

                        case TileStatus.Black:
                           TileView.Content = blackImage;
                             //TileImage = blackImage;
                            break;

                        case TileStatus.White:
                            TileView.Content = whiteImage;
                            //TileImage = whiteImage;

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
        public int CurrentStep = 0;

        public event EventHandler TileStatusChanged;
        public Tile(int row = 0, int col = 0)
        {
            Y = row;
            X = col;
            //TileImage = gbImage;

            TileView = new Frame
            {
                //Content = hiddenLabel,
                //BackgroundColor = Color.Yellow,
                //BorderColor = Color.Black,
                Padding = new Thickness(1)
            };

            TapGestureRecognizer singleTap = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1
            };
            singleTap.Tapped += OnSingleTap;
            TileView.GestureRecognizers.Add(singleTap);
            //TileImage.GestureRecognizers.Add(singleTap);

        }
        public void Initialize()
        {
            doNotFireEvent = true;
            buttonStatus = TileStatus.Empty;
            CurrentStep = 0;
            TileView.Content = emptyImage;
            doNotFireEvent = false;
        }

        void OnSingleTap(object sender, object args)
        {
            if (buttonStatus == TileStatus.Empty)
            {
                CurrentStep = (int)Application.Current.Properties["CurrentStep"];
                int nextStep = CurrentStep + 1;
                if (nextStep % 2 == 1)
                {
                    Tilestatus = TileStatus.Black;
                }
                else
                {
                    Tilestatus = TileStatus.White;
                }
            }
        }


    }
    public enum TileStatus
    {
        Empty,
        Black,
        White
    }
    //public class PieceButtonTapEventArgs : EventArgs
    //{
    //    public Tile TapedTile { get; set; }
    //}
}
