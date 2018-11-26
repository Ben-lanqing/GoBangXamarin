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
        //public ContentView TileView { private set; get; }
        public Image TileImage { private set; get; }

        //static ImageSource blackImageSource = ImageSource.FromResource("GoBangXamarin.Image.black.png");
        //static ImageSource whiteImageSource = ImageSource.FromResource("GoBangXamarin.Image.white.png");
        //static ImageSource emptyImageSource = ImageSource.FromResource("GoBangXamarin.Image.empty.png");
        static ImageSource blackImageSource = ImageSource.FromResource("GoBangXamarin.Image.X.png");
        static ImageSource whiteImageSource = ImageSource.FromResource("GoBangXamarin.Image.O.png");
        static ImageSource emptyImageSource = ImageSource.FromResource("GoBangXamarin.Image.gb.png");
        static ImageSource gbImageSource = ImageSource.FromResource("GoBangXamarin.Image.gb.png");

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
                            //TileView.Content = emptyImage;
                            TileImage.SetValue(Image.SourceProperty, emptyImageSource);
                            break;

                        case TileStatus.Black:
                            //TileView.Content = blackImage;
                            TileImage.SetValue(Image.SourceProperty, blackImageSource);
                            break;

                        case TileStatus.White:
                            //TileView.Content = whiteImage;
                            TileImage.SetValue(Image.SourceProperty, whiteImageSource);

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

        public event EventHandler TileStatusChanged;
        public Tile(int row = 0, int col = 0)
        {
            Y = row;
            X = col;
            TileImage = new Image();
            //TileView = new Frame
            //{
            //    Opacity = 0.1,
            //    //Content = hiddenLabel,
            //    //BackgroundColor = Color.p,
            //    //BorderColor = Color.Black,
            //    Padding = new Thickness(1)
            //};

            TapGestureRecognizer singleTap = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1
            };
            singleTap.Tapped += OnSingleTap;
            //TileView.GestureRecognizers.Add(singleTap);
            TileImage.GestureRecognizers.Add(singleTap);

        }
        public void Initialize()
        {
            doNotFireEvent = true;
            buttonStatus = TileStatus.Empty;
            //TileView.Content = emptyImage;
            TileImage.SetValue(Image.SourceProperty, emptyImageSource);

            doNotFireEvent = false;
        }

        void OnSingleTap(object sender, object args)
        {
            if (buttonStatus == TileStatus.Empty)
            {
                int nextStep = (int)Application.Current.Properties["CurrentStep"] + 1;
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
}
