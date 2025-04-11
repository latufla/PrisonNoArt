using System.Collections.Generic;


namespace Honeylab.Utils
{
    public interface IPlaceQueueItemData
    {
        bool IsEmpty();
    }


    public class PlaceQueueItem<T> where T : IPlaceQueueItemData
    {
        public int Index;
        public T Data;


        public bool IsEmpty() => Data.IsEmpty();
    }


    public class PlaceQueue<T> where T : IPlaceQueueItemData, new()
    {
        private int _capacity;
        private readonly List<PlaceQueueItem<T>> _items;


        public int Count => _items.Count;


        public int Capacity
        {
            get => _capacity;
            set => _capacity = value;
        }


        public PlaceQueue(int capacity)
        {
            Capacity = capacity;
            _items = new List<PlaceQueueItem<T>>();
        }


        public PlaceQueueItem<T> GetOrEnqueueEmpty() => GetEmpty() ?? EnqueueEmpty();


        public PlaceQueueItem<T> GetEmpty()
        {
            foreach (var it in _items)
            {
                if (it.IsEmpty())
                {
                    return it;
                }
            }

            return null;
        }


        public PlaceQueueItem<T> GetFirst() => _items.Count > 0 ? _items[0] : null;
        public IReadOnlyList<PlaceQueueItem<T>> GetAll() => _items;


        public PlaceQueueItem<T> Dequeue()
        {
            if (_items.Count == 0)
            {
                return null;
            }

            var item = _items[0];
            _items.RemoveAt(0);
            _items.ForEach(it => it.Index--);
            return item;
        }


        public PlaceQueueItem<T> EnqueueEmpty()
        {
            int count = _items.Count;
            if (count == Capacity)
            {
                return null;
            }

            var item = new PlaceQueueItem<T>
            {
                Index = count,
                Data = new T()
            };
            _items.Add(item);
            return item;
        }
    }
}
