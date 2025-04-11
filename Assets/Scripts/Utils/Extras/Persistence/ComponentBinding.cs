using System;


namespace Honeylab.Utils.Persistence
{
    [Serializable]
    internal class ComponentBinding
    {
        public string N { get; set; }
        public PersistentComponent C { get; set; }
    }
}
