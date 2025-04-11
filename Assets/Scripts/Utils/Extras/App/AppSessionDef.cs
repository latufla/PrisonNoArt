namespace Honeylab.Utils.App
{
    internal readonly struct AppSessionDef
    {
        public readonly int Number;
        public readonly float StartGameTime;


        public AppSessionDef(int number, float startGameTime)
        {
            Number = number;
            StartGameTime = startGameTime;
        }
    }
}
