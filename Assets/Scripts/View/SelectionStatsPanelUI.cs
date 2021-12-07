using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionStatsPanelUI : MonoBehaviour
{
    private Text _shipNameText;
    private ShipUIManager _shipsUiManager;

    void Awake()
    {
        this._shipsUiManager = FindObjectOfType<ShipUIManager>();
        this._shipNameText = transform.Find("ShipNameReadout").GetComponent<Text>();
        //this.transform.Find("AdvancePhaseButton").GetComponent<Button>().onClick.AddListener(TryAdvancePhase);
    }

    void Update()
    {
        Ship selectedShip = _shipsUiManager.GetSelectedShip();
        if (selectedShip != null)
        {
            _shipNameText.text = selectedShip.displayName;
        }
    }
}
