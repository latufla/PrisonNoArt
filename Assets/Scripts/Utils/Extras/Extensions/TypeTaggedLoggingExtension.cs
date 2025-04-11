using Honeylab.Utils.Logging;
#if HONEYLAB_DISABLE_LOGGING
using System.Diagnostics;
#endif


namespace Honeylab.Utils.Extensions
{
    public static class TypeTaggedLoggingExtension
    {
        #if HONEYLAB_DISABLE_LOGGING
        [Conditional(TypeTaggedLogger.NeverDefinedSymbol)]
        #endif
        public static void SelfLog<T>(this T _, string message) => TypeTaggedLogger.SelfLog<T>(message);


        #if HONEYLAB_DISABLE_LOGGING
        [Conditional(TypeTaggedLogger.NeverDefinedSymbol)]
        #endif
        public static void SelfLogWarning<T>(this T _, string message) => TypeTaggedLogger.SelfLogWarning<T>(message);


        #if HONEYLAB_DISABLE_LOGGING
        [Conditional(TypeTaggedLogger.NeverDefinedSymbol)]
        #endif
        public static void SelfLogError<T>(this T _, string message) => TypeTaggedLogger.SelfLogError<T>(message);
    }
}
