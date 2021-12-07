using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhasePanelUI : MonoBehaviour
{
    private Text _phaseNameText;
    private PhaseManager _phaseManager;
    private Button _endPhaseButton;

    void Awake()
    {
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
            Debug.Log("Phase not advanced; not all ships ready");
        }
    }
}
