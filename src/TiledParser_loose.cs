using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Microsoft.Graphics.Canvas;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TiledUwp
{
    public class TiledParser_loose
    {
        CanvasBitmap _spriteSheet;
        JObject _properties;

        TiledParser_loose(CanvasBitmap spriteSheet, JObject properties)
        {
            _spriteSheet = spriteSheet;
            _properties = properties;
        }

        public static async Task<TiledParser_loose> Create(CanvasBitmap spriteSheet, string filename)
        {
            StorageFile sFile = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(filename);
            Stream fileStream = await sFile.OpenStreamForReadAsync();


            var s = new StreamReader(fileStream);

            var ser = new JsonSerializer();

            using (var s2 = new JsonTextReader(s))
            {
                s2.CloseInput = true;

                var loose = ser.Deserialize<JObject>(s2);
                return new TiledParser_loose(spriteSheet, loose);
            }
        }

        public void Draw(CanvasDrawingSession argsDrawingSession)
        {
            const int tileImageWidth = 24;

            var jEnumerable = _properties["layers"].Values<JObject>();

            for (int i = 0; i < jEnumerable.Count(); i++)
            {
                var l = jEnumerable.ElementAt(i);
                string value = l["type"].Value<string>();

                var visible = l["visible"].Value<bool>();
                if (value == "tilelayer" && visible)
                {
                    handleTileLayer(argsDrawingSession, l, tileImageWidth);
                }

                if (value == "objectgroup" && visible)
                {
                    handleObjectGroup(argsDrawingSession, l, tileImageWidth);
                }
            }
        }

        void handleTileLayer(CanvasDrawingSession argsDrawingSession, JObject layer, int tileImageWidth)
        {
            int[] vals = layer["data"].Values<int>().ToArray();

            var height = layer["height"].Value<int>();

            for (int y = 0; y < height; y++)
            {
                var width = layer["width"].Value<int>();

                for (int x = 0; x < width; x++)
                {

                    int d =  vals[y * width + x];
                    if (d == 0)
                    {
                        continue;
                    }

                    d = d - 1;

                    var tileWidth = _properties["tilewidth"].Value<int>();
                    var tileHeight = _properties["tileheight"].Value<int>();

                    Rect destinationRectangle = new Rect(new Point(x * tileWidth, y * tileHeight), new Size(tileWidth, tileHeight));

                    var location = new Point((d % tileImageWidth) * tileWidth, d / tileImageWidth * tileWidth);

                    Rect sourceRectangle = new Rect(
                        location,
                        new Size(tileWidth, tileHeight));

                    argsDrawingSession.DrawImage(_spriteSheet,
                        destinationRectangle,
                        sourceRectangle);
                }
            }
        }

        void handleObjectGroup(CanvasDrawingSession argsDrawingSession, JObject l, int tileImageWidth)
        {
//            for (int y = 0; y < l.Height; y++)
//            {
//                for (int x = 0; x < l.Width; x++)
//                {
//                    int d = l.Data[y * l.Width + x];
//                    if (d == 0)
//                    {
//                        continue;
//                    }
//
//                    d = d - 1;
//
//                    Rect destinationRectangle = new Rect(new Point(x * _properties.TileWidth, y * _properties.TileHeight),
//                        new Size(_properties.TileWidth, _properties.TileHeight));
//
//                    var location = new Point((d % tileImageWidth) * _properties.TileWidth,
//                        d / tileImageWidth * _properties.TileWidth);
//
//                    Rect sourceRectangle = new Rect(
//                        location,
//                        new Size(_properties.TileWidth, _properties.TileHeight));
//
//                    argsDrawingSession.DrawImage(_spriteSheet,
//                        destinationRectangle,
//                        sourceRectangle);
//                }
//            }
        }
    }
}