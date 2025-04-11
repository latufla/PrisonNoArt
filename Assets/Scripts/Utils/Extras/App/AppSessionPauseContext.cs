using System;


namespace Honeylab.Utils.App
{
    internal readonly struct AppSessionPauseContext
    {
        public readonly float GameTime;
        public readonly DateTimeOffset DateTimeOffsetUtc;


        public AppSessionPauseContext(float gameTime, DateTimeOffset dateTimeOffsetUtc)
        {
            GameTime = gameTime;
            DateTimeOffsetUtc = dateTimeOffsetUtc;
        }
    }
}
