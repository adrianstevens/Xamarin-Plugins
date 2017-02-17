using Plugin.SimpleAudioPlayer.Abstractions;
using System;

namespace Plugin.SimpleAudioPlayer
{
  /// <summary>
  /// Cross platform SimpleAudioPlayer implemenations
  /// </summary>
  public class CrossSimpleAudioPlayer
  {
    static Lazy<ISimpleAudioPlayer> Implementation = new Lazy<ISimpleAudioPlayer>(() => CreateSimpleAudioPlayer(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

    /// <summary>
    /// Current settings to use
    /// </summary>
    public static ISimpleAudioPlayer Current
    {
      get
      {
        var ret = Implementation.Value;
        if (ret == null)
        {
          throw NotImplementedInReferenceAssembly();
        }
        return ret;
      }
    }

    public static ISimpleAudioPlayer CreateSimpleAudioPlayer()
    {
#if PORTABLE
        return null;
#else
        return new SimpleAudioPlayerImplementation();
#endif
    }

    internal static Exception NotImplementedInReferenceAssembly()
    {
      return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
    }
  }
}
