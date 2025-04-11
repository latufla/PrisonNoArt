using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Honeylab.Utils
{

    public class PlaceHeap<T> : MonoBehaviour where T : MonoBehaviour
    {
        private List<PlaceHeapItem<T>> _places = new List<PlaceHeapItem<T>>();

        public IReadOnlyList<PlaceHeapItem<T>> GetPlaces() => _places;
        public bool HasFreePlace() => _places.Any(it => it.Occupier == null);
        public bool HasOccupiedPlace() => _places.Any(it => it.Occupier != null);
        public int GetFreePlaceCount() => _places.Count(it => it.Occupier == null);
        public int GetOccupiedPlaceCount() => _places.Count(it => it.Occupier != null);

        private void Awake()
        {
            _places = GetComponentsInChildren<PlaceHeapItem<T>>().ToList();
        }
        public Transform OccupyClosestPlace(Vector3 position, T occupier)
        {
            var it = GetClosestFreePlace(position);
            if (it == null)
            {
                return null;
            }

            it.Occupier = occupier;
            it.OccupationTime = Time.time;
            return it.Place;
        }


        public void FreePlace(Transform place)
        {
            var it = _places.FirstOrDefault(it => it.Place == place);
            if (it == null)
            {
                return;
            }

            it.Occupier = null;
            it.OccupationTime = 0.0f;
        }


        public T FreeLastPlace()
        {
            var it = _places.LastOrDefault(it => it.Occupier != null);
            if (it == null)
            {
                return null;
            }

            T occupier = it.Occupier;
            it.Occupier = null;
            it.OccupationTime = 0.0f;
            return occupier;
        }


        public void FreePlace(T occupier)
        {
            var it = _places.FirstOrDefault(it => it.Occupier == occupier);
            if (it == null)
            {
                return;
            }

            it.Occupier = null;
            it.OccupationTime = 0.0f;
        }


        public void FreeAllPlaces()
        {
            _places.ForEach(it => it.Occupier = null);
        }


        public Transform OccupyRandomPlace(T occupier)
        {
            var places = GetFreePlaces();
            if (places.Count == 0)
            {
                return null;
            }

            var place = places[Random.Range(0, places.Count)];
            place.Occupier = occupier;
            place.OccupationTime = Time.time;
            return place.Place;
        }


        public Transform OccupyPlace(T occupier)
        {
            var places = GetFreePlaces();
            if (places.Count == 0)
            {
                return null;
            }

            var place = places.First();
            place.Occupier = occupier;
            place.OccupationTime = Time.time;
            return place.Place;
        }

        public Transform OccupySpecificPlace(T occupier, Transform specificPlace)
        {
            var placeItem = _places.FirstOrDefault(it => it.Place == specificPlace && it.Occupier == null);

            if (placeItem != null)
            {
                placeItem.Occupier = occupier;
                placeItem.OccupationTime = Time.time;
                return placeItem.Place;
            }

            return null;
        }

        public bool IsPlaceOccupied(Transform place)
        {
            var placeItem = _places.FirstOrDefault(it => it.Place == place);
            return placeItem != null && placeItem.Occupier != null;
        }

        private PlaceHeapItem<T> GetClosestFreePlace(Vector3 position)
        {
            var places = GetFreePlaces();
            return places.Count == 0 ? null : FindClosestPlace(places, position);
        }

        private static PlaceHeapItem<T> FindClosestPlace(List<PlaceHeapItem<T>> places, Vector3 position)
        {
            float minDistance = float.MaxValue;
            PlaceHeapItem<T> closestPlace = null;
            foreach (var it in places)
            {
                float distance = Vector3.Distance(position, it.Place.position);
                if (closestPlace == null || distance < minDistance)
                {
                    closestPlace = it;
                    minDistance = distance;
                }
            }

            return closestPlace;
        }

        public List<PlaceHeapItem<T>> GetFreePlaces() => _places.Where(it => it.Occupier == null).ToList();
        public List<PlaceHeapItem<T>> GetOccupiedPlaces() => _places.Where(it => it.Occupier != null).ToList();
    }
}
