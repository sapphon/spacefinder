using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShipUIManager : MonoBehaviour
{
    public GameObject shipUIPrefab;
    protected Tilemap shipMap;
    protected Ship selectedShip;
    protected bool showingArcs;
    private int showingRange;

    void Awake()
    {
        this.shipMap = GameObject.Find("Tilemap").GetComponent<Tilemap>();

        this.selectedShip = null;
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
            shipUIObject.name = (ship.displayName+"ShipUI");
            ShipUI ui = shipUIObject.GetComponent<ShipUI>();
            ui.shipToTrack = ship;
        }
    }

    public bool TrySelectShip(Vector3Int shipCoordinates)
    {
        Ship shipFound = ShipPresentAt(shipCoordinates);
        if (shipFound != null)
        {
            SelectShip(shipFound);
            return true;
        }

        DeselectShip();
        return false;
    }

    protected void DeselectShip()
    {
        this.selectedShip = null;
    }

    protected void SelectShip(Ship ship)
    {
        
            Vector3 worldCenterOfCell = shipMap.CellToWorld(ship.gridPosition);
            Camera.main.transform.position = new Vector3(worldCenterOfCell.x, worldCenterOfCell.y, Camera.main.transform.position.z);
            this.selectedShip = ship;
    }

    private Ship ShipPresentAt(Vector3Int tileCoordinates)
    {
        return FindObjectsOfType<Ship>().FirstOrDefault(ship => ship.gridPosition.Equals(tileCoordinates));
    }

    public Ship GetSelectedShip()
    {
        return this.selectedShip;
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
}
