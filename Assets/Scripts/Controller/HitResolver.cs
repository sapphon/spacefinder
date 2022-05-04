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
            WeaponFiringArc direction = determineHitDirection();
            int totalDamage = rollDamage();
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
            else if (direction == WeaponFiringArc.Aft)
            {
                int damageToShield = Math.Min(_solution.target.shieldAftCurrent, damage);
                _solution.target.shieldAftCurrent -= damageToShield;
                return damage - damageToShield;
            }
            else if (direction == WeaponFiringArc.Port)
            {
                int damageToShield = Math.Min(_solution.target.shieldPortCurrent, damage);
                _solution.target.shieldPortCurrent -= damageToShield;
                return damage - damageToShield;
            }
            else
            {
                int damageToShield = Math.Min(_solution.target.shieldStarboardCurrent, damage);
                _solution.target.shieldStarboardCurrent -= damageToShield;
                return damage - damageToShield;
            }
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
            int stepsAfterDamage = (_solution.target.hitPointsTotal - _solution.target.hitPoints + amount) /
                                   criticalThreshold;
            if (stepsAfterDamage > stepsIncurredBeforeDamage)
            {
                if (Util.isGameDebugging())
                {
                    Debug.Log("Ship " + _solution.target.displayName + " dealt " +
                              (stepsAfterDamage - stepsIncurredBeforeDamage) + " critical hits.");
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
            WeaponFiringArc hitDirection;
            float angleBetween = Util.getAngleBetweenShips(_solution.target, _solution.attacker);
            if (Mathf.Abs(angleBetween) <= 30f || Mathf.Approximately(Mathf.Abs(angleBetween), 30f))
            {
                hitDirection = WeaponFiringArc.Fore;
            }
            else if (Mathf.Abs(angleBetween) >= 150f || Mathf.Approximately(Mathf.Abs(angleBetween), 150f))
            {
                hitDirection = WeaponFiringArc.Aft;
            }
            else if (angleBetween > 30f && angleBetween < 150f)
            {
                hitDirection = WeaponFiringArc.Port;
            }
            else
            {
                hitDirection = WeaponFiringArc.Starboard;
            }

            if (Util.isGameDebugging())
            {
                Debug.Log("Attack hit ship " + _solution.target.displayName + " in the " + hitDirection.ToString() +
                          " side, according with an angle of incidence of " + angleBetween + ".");
            }

            return hitDirection;
        }
    }
}