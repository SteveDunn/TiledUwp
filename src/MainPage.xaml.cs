using System;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TiledUwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        CanvasBitmap _spriteSheet;
        TiledParser_loose _tiledDrawerStronlytyped;

        public MainPage()
        {
            this.InitializeComponent();
        }

        void onCanvasDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            args.DrawingSession.Clear(Colors.Black);
            _tiledDrawerStronlytyped.Draw(args.DrawingSession);

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
            _tiledDrawerStronlytyped = await TiledParser_loose.Create(_spriteSheet, @"Assets\tilemap.json");
        }
    }
}
