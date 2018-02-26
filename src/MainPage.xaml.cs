using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Newtonsoft.Json;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TiledUwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        CanvasBitmap _spriteSheet;
        TiledDrawer _tiledDrawer;

        public MainPage()
        {
            this.InitializeComponent();
        }

        void onCanvasDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            args.DrawingSession.Clear(Colors.Black);
            _tiledDrawer.Draw(args.DrawingSession);

        }

        void onCanvasUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {

        }

        void onCanvasCreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(createResourcesAsync(sender).AsAsyncAction());
        }

        async Task createResourcesAsync(CanvasAnimatedControl sender)
        {
            _spriteSheet = await CanvasBitmap.LoadAsync(resourceCreator: sender, fileName: "assets/dirt-tiles.png");

            //string jsonString = await DeserializeFileAsync("./Assets/tilemap.json");
            _tiledDrawer = await TiledDrawer.Create(_spriteSheet, @"Assets\tilemap.json");
        }
    }

    public class TiledDrawer
    {
        private CanvasBitmap _spriteSheet;
        private TiledProperties _properties;

        TiledDrawer(CanvasBitmap spriteSheet, TiledProperties properties)
        {
            this._spriteSheet = spriteSheet;
            this._properties = properties;
        }

        public static async Task<TiledDrawer> Create(CanvasBitmap spriteSheet, string filename)
        {
            StorageFile sFile = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(filename);
            //            StorageFile sFile = await StorageFile.GetFileFromPathAsync(filename);
            Stream fileStream = await sFile.OpenStreamForReadAsync();


            var s = new StreamReader(fileStream);

            var ser = new JsonSerializer();

            TiledProperties props;
            using (var s2 = new JsonTextReader(s))
            {
                s2.CloseInput = true;
                props = ser.Deserialize<TiledProperties>(s2);
            }


            return new TiledDrawer(spriteSheet, props);
        }

        public void Draw(CanvasDrawingSession argsDrawingSession)
        {
            const int tileImageWidth = 24;

            for (int i = 0; i < _properties.Layers.Length; i++)
            {
                var l = _properties.Layers[i];
                if (l.Type == "tilelayer" && l.Visible)
                {
                    for (int y = 0; y < l.Height; y++)
                    {
                        for (int x = 0; x < l.Width; x++)
                        {
                            int d = l.Data[y * l.Width + x];
                            if (d == 0)
                            {
                                continue;
                            }

                            d = d -1;

                            Rect destinationRectangle = new Rect(new Point(x * _properties.TileWidth, y * _properties.TileHeight), new Size(_properties.TileWidth, _properties.TileHeight));

                            var location = new Point((d % tileImageWidth) * _properties.TileWidth,
                                d / tileImageWidth * _properties.TileWidth);

                            Rect sourceRectangle = new Rect(
                                location,
                                new Size(_properties.TileWidth, _properties.TileHeight));

                            argsDrawingSession.DrawImage(_spriteSheet,
                                destinationRectangle,
                                sourceRectangle);
                        }
                    }
                }
            }
        }
    }
}
