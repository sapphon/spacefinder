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
    private Slider _phaseFinishedSignal;
    private Text _phaseFinishedText;
    private Color _cautionOrange;

    void Awake()
    {
        this._phaseManager = FindObjectOfType<PhaseManager>();
        this._shipsUiManager = FindObjectOfType<ShipUIManager>();
        this._actionsText = transform.Find("PhaseNameReadout").GetComponent<Text>();
        this._phaseFinishedSignal = transform.Find("EndPhaseOKSlider").GetComponent<Slider>();
        this._phaseFinishedSignal.onValueChanged.AddListener(ToggleDone);
        this._phaseFinishedText = transform.Find("EndPhaseOKReadout").GetComponent<Text>();
        this._cautionOrange = new Color(.886f,.435f,.02f);
    }

    void ToggleDone(float dontCare)
    {
        Ship selectedShip = getSelectedShip();
        if (_phaseManager.isShipDone(selectedShip))
        {
            _phaseManager.SignalStillWorking(selectedShip);
        }
        else
        {
            _phaseManager.SignalComplete(selectedShip);
        }
    }

    private Ship getSelectedShip()
    {
        return _shipsUiManager.GetSelectedShip();
    }

    void Update()
    {
        Ship selectedShip = getSelectedShip();
        if (selectedShip != null)
        {
            _actionsText.text = _phaseManager.GetCurrentPhase().ToString() + " Actions";
            if (_phaseManager.isShipDone(selectedShip))
            {
                _phaseFinishedText.text = "!!! DONE WITH PHASE !!!";
                _phaseFinishedText.color = _cautionOrange;
                ColorBlock colorBlock = ColorBlock.defaultColorBlock;
                colorBlock.normalColor = _cautionOrange;
                colorBlock.selectedColor = _cautionOrange;
                _phaseFinishedSignal.colors = colorBlock;
                _phaseFinishedSignal.SetValueWithoutNotify(1);
            }
            else
            {
                _phaseFinishedText.text = "Done w/ Phase?";
                _phaseFinishedText.color = new Color(50,50,50);
                _phaseFinishedSignal.colors = ColorBlock.defaultColorBlock;
                _phaseFinishedSignal.SetValueWithoutNotify(0);
            }

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
