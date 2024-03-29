﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Controller.PhaseControllers;
using JetBrains.Annotations;
using Model;
using UnityEngine;
using UnityEngine.Tilemaps;
using View;

public class ShipUIManager : AShipSelectionObservable
{
    public GameObject shipUIPrefab;
    public GameObject attackMarkerPrefab;
    protected Tilemap shipMap;
    protected Ship selectedShip;
    protected bool showingArcs;
    private int showingRange;
    protected CameraController cameraControlller;
    private GunneryPhaseController _gunneryPhaseController;
    protected Weapon selectedWeapon;
    private GameObject _attackMarkerParent;
    private CrewPanelUI _crewUI;


    void Awake()
    {
        this.shipMap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        this.cameraControlller = GameObject.FindObjectOfType<CameraController>();
        this._gunneryPhaseController = GameObject.FindObjectOfType<GunneryPhaseController>();
        this._crewUI = FindObjectOfType<CrewPanelUI>();
        this._attackMarkerParent = GameObject.Find("Attack Markers");

        this.selectedShip = null;
        this.selectedWeapon = null;
        this.showingArcs = false;
        this.showingRange = 0;
    }

    void Start()
    {
        createShipUIs();
    }

    private void createShipUIs()
    {
        Ship[] allShips = Ship.getAllShips();
        GameObject shipsParent = GameObject.Find("Ship Views");
        foreach (Ship ship in allShips)
        {
            GameObject shipUIObject = Instantiate(shipUIPrefab, shipsParent.transform);
            shipUIObject.name = (ship.displayName + "ShipUI");
            ShipUI ui = shipUIObject.GetComponent<ShipUI>();
            ui.shipToTrack = ship;
        }
    }

    public bool TrySelectShip(Vector3Int shipCoordinates)
    {
        DeselectShip();
        Ship shipFound = ShipPresentAt(shipCoordinates);
        if (shipFound != null)
        {
            SelectShip(shipFound);
            return true;
        }
        return false;
    }

    public bool TryTargetShip(Vector3Int tileCoordinates)
    {
        Ship shipFound = ShipPresentAt(tileCoordinates);
        if (shipFound != null && getSelectedWeapon() != null)
        {
            TargetShip(shipFound);
            return true;
        }

        return false;
    }

    private void TargetShip(Ship ship)
    {
        _gunneryPhaseController.TryTarget(this.GetSelectedShip(), ship, this.getSelectedWeapon(), _crewUI.getSelectedCrewmemberForShip(this.GetSelectedShip()));
    }

    public void DeselectShip()
    {
        this.selectedShip = null;
        this.selectedWeapon = null;
        this.showingArcs = false;
        this.showingRange = 0;
        this.NotifyObserversShipSelectionChanged(null);
    }

    private void NotifyObserversShipSelectionChanged(Ship newSelection)
    {
        foreach (var observer in this.observers)
        {
            observer.ShipSelectionChanged(newSelection);
        }
    }

    protected void SelectShip(Ship ship)
    {
        Vector3 worldCenterOfCell = shipMap.CellToWorld(ship.gridPosition);
        cameraControlller.setAimPoint(new Vector3(worldCenterOfCell.x, worldCenterOfCell.y));
        this.selectedShip = ship;
        NotifyObserversShipSelectionChanged(ship);
    }

    public bool TrySelectWeapon(int weaponOrdinal)
    {
        if (GetSelectedShip() != null)
        {
            if (GetSelectedShip().weapons.Count >= weaponOrdinal)
            {
                this.selectedWeapon = GetSelectedShip().weapons[weaponOrdinal - 1];
                Util.logIfDebugging("Weapon " + weaponOrdinal + " Selected: it's a " + this.selectedWeapon.name + ", arc: " + this.selectedWeapon.arc);
                return true;
            }
        }
        Util.logIfDebugging("Weapon selection failed.");
        return false;
    }

    public Ship ShipPresentAt(Vector3Int tileCoordinates)
    {
        return FindObjectsOfType<Ship>().FirstOrDefault(ship => ship.gridPosition.Equals(tileCoordinates));
    }

    public Ship GetSelectedShip()
    {
        return this.selectedShip;
    }

    public Weapon getSelectedWeapon()
    {
        return this.selectedWeapon;
    }

    public bool GetShowingArcs()
    {
        return this.showingArcs;
    }

    public void SetShowingArcs(bool toSet)
    {
        this.showingArcs = toSet;
    }

    public int GetShowingRange()
    {
        return this.showingRange;
    }

    public void SetShowingRange(int toSet)
    {
        this.showingRange = toSet;
    }

    public void UpdateAttackMarkers(HashSet<FiringSolutionStruct> solutions)

    {
        ClearAttackMarkers();
        foreach(FiringSolutionStruct solution in solutions)
        {
            GameObject attackMarkerObject = Instantiate(attackMarkerPrefab, this._attackMarkerParent.transform);
            LineRenderer attackMarker = attackMarkerObject.GetComponent<LineRenderer>();
            float dontChangeTheZ = attackMarker.GetPosition(0).z;
            Vector3 attackerPosition = solution.attacker.getWorldSpacePosition();
            Vector3 targetPosition = solution.target.getWorldSpacePosition();
            attackMarker.SetPositions(new[]{new Vector3(attackerPosition.x, attackerPosition.y, dontChangeTheZ), new Vector3(targetPosition.x, targetPosition.y, dontChangeTheZ)});
        }
    }

    private void ClearAttackMarkers()
    {
        while (this._attackMarkerParent.transform.childCount > 0)
        {
            DestroyImmediate(_attackMarkerParent.transform.GetChild(0).gameObject);
        }
    }
}