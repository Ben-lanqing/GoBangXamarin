using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Diagnostics;

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
        static ImageSource emptyImageSource = ImageSource.FromResource("GoBangXamarin.Image.empty+.png");
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
                            while (TileImage.Source.Id != emptyImageSource.Id)
                            {
                                TileImage.SetValue(Image.SourceProperty, emptyImageSource);
                            }
                            break;

                        case TileStatus.Black:
                            //TileView.Content = blackImage;
                            while (TileImage.Source.Id != blackImageSource.Id)
                            {
                                TileImage.SetValue(Image.SourceProperty, blackImageSource);
                            }
                            break;

                        case TileStatus.White:
                            //TileView.Content = whiteImage;
                            while (TileImage.Source.Id != whiteImageSource.Id)
                            {
                                TileImage.SetValue(Image.SourceProperty, whiteImageSource);
                            }
                            break;
                    }
                    Debug.WriteLine($"Tile: Tilestatus  [{X},{Y}] ButtonStatus:{buttonStatus }");

                }
            }
            get
            {
                return buttonStatus;
            }
        }

        public event EventHandler SingleTaped;
        public Tile(int row = 0, int col = 0)
        {
            Y = row;
            X = col;
            TileImage = new Image();
            TapGestureRecognizer singleTap = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1
            };
            singleTap.Tapped += OnSingleTap;
            TileImage.GestureRecognizers.Add(singleTap);
        }
        public void Initialize()
        {
            buttonStatus = TileStatus.Empty;
            TileImage.SetValue(Image.SourceProperty, emptyImageSource);
        }

        void OnSingleTap(object sender, object args)
        {
            if (buttonStatus != TileStatus.White && buttonStatus != TileStatus.Black)
            {
                SingleTaped?.Invoke(this, null);
            }
            else
            {
                Debug.WriteLine($"Tile: OnSingleTap Err [{X},{Y}] ButtonStatus:{buttonStatus }");
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
