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
        public static ImageSource blackImageSource = ImageSource.FromResource("GoBangXamarin.Image.X.png");
        public static ImageSource whiteImageSource = ImageSource.FromResource("GoBangXamarin.Image.O.png");
        public static ImageSource emptyImageSource = ImageSource.FromResource("GoBangXamarin.Image.empty+.png");
        public static ImageSource gbImageSource = ImageSource.FromResource("GoBangXamarin.Image.gb.png");
        public static ImageSource gbOImageSource = ImageSource.FromResource("GoBangXamarin.Image.gbO.png");
        public static ImageSource gbXImageSource = ImageSource.FromResource("GoBangXamarin.Image.gbX.png");

        public Tile(int row = 0, int col = 0)
        {
            Y = row;
            X = col;
            TileImage = new Image();
            //TileImage.SetValue(Image.SourceProperty, emptyImageSource);
            Tilestatus = TileStatus.Empty;

            TapGestureRecognizer singleTap = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1
            };
            singleTap.Tapped += OnSingleTap;
            TileImage.GestureRecognizers.Add(singleTap);
            //TileImage.PropertyChanged += TileImage_PropertyChanged;
        }

        private void TileImage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "Source")
            //{
            //    if (Tilestatus == TileStatus.Black)
            //    {
            //        if (TileImage.Source.Id != blackImageSource.Id)
            //        {
            //            Device.BeginInvokeOnMainThread(() =>
            //            {
            //                TileImage.SetValue(Image.SourceProperty, blackImageSource);
            //            });
            //            Debug.WriteLine($"MainPage: checkTileSource [{X},{Y}] Tilestatus:{Tilestatus }");
            //        }
            //    }
            //    if (Tilestatus == TileStatus.White)
            //    {
            //        if (TileImage.Source.Id != whiteImageSource.Id)
            //        {
            //            Device.BeginInvokeOnMainThread(() =>
            //            {
            //                TileImage.SetValue(Image.SourceProperty, whiteImageSource);
            //            });

            //            Debug.WriteLine($"MainPage: checkTileSource [{X},{Y}] Tilestatus:{Tilestatus }");

            //        }
            //    }

            //}
            if (e.PropertyName == "Tilestatus")
            {
                Debug.WriteLine($"TileImage_PropertyChanged: Tilestatus  {((Tile)sender).Tilestatus}");

            }
        }

        public void EmptyTile()
        {
            //if (Tilestatus == TileStatus.Empty) return;
            Tilestatus = TileStatus.Empty;
            TileImage.Source = emptyImageSource;
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
                    case TileStatus.BlackGB:
                        source = gbXImageSource;
                        break;
                    case TileStatus.WhiteGB:
                        source = gbOImageSource;
                        break;
                }
                TileImage.Source = source;

                //Device.BeginInvokeOnMainThread(() =>
                //{
                //TileImage.SetValue(Image.SourceProperty, source);
                //Debug.WriteLine($"Tile: empty {emptyImageSource.Id} black {blackImageSource.Id} white {whiteImageSource.Id}");

                //});
                Debug.WriteLine($"Tile: ChangeTileStatus  [{X},{Y}] ButtonStatus:{status }  TileImage  {source.Id}");
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
    White,
    BlackGB,
    WhiteGB

}

