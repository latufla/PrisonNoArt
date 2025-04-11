namespace Honeylab.Utils.GameTime
{
    public interface IGamePauseService
    {
        void RetainPause(object retainer);
        void ReleasePause(object retainer);
    }
}
