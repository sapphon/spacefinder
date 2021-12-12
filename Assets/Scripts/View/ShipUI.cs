using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShipUI : MonoBehaviour
{

    public Ship shipToTrack;
    protected SpriteRenderer spriteRenderer;
    protected Tilemap shipMap;
    protected SpriteRenderer advanceIndicator;
    protected ShipUIManager shipUiManager;
    protected HelmPhaseController helmPhaseController;
    protected GameObject maneuverUI;
    private SpriteRenderer starboardTurnIndicator;
    private SpriteRenderer portTurnIndicator;
    private GameObject _firingArcUI;

    void Awake()
    {
        this.shipMap = FindObjectOfType<Tilemap>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.maneuverUI = this.transform.Find("ManeuverUI").gameObject;
        this.advanceIndicator = this.transform.Find("ManeuverUI").transform.Find("AdvanceIndicator").GetComponent<SpriteRenderer>();
        this.starboardTurnIndicator = this.transform.Find("ManeuverUI").transform.Find("StarboardTurnIndicator").GetComponent<SpriteRenderer>();
        this.portTurnIndicator = this.transform.Find("ManeuverUI").transform.Find("PortTurnIndicator").GetComponent<SpriteRenderer>();
        this.shipUiManager = FindObjectOfType<ShipUIManager>();
        this.helmPhaseController = FindObjectOfType<HelmPhaseController>();
        this._firingArcUI = this.transform.Find("FiringArcUI").gameObject;
    }

    void Update()
    {
        Transform transform = this.transform;
        transform.position = shipMap.CellToWorld(shipToTrack.gridPosition);
        transform.rotation = Quaternion.AngleAxis((int)shipToTrack.facing, Vector3.forward);
        spriteRenderer.color = shipToTrack.affiliation == Affiliation.Player ? Color.green : Color.red;
        EnableManeuverUIIfManeuvering();
        FiringArcs();
    }

    private void EnableManeuverUIIfManeuvering()
    {
        if (helmPhaseController.IsShipCurrentlyActing(shipToTrack) &&
            helmPhaseController.getShipAction(shipToTrack).name == "Maneuver")
        {
            this.maneuverUI.SetActive(true);
            SetAdvanceUIColor();
            SetTurnUIColor();
        }
        else
        {
            this.maneuverUI.SetActive(false);
        }
    }

    private void FiringArcs()
    {
        if (isSelected() && shipUiManager.ShowFiringArcs())
        {
            this._firingArcUI.SetActive(true);
        }
        else
        {
            this._firingArcUI.SetActive(false);
        }
    }

    private void SetAdvanceUIColor()
    {
        this.advanceIndicator.color = helmPhaseController.MayAdvance(shipToTrack) ? Color.green : Color.red;
    }
    
    private void SetTurnUIColor()
    {
        var turnColor = helmPhaseController.MayTurn(shipToTrack) ? Color.green : Color.red;
        this.portTurnIndicator.color = turnColor;
        this.starboardTurnIndicator.color = turnColor;
    }

    private bool isSelected()
    {
        return shipUiManager.GetSelectedShip() == this.shipToTrack;
    }

}
