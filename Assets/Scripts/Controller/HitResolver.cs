using System;
using System.Collections.Generic;
using System.Linq;
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
                Util.logIfDebugging("Ship " + _solution.target.displayName + "'s DT negated damage totally.");


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
            Util.logIfDebugging("Ship " + _solution.target.displayName + " lost " + amount +
                                " hull points from the attack.");

            handleLowHullPoints();
        }

        private void handleLowHullPoints()
        {
            if (_solution.target.hitPoints < -_solution.target.hitPointsTotal)
            {
                Util.logIfDebugging("Ship " + _solution.target.displayName + " was destroyed by loss of hull points.");
            }
            else if (_solution.target.hitPoints <= 0)
            {
                Util.logIfDebugging("Ship " + _solution.target.displayName + " was crippled by loss of hull points.");
            }
        }

        private void handleCriticalHits(int amount)
        {
            int criticalThreshold = _solution.target.hitPointsTotal / 5;
            int stepsIncurredBeforeDamage =
                (_solution.target.hitPointsTotal - _solution.target.hitPoints) / criticalThreshold;
            int stepsAfterDamage = (_solution.target.hitPointsTotal - _solution.target.hitPoints + amount) /
                                   criticalThreshold;
            Util.logIfDebugging("Before: " + stepsIncurredBeforeDamage + " steps; after: " + stepsAfterDamage);
            if (stepsAfterDamage > stepsIncurredBeforeDamage)
            {
                Util.logIfDebugging("Ship " + _solution.target.displayName + " dealt " +
                                    (stepsAfterDamage - stepsIncurredBeforeDamage) + " critical hits.");

                dealCriticalHits(stepsAfterDamage - stepsIncurredBeforeDamage);
            }
        }

        private void dealCriticalHits(int howMany)
        {
            for (int i = 0; i < howMany; i++)
            {
                dealCriticalHit(_solution.target);
            }
        }

        private void dealCriticalHit(Ship ship)
        {
            int systemAffectedRoll = _random.rollAndTotal(1, Die.D100);
            Util.logIfDebugging("Crit table roll: " + systemAffectedRoll);

            bool cascade = false;
            if (systemAffectedRoll <= 10)
            {
                if (ship.lifeSupport == SystemCondition.Wrecked)
                {
                    cascade = true;
                    Util.logIfDebugging("Life support already wrecked; critical hit cascading");
                }
                else
                {
                    ship.lifeSupport++;
                    Util.logIfDebugging("Life support damaged by critical, status: " + ship.lifeSupport);

                    return;
                }
            }

            if (cascade || systemAffectedRoll <= 30)
            {
                if (ship.sensors == SystemCondition.Wrecked)
                {
                    cascade = true;
                    Util.logIfDebugging("Sensors already wrecked; critical hit cascading");
                }
                else
                {
                    ship.sensors++;
                    Util.logIfDebugging("Sensors damaged by critical, status: " + ship.sensors);
                    return;
                }
            }

            if (cascade || systemAffectedRoll <= 60)
            {
                if (ship.isWeaponsSystemWrecked())
                {
                    cascade = true;
                    Util.logIfDebugging("All weapons already wrecked; critical hit cascading");
                }
                else
                {
                    SystemCondition[] allWeaponsSystemConditions = ship.getAllWeaponsSystemConditions();
                    List<WeaponFiringArc> damageables = new List<WeaponFiringArc>();
                    for (int i = 0; i < 4; i++)
                    {
                        if (allWeaponsSystemConditions[i] != SystemCondition.Wrecked)
                        {
                            damageables.Add((WeaponFiringArc) i);
                        }
                    }

                    WeaponFiringArc damaged = Util.chooseOneRandomlyFrom(damageables.ToArray(), _random);
                    switch (damaged)
                    {
                        case WeaponFiringArc.Fore:
                            ship.foreWeapons++;
                            Util.logIfDebugging("Fore weapons damaged by critical, status: " + ship.foreWeapons);
                            break;
                        case WeaponFiringArc.Aft:
                            ship.aftWeapons++;
                            Util.logIfDebugging("Aft weapons damaged by critical, status: " + ship.aftWeapons);
                            break;
                        case WeaponFiringArc.Starboard:
                            ship.starboardWeapons++;
                            Util.logIfDebugging("Starboard weapons damaged by critical, status: " + ship.starboardWeapons);
                            break;
                        case WeaponFiringArc.Port:
                            ship.portWeapons++;
                            Util.logIfDebugging("Port weapons damaged by critical, status: " + ship.portWeapons);
                            break;
                    }
                }
            }

            if (cascade || systemAffectedRoll <= 80)
            {
                if (ship.engines == SystemCondition.Wrecked)
                {
                    cascade = true;
                    Util.logIfDebugging("Engines already wrecked; critical hit cascading");
                }
                else
                {
                    ship.engines++;
                    Util.logIfDebugging("Engines damaged by critical, status: " + ship.engines);
                    return;
                }
            }

            if (cascade || systemAffectedRoll <= 100)
            {
                if (ship.powerCore == SystemCondition.Wrecked)
                {
                    Util.logIfDebugging("Power core already wrecked; critical hit cascading");
                    DamageCrew(ship);
                }
                else
                {
                    ship.powerCore++;
                    Util.logIfDebugging("Power core damaged by critical, status: " + ship.powerCore);
                }
            }
        }

        private void DamageCrew(Ship ship)
        {
            Debug.Log("Crew damage not implemented");
        }

        private int rollDamage()
        {
            int damageRoll =
                _random.rollAndTotal(_solution.weapon.damageDieCount, _solution.weapon.damageDieType);
            Util.logIfDebugging(damageRoll + " damage rolled.");
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

            Util.logIfDebugging("Attack hit ship " + _solution.target.displayName + " in the " +
                                hitDirection.ToString() +
                                " side, according with an angle of incidence of " + angleBetween + ".");

            return hitDirection;
        }
    }
}