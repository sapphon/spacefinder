using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRotator : MonoBehaviour
{

    public float rotationSpeed = 1f;

    //arc through which to rotate before returning; 0 or 360 means an infinite spin
    public int rotationArc = 360;
    
    // Update is called once per frame
    void Update()
    {
        this.transform.RotateAround(Vector3.forward, rotationSpeed * Time.deltaTime);
        reverseIfNecessary();
    }

    private void reverseIfNecessary()
    {
        if ((rotationArc % 360 != 0) && Mathf.Abs(this.gameObject.transform.eulerAngles.z) > Mathf.Abs(rotationArc / 2f))
        {
            this.rotationSpeed = -rotationSpeed;
        }
    }
}
