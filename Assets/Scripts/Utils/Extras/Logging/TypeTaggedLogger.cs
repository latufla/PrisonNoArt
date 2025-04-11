using Debug = UnityEngine.Debug;
#if HONEYLAB_DISABLE_LOGGING
using System.Diagnostics;
#endif


namespace Honeylab.Utils.Logging
{
    public static class TypeTaggedLogger
    {
        #if HONEYLAB_DISABLE_LOGGING
        internal const string NeverDefinedSymbol = "__NEVER_DEFINED__";
        #endif


        #if HONEYLAB_DISABLE_LOGGING
        [Conditional("NeverDefinedSymbol")]
        #endif
        public static void SelfLog<T>(string message)
        {
            //Debug.Log(CreateTypeTaggedText<T>(message));
        }


        #if HONEYLAB_DISABLE_LOGGING
        [Conditional("NeverDefinedSymbol")]
        #endif
        public static void SelfLogWarning<T>(string message)
        {
            //Debug.LogWarning(CreateTypeTaggedText<T>(message));
        }


        #if HONEYLAB_DISABLE_LOGGING
        [Conditional("NeverDefinedSymbol")]
        #endif
        public static void SelfLogError<T>(string message)
        {
            //Debug.LogError(CreateTypeTaggedText<T>(message));
        }


        private static string CreateTypeTaggedText<T>(string message) => $"[{typeof(T).Name}] {message}";
    }
}
