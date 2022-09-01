using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backgrounds : MonoBehaviour
{
    public Sprite[] backgrounds;
    protected SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Awake()
    {
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    protected int getIndexOfCurrentBackground()
    {
        return Array.IndexOf(backgrounds, spriteRenderer.sprite);
    }

    public void next()
    {
        this.spriteRenderer.sprite = backgrounds[(getIndexOfCurrentBackground() + 1) % backgrounds.Length ];
    }
}
