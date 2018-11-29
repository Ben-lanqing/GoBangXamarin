using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GoBangXamarin
{
    public class Tile
    {
        #region public
        public int Y { private set; get; }
        public int X { private set; get; }
        public Image TileImage { private set; get; }
        public TileStatus Tilestatus { set; get; }
        public event EventHandler SingleTaped;
        #endregion

        //static ImageSource blackImageSource = ImageSource.FromResource("GoBangXamarin.Image.black.png");
        //static ImageSource whiteImageSource = ImageSource.FromResource("GoBangXamarin.Image.white.png");
        //static ImageSource emptyImageSource = ImageSource.FromResource("GoBangXamarin.Image.empty.png");
        static ImageSource blackImageSource = ImageSource.FromResource("GoBangXamarin.Image.X.png");
        static ImageSource whiteImageSource = ImageSource.FromResource("GoBangXamarin.Image.O.png");
        static ImageSource emptyImageSource = ImageSource.FromResource("GoBangXamarin.Image.empty+.png");
        static ImageSource gbImageSource = ImageSource.FromResource("GoBangXamarin.Image.gb.png");

        public Tile(int row = 0, int col = 0)
        {
            Y = row;
            X = col;
            TileImage = new Image();
            Tilestatus = TileStatus.Empty;

            TapGestureRecognizer singleTap = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1
            };
            singleTap.Tapped += OnSingleTap;
            TileImage.GestureRecognizers.Add(singleTap);
        }
        public void Initialize()
        {
            Tilestatus = TileStatus.Empty;
            //TileImage.Source = emptyImageSource;
            Device.BeginInvokeOnMainThread(() =>
            {
                TileImage.SetValue(Image.SourceProperty, emptyImageSource);
            });
        }

        void OnSingleTap(object sender, object args)
        {
            SingleTaped?.Invoke(this, null);
        }
        public void ChangeTileStatus(TileStatus status)
        {
            try
            {
                var source = emptyImageSource;
                switch (status)
                {
                    case TileStatus.Empty:
                        source = emptyImageSource;
                        break;

                    case TileStatus.Black:
                        source = blackImageSource;
                        break;

                    case TileStatus.White:
                        source = whiteImageSource;
                        break;
                }
                //TileImage.Source = emptyImageSource;

                Device.BeginInvokeOnMainThread(() =>
                {
                    TileImage.SetValue(Image.SourceProperty, source);
                });
                Debug.WriteLine($"Tile: Tilestatus  [{X},{Y}] ButtonStatus:{status }");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(StaticClass.LogException("ChangeTileStatus", ex));
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

