using System;
using Controller.PhaseControllers;
using Model;
using UnityEngine;

namespace Controller
{
    public class HitResolver
    {
        private readonly FiringSolution _solution;
        private readonly DiceRoller _random;

        public HitResolver(FiringSolution solution, DiceRoller random)
        {
            _solution = solution;
            _random = random;
        }

        public void Resolve()
        {
            int totalDamage = rollDamage();
            WeaponFiringArc direction = determineHitDirection();
            int damageAfterShields = decrementRelevantShieldsAndReturnRemainder(totalDamage, direction);
            if (damageAfterShields > 0)
            {
                damageHull(damageAfterShields);
            }
        }

        private int decrementRelevantShieldsAndReturnRemainder(int damage, WeaponFiringArc direction)
        {
            if (direction == WeaponFiringArc.Fore)
            {
                int damageToShield = Math.Min(_solution.target.shieldForeCurrent, damage);
                _solution.target.shieldForeCurrent -= damageToShield;
                return damage - damageToShield;
            }

            return 0;
        }

        private void damageHull(int amount)
        {
            amount -= _solution.target.damageThreshold;
            if (amount <= 0)
            {
                if (Util.isGameDebugging())
                {
                    Debug.Log("Ship " + _solution.target.displayName + "'s DT negated damage totally.");
                }
                return;
            }
            else
            {
                handleCriticalHits(amount);
                deductHullPoints(amount);
            }
            
        }

        private void deductHullPoints(int amount)
        {
            _solution.target.hitPoints -= amount;
            if (Util.isGameDebugging())
            {
                Debug.Log("Ship " + _solution.target.displayName + " lost " + amount + "hull points from the attack.");
            }

            handleLowHullPoints();
        }

        private void handleLowHullPoints()
        {
            if (_solution.target.hitPoints < -_solution.target.hitPointsTotal)
            {
                if (Util.isGameDebugging())
                    Debug.Log("Ship " + _solution.target.displayName + " was destroyed by loss of hull points.");
            }
            else if (_solution.target.hitPoints <= 0)
            {
                if (Util.isGameDebugging())
                    Debug.Log("Ship " + _solution.target.displayName + " was crippled by loss of hull points.");
            }
        }

        private void handleCriticalHits(int amount)
        {
            int criticalThreshold = _solution.target.hitPointsTotal / 5;
            int stepsIncurredBeforeDamage =
                (_solution.target.hitPointsTotal - _solution.target.hitPoints) / criticalThreshold;
            int stepsAfterDamage = (_solution.target.hitPointsTotal - amount);
            if (stepsAfterDamage > stepsIncurredBeforeDamage)
            {
                if (Util.isGameDebugging())
                {
                    Debug.Log("Ship " + _solution.target.displayName + " dealt " + (stepsAfterDamage - stepsIncurredBeforeDamage) + " critical hits.");   
                }
                dealCriticalHits(stepsAfterDamage - stepsIncurredBeforeDamage);
            }
        }

        private void dealCriticalHits(int howMany)
        {
            for (int i = 0; i < howMany; i++)
            {
                Debug.Log("Critical hits not implemented.");
            }
        }

        private int rollDamage()
        {
            int damageRoll =
                _random.rollAndTotal(_solution.weapon.damageDieCount, _solution.weapon.damageDieType);
            if (Util.isGameDebugging()) 
                Debug.Log(damageRoll + " damage rolled.");
            return damageRoll;
        }

        private WeaponFiringArc determineHitDirection()
        {
            return WeaponFiringArc.Fore;
        }
        
        
    }
}