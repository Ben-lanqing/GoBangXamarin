using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

namespace GoBangXamarin
{
    public class Tile
    {
        #region public
        public int Y { private set; get; }
        public int X { private set; get; }
        //public Image TileImage { private set; get; }
        public TileStatus Tilestatus { set; get; }
        public event EventHandler SingleTaped;
        #endregion

        //static ImageSource blackImageSource = ImageSource.FromResource("GoBangXamarin.Image.black.png");
        //static ImageSource whiteImageSource = ImageSource.FromResource("GoBangXamarin.Image.white.png");
        //static ImageSource emptyImageSource = ImageSource.FromResource("GoBangXamarin.Image.empty.png");
        static ImageSource blackImageSource = ImageSource.FromResource("GoBangXamarin.Image.X.png");
        static ImageSource whiteImageSource = ImageSource.FromResource("GoBangXamarin.Image.O.png");
        static ImageSource emptyImageSource = ImageSource.FromResource("GoBangXamarin.Image.empty+.png");
        //public static ImageSource gbImageSource = ImageSource.FromResource("GoBangXamarin.Image.gb.png");
        static ImageSource gbOImageSource = ImageSource.FromResource("GoBangXamarin.Image.gbO.png");
        static ImageSource gbXImageSource = ImageSource.FromResource("GoBangXamarin.Image.gbX.png");

        public Image EmptyImage = new Image() { Source = emptyImageSource, IsVisible = true };
        public Image BlackImage = new Image() { Source = blackImageSource, IsVisible = false };
        public Image WhiteImage = new Image() { Source = whiteImageSource, IsVisible = false };
        public Image GbOImage = new Image() { Source = gbOImageSource, IsVisible = false };
        public Image GbXImage = new Image() { Source = gbXImageSource, IsVisible = false };

        public Tile(int row = 0, int col = 0)
        {
            Y = row;
            X = col;
            //TileImage = new Image();
            //TileImage.Source = emptyImageSource;
            Tilestatus = TileStatus.Empty;

            TapGestureRecognizer singleTap = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1
            };
            singleTap.Tapped += OnSingleTap;
            EmptyImage.GestureRecognizers.Add(singleTap);
            BlackImage.GestureRecognizers.Add(singleTap);
            WhiteImage.GestureRecognizers.Add(singleTap);
            GbOImage.GestureRecognizers.Add(singleTap);
            GbXImage.GestureRecognizers.Add(singleTap);
        }


        public void EmptyTile()
        {
            //if (Tilestatus == TileStatus.Empty) return;
            Tilestatus = TileStatus.Empty;
            ChangeTileImageVisible(TileStatus.Empty);
            //TileImage.Source = emptyImageSource;
            //Device.BeginInvokeOnMainThread(() =>
            //{
            //    TileImage.SetValue(Image.SourceProperty, emptyImageSource);
            //});
        }

        void OnSingleTap(object sender, object args)
        {
            SingleTaped?.Invoke(this, null);
        }
        public void ChangeTileStatus(TileStatus status)
        {
            try
            {
                Debug.WriteLine($"Tile: ChangeTileStatus  {Tilestatus} to {status}");
                Tilestatus = status;
                //var source = emptyImageSource;
                ChangeTileImageVisible(status);
                //switch (status)
                //{
                //    case TileStatus.Empty:
                //        //source = emptyImageSource;
                //        break;

                //    case TileStatus.Black:
                //        //source = blackImageSource;
                //        break;

                //    case TileStatus.White:
                //        //source = whiteImageSource;
                //        break;
                //    case TileStatus.BlackGB:
                //        //source = gbXImageSource;
                //        break;
                //    case TileStatus.WhiteGB:
                //        //source = gbOImageSource;
                //        break;
                //}
                //Device.BeginInvokeOnMainThread(async () =>
                //{
                //    await ChangeSource(source);

                //});
                //Device.BeginInvokeOnMainThread(() =>
                //{
                //TileImage.SetValue(Image.SourceProperty, source);
                //Debug.WriteLine($"Tile: empty {emptyImageSource.Id} black {blackImageSource.Id} white {whiteImageSource.Id}");

                //});
                //Debug.WriteLine($"Tile: ChangeTileStatus  [{X},{Y}] ButtonStatus:{status }  TileImage  {source.Id}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(StaticClass.LogException("ChangeTileStatus", ex));
            }
        }

        //private async Task ChangeSource(ImageSource source)
        //{
        //    TileImage.Source = source;

        //}
        public void ChangeTileImageVisible(TileStatus status)
        {
            switch (status)
            {
                case TileStatus.Empty:
                    EmptyImage.IsVisible = true;
                    BlackImage.IsVisible = false;
                    WhiteImage.IsVisible = false;
                    GbOImage.IsVisible = false;
                    GbXImage.IsVisible = false;
                    break;

                case TileStatus.Black:
                    EmptyImage.IsVisible = false;
                    BlackImage.IsVisible = true;
                    WhiteImage.IsVisible = false;
                    GbOImage.IsVisible = false;
                    GbXImage.IsVisible = false;
                    break;

                case TileStatus.White:
                    EmptyImage.IsVisible = false;
                    BlackImage.IsVisible = false;
                    WhiteImage.IsVisible = true;
                    GbOImage.IsVisible = false;
                    GbXImage.IsVisible = false;
                    break;
                case TileStatus.BlackGB:
                    EmptyImage.IsVisible = false;
                    BlackImage.IsVisible = false;
                    WhiteImage.IsVisible = false;
                    GbXImage.IsVisible = true;
                    GbOImage.IsVisible = false;
                    break;
                case TileStatus.WhiteGB:
                    EmptyImage.IsVisible = false;
                    BlackImage.IsVisible = false;
                    WhiteImage.IsVisible = false;
                    GbXImage.IsVisible = false;
                    GbOImage.IsVisible = true;
                    break;
            }


        }

    }
}
public enum TileStatus
{
    Empty,
    Black,
    White,
    BlackGB,
    WhiteGB

}

