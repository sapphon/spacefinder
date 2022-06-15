using Model;

namespace View
{
    public interface IShipSelectionObserver
    {
        void ShipSelectionChanged(Ship newSelection);
    }
}