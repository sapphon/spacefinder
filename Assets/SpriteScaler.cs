using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteScaler : MonoBehaviour
{

    public float minScaleNormalized = 0.8f;

    public float maxScaleNormalized = 1.2f;

    public float scalingSpeed = 1f;
    
    private float currentScale = 1f;

    void Update()
    {
        this.transform.localScale.Set(currentScale, currentScale, 1);
        GrowOrShrink();
    }

    private void GrowOrShrink()
    {
        bool shouldReverse = this.currentScale > maxScaleNormalized || this.currentScale < minScaleNormalized;
        if (shouldReverse) scalingSpeed = -scalingSpeed;
        this.currentScale += scalingSpeed * 0.01f * Time.deltaTime;
    }
}
