using AppKit;
using Foundation;
using Xamarin.Forms;
using Xamarin.Forms.Platform.MacOS;

namespace SAPlayerSample.macOS
{
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate
    {
        NSWindow window;

        public override NSWindow MainWindow
        {
            get
            {
                return window;
            }
        }

        public AppDelegate()
        {
            var style = NSWindowStyle.Resizable | NSWindowStyle.Titled | NSWindowStyle.Closable;
            var rect = new CoreGraphics.CGRect(100, 100, 1024, 768);
            window = new NSWindow(rect, style, NSBackingStore.Buffered, false);
            window.Title = "SAP Sample";
            window.TitleVisibility = NSWindowTitleVisibility.Hidden;
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            Forms.Init();
            LoadApplication(new App());
            base.DidFinishLaunching(notification);
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }
    }
}
