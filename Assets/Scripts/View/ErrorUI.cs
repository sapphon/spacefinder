using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorUI : MonoBehaviour
{

    public static ErrorUI Get()
    {
        return FindObjectOfType<ErrorUI>();
    }

    public float alphaReductionRate = 0.5f;
    private Text _text;

    void Start()
    {
        _text = GetComponent<Text>();
    }

    void Update()
    {
        if (isShowing())
        {
            SetAlpha(_text.color.a - (alphaReductionRate * Time.deltaTime));
        }
    }

    private bool isShowing()
    {
        return !Mathf.Approximately(_text.color.a, 0f);
    }

    public void ShowError(String text)
    {
        _text.text = text;
        SetAlpha(1);
    }

    private void SetAlpha(float alpha)
    {
        _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, Mathf.Max(0,alpha));
    }
}
