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
    }

    void Update()
    {
        Ship selectedShip = _shipsUiManager.GetSelectedShip();
        if (selectedShip != null)
        {
            _shipNameText.text = selectedShip.displayName;
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
        _shipNameText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 160f);
    }

    protected void ShrinkPanel()
    {
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
        _shipNameText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
    }
}
