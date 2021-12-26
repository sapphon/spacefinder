using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;
using UnityEngine.UI;

public class PhasePanelUI : MonoBehaviour
{
    private Text _phaseNameText;
    private PhaseManager _phaseManager;
    private Button _endPhaseButton;
    private Text _phaseRolesText;
    private ShipUIManager _shipsUI;

    void Awake()
    {
        this._shipsUI = FindObjectOfType<ShipUIManager>();
        this._phaseManager = FindObjectOfType<PhaseManager>();
        this._phaseNameText = transform.Find("CurrentPhaseReadout").GetComponent<Text>();
        this._phaseRolesText = transform.parent.Find("PhaseAvailableRolesPanel").Find("ActiveRolesReadout")
            .GetComponent<Text>();
        _endPhaseButton = this.transform.Find("AdvancePhaseButton").GetComponent<Button>();
        _endPhaseButton.onClick.AddListener(TryAdvancePhase);
    }

    void Update()
    {
        _phaseNameText.text = _phaseManager.GetCurrentPhase().ToString();
        SetActiveCharactersOrRoles();
        _endPhaseButton.interactable = _phaseManager.CanPhaseEnd();
    }

    private void SetActiveCharactersOrRoles()
    {
        List<Crew.Role> activePhaseRoles = _phaseManager.GetActivePhaseRoles();
        Ship selectedShip = this._shipsUI.GetSelectedShip();
        if (selectedShip != null && selectedShip.crew.Count > 0)
        {
            _phaseRolesText.text = String.Join("\r\n",
                selectedShip.crew.Where(crewperson => activePhaseRoles.Contains(crewperson.role))
                    .Select(crewperson => crewperson.name));
        }
        else
        {   
            _phaseRolesText.text = String.Join("\r\n", activePhaseRoles.Select(role => role.ToString()));
        }
    }

    public void TryAdvancePhase()
    {
        bool phaseAdvanced = _phaseManager.TryAdvancePhase();
        if (!phaseAdvanced)
        {
            Debug.Log("Phase not advanced; not all ships ready");
        }
    }
}
