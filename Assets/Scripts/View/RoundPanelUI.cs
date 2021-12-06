using System.Collections;
using System.Collections.Generic;
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
        this.transform.Find("AdvanceRoundButton").GetComponent<Button>().onClick.AddListener(TryAdvanceRound);
    }

    void Update()
    {
        _readoutText.text = _roundManager.GetCurrentRound().ToString();
    }

    public void TryAdvanceRound()
    {
        bool roundAdvanced = _roundManager.TryAdvanceRound();
        if (!roundAdvanced)
        {
            Debug.Log("Round not advanced; not all ships ready");
        }
    }
}
