using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionActionsPanelUI : MonoBehaviour
{
    private Text _actionsText;
    private ShipUIManager _shipsUiManager;
    private PhaseManager _phaseManager;
    private Slider _phaseFinishedSignal;
    private Text _phaseFinishedText;
    private Color _cautionOrange;
    private Button[] _actionButtons;
    private Text _notYourTurnText;
    private HelmPhaseController _helmPhaseController;

    void Awake()
    {
        this._phaseManager = FindObjectOfType<PhaseManager>();
        this._shipsUiManager = FindObjectOfType<ShipUIManager>();
        this._actionsText = transform.Find("PhaseNameReadout").GetComponent<Text>();
        this._phaseFinishedSignal = transform.Find("EndPhaseOKSlider").GetComponent<Slider>();
        this._phaseFinishedSignal.onValueChanged.AddListener(ToggleDone);
        this._phaseFinishedText = transform.Find("EndPhaseOKReadout").GetComponent<Text>();
        this._cautionOrange = new Color(.886f, .435f, .02f);
        this._helmPhaseController = FindObjectOfType<HelmPhaseController>();
        this._notYourTurnText = transform.Find("NotYourTurnText").GetComponent<Text>();
        initializeActionButtons();
    }

    private void initializeActionButtons()
    {
        _actionButtons = new Button[1];
        List<string> possibleActions = _helmPhaseController.GetPossibleActionNamesForPhase();
        for (int n = 0; n < possibleActions.Count; n++)
        {
            string actionName = possibleActions[n];
            _actionButtons[n] = transform.Find("ActionButton" + (n + 1)).GetComponent<Button>();
            _actionButtons[n].GetComponentInChildren<Text>().text = actionName;
            _actionButtons[n].onClick.AddListener(() =>
            {
                _helmPhaseController.ToggleShipAction(getSelectedShip(), actionName);
            });
        }
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
                _phaseFinishedText.color = new Color(50, 50, 50);
                _phaseFinishedSignal.colors = ColorBlock.defaultColorBlock;
                _phaseFinishedSignal.SetValueWithoutNotify(0);
            }

            if (_phaseManager.GetCurrentPhase() == Phase.Engineering || _phaseManager.ShipHasInitiative(selectedShip))
            {
                updateActionButtons(selectedShip);
                _notYourTurnText.gameObject.SetActive(false);
            }
            else
            {
                hideActionButtons();
                _notYourTurnText.gameObject.SetActive(true);
            }

            GrowPanel();
        }
        else
        {
            ShrinkPanel();
        }
    }

    private void hideActionButtons()
    {
        foreach (var button in _actionButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    private void updateActionButtons(Ship actor)
    {
        List<string> possibleActions = _helmPhaseController.GetPossibleActionNamesForPhase();
        for (int n = 0; n < possibleActions.Count; n++)
        {
            colorButtonByActivity(actor, possibleActions[n], _actionButtons[n]);
        }
    }

    private void colorButtonByActivity(Ship actor, string buttonActionName, Button button, bool buttonEnabled = true)
    {
        CrewAction shipAction = _helmPhaseController.getShipAction(actor);
        ColorBlock block = ColorBlock.defaultColorBlock;

        if (shipAction != null && shipAction.name.Equals(buttonActionName))
        {
            block.normalColor = Color.cyan;
            block.highlightedColor = Color.cyan;
            block.selectedColor = Color.cyan;
        }

        button.gameObject.SetActive(true);
        button.colors = block;
    }

    private void GrowPanel()
    {
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 175f);
        _actionsText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 160f);
        _phaseFinishedSignal.GetComponent<RectTransform>()
            .SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 45f);
        _phaseFinishedSignal.gameObject.transform.Find("Handle Slide Area/Handle").GetComponent<RectTransform>()
            .SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 20f);
    }

    protected void ShrinkPanel()
    {
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
        _actionsText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
        _phaseFinishedSignal.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
        _phaseFinishedSignal.gameObject.transform.Find("Handle Slide Area/Handle").GetComponent<RectTransform>()
            .SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
    }
}