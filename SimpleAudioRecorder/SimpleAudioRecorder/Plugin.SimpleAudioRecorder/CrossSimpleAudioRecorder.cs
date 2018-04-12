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
        static Lazy<ISimpleAudioRecorder> Implementation = new Lazy<ISimpleAudioRecorder>(() => CreateSimpleAudioRecorder(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Current settings to use
        /// </summary>
        public static ISimpleAudioRecorder Current
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