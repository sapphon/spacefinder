using Model;

namespace Controller.PhaseControllers
{
    public interface IPhaseController
    {
        void OnPhaseBegin();
        void OnPhaseEnd();
        void OnActionBegin(CrewAction action, Ship ship);
        void OnActionEnd(CrewAction action, Ship ship);
        void OnActionCancel(CrewAction action, Ship ship);
    }
}