using System;

namespace Plugin.SimpleAudioPlayer
{
    /// <summary>
    /// Cross platform SimpleAudioPlayer implemenations
    /// </summary>
#if __ANDROID__
    [Android.Runtime.Preserve(AllMembers = true)]
#endif
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

		///<Summary>
		/// Create a new SimpleAudioPlayer object
		///</Summary>
		public static ISimpleAudioPlayer CreateSimpleAudioPlayer()
      {
#if NETSTANDARD1_0
          return null;
#else
          return new SimpleAudioPlayerImplementation();
#endif
      }

      internal static Exception NotImplementedInReferenceAssembly()
      {
         return new NotImplementedException("This functionality is not implemented in the .NET standard version of this assembly. Reference the NuGet package from your platform-specific (head) application project in order to reference the platform-specific implementation.");
      }
   }
}