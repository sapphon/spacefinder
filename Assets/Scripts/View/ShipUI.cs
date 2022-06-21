using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Controller.PhaseControllers;
using Model;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShipUI : MonoBehaviour
{

    public Ship shipToTrack;
    protected SpriteRenderer spriteRenderer;
    protected Tilemap shipMap;
    protected ShipUIManager shipUiManager;
    protected HelmPhaseController helmPhaseController;
    protected GunneryPhaseController gunneryPhaseController;
    private TextMesh movesUntilTurnReadout;
    private TextMesh movesLeftReadout;
    private PhaseManager phaseManager;
    protected SpriteRenderer advanceIndicator;
    private SpriteRenderer starboardTurnIndicator;
    private SpriteRenderer portTurnIndicator;
    private SpriteRenderer targetedIndicator;
    private SpriteRenderer aiControlledIndicator;
    private GameObject _firingArcUI;
    protected GameObject maneuverUI;
    private GameObject targetingUI;

    void Awake()
    {
        this.shipMap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.maneuverUI = this.transform.Find("ManeuverUI").gameObject;
        this.targetingUI = this.transform.Find("TargetingUI").gameObject;
        this.advanceIndicator = this.maneuverUI.transform.Find("AdvanceIndicator").GetComponent<SpriteRenderer>();
        this.targetedIndicator = this.targetingUI.transform.Find("TargetedIndicator").GetComponent<SpriteRenderer>();
        this.starboardTurnIndicator = this.maneuverUI.transform.Find("StarboardTurnIndicator").GetComponent<SpriteRenderer>();
        this.portTurnIndicator = this.maneuverUI.transform.Find("PortTurnIndicator").GetComponent<SpriteRenderer>();
        this.movesUntilTurnReadout = this.maneuverUI.transform.Find("MovesUntilTurnReadout").GetComponent<TextMesh>();
        this.movesLeftReadout = this.maneuverUI.transform.Find("MovesRemainingReadout").GetComponent<TextMesh>();
        this.aiControlledIndicator = this.transform.Find("ArtificialIntelligenceUI").gameObject.transform.Find("AIControlledIndicator").GetComponent<SpriteRenderer>();
        this.shipUiManager = FindObjectOfType<ShipUIManager>();
        this.helmPhaseController = FindObjectOfType<HelmPhaseController>();
        this.gunneryPhaseController = FindObjectOfType<GunneryPhaseController>();
        this._firingArcUI = this.transform.Find("FiringArcUI").gameObject;
        this.phaseManager = FindObjectOfType<PhaseManager>();
    }

    void Update()
    {
        Transform transform = this.transform;
        transform.position = shipMap.CellToWorld(shipToTrack.gridPosition);
        transform.rotation = Quaternion.AngleAxis((int)shipToTrack.facing, Vector3.forward);
        spriteRenderer.color = shipToTrack.hitPoints < 1? Color.grey : shipToTrack.affiliation == Affiliation.Player ? Color.green : Color.red;
        spriteRenderer.sortingOrder = 10;
        EnableManeuverUIIfManeuvering();
        FiringArcs();
        TargetingCrosshair();
        AIIndicator();
    }

    private void AIIndicator()
    {
        this.aiControlledIndicator.enabled = this.shipToTrack.isArtificiallyIntelligentlyControlled;
    }

    private void EnableManeuverUIIfManeuvering()
    {
        
        if (phaseManager.getShipActionsThisPhase(this.shipToTrack).Any(action => action.actionType.isMovement()))
        {
            this.maneuverUI.SetActive(true);
            SetAdvanceUIColor();
            SetTurnUIColor();
            SetMovesRemaining();
            SetMovesUntilTurn();
        }
        else
        {
            this.maneuverUI.SetActive(false);
        }
    }

    private void TargetingCrosshair()
    {
        bool shouldAppear = phaseManager.GetCurrentPhase() == Phase.Gunnery && this.gunneryPhaseController.IsTargeted(this.shipToTrack);
        this.targetedIndicator.gameObject.SetActive(shouldAppear);
    }

    private void SetMovesUntilTurn()
    {
        int movesUntilNextTurn = helmPhaseController.MovesUntilNextTurn(shipToTrack);
        this.movesUntilTurnReadout.text = movesUntilNextTurn + " Mv B4 Turn";
        this.movesUntilTurnReadout.color = movesUntilNextTurn == 0 ? Color.green : Color.red;
    }

    private void SetMovesRemaining()
    {
        int movesRemaining = helmPhaseController.MovesRemaining(shipToTrack);
        this.movesLeftReadout.text = movesRemaining + "Mv Left";
        this.movesLeftReadout.color = movesRemaining > 0 ? Color.green : Color.red;
    }

    private void FiringArcs()
    {
        if (isSelected() && shipUiManager.GetShowingArcs())
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
