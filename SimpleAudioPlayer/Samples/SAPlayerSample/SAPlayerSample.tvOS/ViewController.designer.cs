// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SAPlayerSample.tvOS
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel isPlaying { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton playButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton stopButton { get; set; }

        [Action ("PlayButton_PrimaryActionTriggered:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void PlayButton_PrimaryActionTriggered (UIKit.UIButton sender);

        [Action ("StopButton_PrimaryActionTriggered:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void StopButton_PrimaryActionTriggered (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (isPlaying != null) {
                isPlaying.Dispose ();
                isPlaying = null;
            }

            if (playButton != null) {
                playButton.Dispose ();
                playButton = null;
            }

            if (stopButton != null) {
                stopButton.Dispose ();
                stopButton = null;
            }
        }
    }
}