using System.Collections.Generic;
using UnityEngine;

namespace View
{
    public abstract class AShipSelectionObservable : MonoBehaviour, IShipSelectionObservable
    {
        protected List<IShipSelectionObserver> observers;

        public void AddObserver(IShipSelectionObserver toAdd)
        {
            createListIfNull();
            observers.Add(toAdd);
        }

        private void createListIfNull()
        {
            if(observers == null)
            observers = new List<IShipSelectionObserver>();
        }

        public void RemoveObserver(IShipSelectionObserver toRemove)
        {
            createListIfNull();
            if (observers.Contains(toRemove))
            {
                observers.Remove(toRemove);
            }
        }
    }

    public interface IShipSelectionObservable
    {
        void AddObserver(IShipSelectionObserver toAdd);
        void RemoveObserver(IShipSelectionObserver toRemove);
    }
}