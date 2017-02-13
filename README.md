# Xamarin-Plugins
Cross-platform Plugins for Xamarin and Xamarin.Forms

## SimpleAudioPlayer
SimpleAudioPlayer plays audio data as a stream. This allows you to store audio data in a portable class library and play it on all supported platforms.

### Example
Coded in the shared PCL using the Xamarin.Forms dependancy service
with **mysound.wav** stored in the PCL as an **Embedded Resource**
```
ISimpleAudioPlayer player = DependancyService.Get<ISimpleAudioPlayer>();
player.Load(GetStreamFromFile("mysound.wav"));
player.Play();
```
