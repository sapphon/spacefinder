using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionActionsPanelUI : MonoBehaviour
{
    private Text _actionsText;
    private Button _firstActionButton;
    private ShipUIManager _shipsUiManager;
    private PhaseManager _phaseManager;

    void Awake()
    {
        this._phaseManager = FindObjectOfType<PhaseManager>();
        this._shipsUiManager = FindObjectOfType<ShipUIManager>();
        //this._firstActionButton = transform.Find("FirstActionButton").GetComponent<Button>();
        this._actionsText = transform.Find("PhaseNameReadout").GetComponent<Text>();
    }

    void Update()
    {
        Ship selectedShip = _shipsUiManager.GetSelectedShip();
        if (selectedShip != null)
        {
            _actionsText.text = _phaseManager.GetCurrentPhase().ToString() + " Actions";
//            _firstActionButton.text = selectedShip.displayName;
            GrowPanel();
        }
        else
        {
            ShrinkPanel();
        }
    }

    private void GrowPanel()
    {
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 175f);
        _actionsText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 160f);
    }

    protected void ShrinkPanel()
    {
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
        _actionsText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
    }
}
