using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Model;
using Model.Crew;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using View;
using Action = Controller.Action;

public class SelectionActionsPanelUI : MonoBehaviour
{
    private Text _actionsText;
    private ShipUIManager _shipsUiManager;
    private PhaseManager _phaseManager;
 
    private Button[] _actionButtons;
    private Text _notYourTurnText;
    private CrewPanelUI _crewUI;

    void Awake()
    {
        this._phaseManager = FindObjectOfType<PhaseManager>();
        this._shipsUiManager = FindObjectOfType<ShipUIManager>();
        this._actionsText = transform.Find("PhaseNameReadout").GetComponent<Text>();
        this._notYourTurnText = transform.Find("NotYourTurnText").GetComponent<Text>();
        this._crewUI = FindObjectOfType<CrewPanelUI>();
        initializeActionButtons();
    }

    private void initializeActionButtons()
    {
        _actionButtons = new Button[12];
        for (int n = 0; n < _actionButtons.Length; n++)
        {
            _actionButtons[n] = transform.Find("ActionButton" + (n + 1)).GetComponent<Button>();
        }
    }

    private void updateActionButtonTextAndOnclick(Button button, String actionName)
    {
        button.gameObject.SetActive(true);
        button.GetComponentInChildren<Text>().text = actionName;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            _phaseManager.ToggleShipAction(getSelectedShip(), getSelectedCrewperson(), actionName );
        });
    }

    private CrewMember getSelectedCrewperson()
    {
        return _crewUI.getSelectedCrewmemberForShip(getSelectedShip());
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

            if (_phaseManager.DoesCurrentPhaseUseInitiative() && !_phaseManager.ShipHasInitiative(selectedShip))
            {
                
                hideActionButtons();
                setErrorText("NOT THIS SHIP'S TURN");
            }
            else if (getSelectedCrewperson() != null)
            {
                updateActionButtons(getSelectedCrewperson());
                clearErrorText();
            }
            else
            {
                hideActionButtons();
                setErrorText("SELECT A CREWPERSON");
            }

            GrowPanel();
        }
        else
        {
            ShrinkPanel();
        }
    }

    private void setErrorText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            _notYourTurnText.gameObject.SetActive(false);
        }
        else
        {
            _notYourTurnText.text = text;
            _notYourTurnText.gameObject.SetActive(true);
        }
    }

    private void clearErrorText()
    {
        this.setErrorText(null);
    }

    private void hideActionButtons()
    {
        foreach (var button in _actionButtons)
        {
             button.gameObject.SetActive(false);
        }
    }

    private void updateActionButtons(CrewMember actor)
    {
        List<Action> possibleActions = _phaseManager.getPossibleActionsForSelectedCrewpersonThisPhase();
        for (int n = 0; n < possibleActions.Count; n++)
        {
            updateActionButtonTextAndOnclick(_actionButtons[n], possibleActions[n].name);
            colorButtonByActivity(_actionButtons[n]);
        }

        for (int m = possibleActions.Count; m < 12; m++)
        {
            _actionButtons[m].gameObject.SetActive(false);
        }
    }

    private void colorButtonByActivity(Button button)
    {
        List<CrewAction> crewpersonActionsThisPhase = _phaseManager.getCrewpersonActionsThisPhase(this.getSelectedCrewperson());
        String buttonText = button.GetComponentInChildren<Text>().text;
        ColorBlock block = ColorBlock.defaultColorBlock;

        if (crewpersonActionsThisPhase.Count > 0 && crewpersonActionsThisPhase.Any(action => action.actionType.name == buttonText))
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
    }

    protected void ShrinkPanel()
    {
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
        _actionsText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
    }
}