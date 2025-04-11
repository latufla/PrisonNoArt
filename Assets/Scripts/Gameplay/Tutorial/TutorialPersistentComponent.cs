using Honeylab.Utils.Persistence;
using System;


namespace Honeylab.Persistence
{
    [Serializable]
    public class TutorialPersistentComponent : PersistentComponent
    {
        public int Step { get; set; }
        public int Progress { get; set; }
    }
}
