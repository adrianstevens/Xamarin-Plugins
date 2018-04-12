### SimpleAudioRecorder
SimpleAudioRecorder records audio and stores it on the local file system.

### Setup
* Available on NuGet: http://www.nuget.org/packages/Xam.Plugin.SimpleAudioRecorder [![NuGet](https://img.shields.io/nuget/v/Xam.Plugin.SimpleAudioRecorder.svg?label=NuGet)](https://www.nuget.org/packages/Xam.Plugin.SimpleAudioRecorder/)
* Install into your shared project and client projects
* Requires JSON.Net

**Platform Support**

|Platform|Version|
| ------------------- | :------------------: |
|Xamarin.iOS|iOS 7+|
|Xamarin.Android|API 16+|
|Windows 10 UWP|10+|
|Xamarin.Mac|All|

**Permissions/Requirements**

***UWP*** 

Add *Internet* and  *Microphone* capabilities

***Android***

Add MODIFY_AUDIO_SETTINGS, RECORD_AUDIO, READ_EXTERNAL_STORAGE & WRITE_EXTERNAL_STORAGE permissions

***iOS***

NSMicrophoneUsageDescription + purpose string




#### Contributions
Contributions are welcome! If you find a bug want a feature added please report it.

If you want to contribute code please file an issue, create a branch, and file a pull request.

#### License 
MIT License - see LICENSE.txt

