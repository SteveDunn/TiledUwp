namespace TiledUwp
{
    public class TiledProperties
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public bool Infinite { get; set; }

        public LayerProperties[] Layers { get; set; }

        public int NextObjectId { get; set; }
        public string Orientation { get; set; }

        public string RenderOrder { get; set; }
        public string TiledVersion { get; set; }
        public int TileHeight { get; set; }
        public TileSetProperties[] TileSets { get; set; }
        public int TileWidth { get; set; }
        public string Type { get; set; }
        public string Version { get; set; }
    }

    public class TileSetProperties
    {
        public string Firstgid { get; set; }
        public string Source { get; set; }

    }

    public class LayerProperties
    {
        public int[] Data { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float Opacity { get; set; }
        public string DrawOrder { get; set; }
        public string Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool Visible { get; set; }
    }
}