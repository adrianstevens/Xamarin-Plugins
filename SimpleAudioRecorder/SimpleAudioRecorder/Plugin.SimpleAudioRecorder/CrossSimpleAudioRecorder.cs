using System;
using System.Collections.Generic;
using System.Text;

namespace Plugin.SimpleAudioRecorder
{
    /// <summary>
    /// Cross platform SimpleAudioPlayer implemenations
    /// </summary>
    public class CrossSimpleAudioRecorder
    {
        ///<Summary>
		/// Create a new SimpleAudioPlayer object
		///</Summary>
		public static ISimpleAudioRecorder CreateSimpleAudioRecorder()
        {
#if NETSTANDARD1_3
          return null;
#else
            return new SimpleAudioRecorderImplementation();
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException("This functionality is not implemented in the .NET standard version of this assembly. Reference the NuGet package from your platform-specific (head) application project in order to reference the platform-specific implementation.");
        }
    }
}
