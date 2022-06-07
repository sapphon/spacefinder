using UnityEngine;

namespace Controller
{
    public class RoundManager : MonoBehaviour
    {
        protected int currentRound = 1;

        public int GetCurrentRound()
        {
            return currentRound;
        }

        public bool TryAdvanceRound(PhaseManager phaseManager)
        {
            bool mayAdvance = phaseManager.CanPhaseEnd() && phaseManager.IsLastPhase();
            if (mayAdvance)
            {
                EndRound();
            }

            return mayAdvance;
        }

        protected void EndRound()
        {
            currentRound++;
        }
    }
}
