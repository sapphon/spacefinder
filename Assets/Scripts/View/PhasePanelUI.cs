using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Model;
using Model.Crew;
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
        _endPhaseButton = this.transform.Find("AdvancePhaseButton").GetComponent<Button>();
        _endPhaseButton.onClick.AddListener(TryAdvancePhase);
    }

    void Update()
    {
        _phaseNameText.text = _phaseManager.GetCurrentPhase().ToString();
        _endPhaseButton.interactable = _phaseManager.CanPhaseEnd();
    }

    public void TryAdvancePhase()
    {
        bool phaseAdvanced = _phaseManager.TryAdvancePhase();
        if (!phaseAdvanced)
        {
            Util.logIfDebugging("Phase not advanced; not all ships ready");
        }
    }
}
