using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;
using UnityEngine.UI;

public class RoundPanelUI : MonoBehaviour
{
    private Text _readoutText;
    private RoundManager _roundManager;

    void Awake()
    {
        this._roundManager = FindObjectOfType<RoundManager>();
        this._readoutText = transform.Find("CurrentRoundReadout").GetComponent<Text>();
    }

    void Update()
    {
        _readoutText.text = _roundManager.GetCurrentRound().ToString();
    }

}
