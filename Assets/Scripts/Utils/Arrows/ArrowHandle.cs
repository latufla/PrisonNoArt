using System.Collections.Generic;


namespace Honeylab.Utils.Arrows
{
    public class ArrowHandle : IArrowHandle
    {
        private static readonly Stack<ArrowHandle> Pool = new Stack<ArrowHandle>();


        public ArrowView Arrow { get; private set; }


        public static ArrowHandle Retain(ArrowView arrow)
        {
            if (Pool.Count == 0)
            {
                return new ArrowHandle { Arrow = arrow };
            }

            ArrowHandle handle = Pool.Pop();
            handle.Arrow = arrow;
            return handle;
        }


        public static void Release(ArrowHandle handle)
        {
            handle.Arrow = null;
            Pool.Push(handle);
        }
    }
}
