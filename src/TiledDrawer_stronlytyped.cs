using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Microsoft.Graphics.Canvas;
using Newtonsoft.Json;

namespace TiledUwp
{
    public class TiledDrawer_stronlytyped
    {
        readonly CanvasBitmap _spriteSheet;
        readonly TiledProperties _properties;

        TiledDrawer_stronlytyped(CanvasBitmap spriteSheet, TiledProperties properties)
        {
            this._spriteSheet = spriteSheet;
            this._properties = properties;
        }

        public static async Task<TiledDrawer_stronlytyped> Create(CanvasBitmap spriteSheet, string filename)
        {
            StorageFile sFile = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(filename);
            Stream fileStream = await sFile.OpenStreamForReadAsync();


            var s = new StreamReader(fileStream);

            var ser = new JsonSerializer();

            TiledProperties props;
            using (var s2 = new JsonTextReader(s))
            {
                s2.CloseInput = true;
                props = ser.Deserialize<TiledProperties>(s2);
            }

            return new TiledDrawer_stronlytyped(spriteSheet, props);
        }

        public void Draw(CanvasDrawingSession argsDrawingSession)
        {
            const int tileImageWidth = 24;

            for (int i = 0; i < _properties.Layers.Length; i++)
            {
                var l = _properties.Layers[i];
                if (l.Type == "tilelayer" && l.Visible)
                {
                    handleTileLayer(argsDrawingSession, l, tileImageWidth);
                }
                if (l.Type == "objectgroup" && l.Visible)
                {
                    handleObjectGroup(argsDrawingSession, l, tileImageWidth);
                }
            }
        }

        void handleTileLayer(CanvasDrawingSession argsDrawingSession, LayerProperties l, int tileImageWidth)
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

                    d = d - 1;

                    Rect destinationRectangle = new Rect(new Point(x * _properties.TileWidth, y * _properties.TileHeight),
                        new Size(_properties.TileWidth, _properties.TileHeight));

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

        void handleObjectGroup(CanvasDrawingSession argsDrawingSession, LayerProperties l, int tileImageWidth)
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

                    d = d - 1;

                    Rect destinationRectangle = new Rect(new Point(x * _properties.TileWidth, y * _properties.TileHeight),
                        new Size(_properties.TileWidth, _properties.TileHeight));

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