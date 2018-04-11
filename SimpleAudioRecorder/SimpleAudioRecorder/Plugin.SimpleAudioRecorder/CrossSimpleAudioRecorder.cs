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
    }
}
