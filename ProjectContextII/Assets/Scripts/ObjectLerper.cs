using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLerper : MonoBehaviour
{
    [SerializeField] protected Vector3 positionA;
    [SerializeField] protected Vector3 positionB;

    [SerializeField, Range(0, 1)] protected float interpolationSize;
    public float currentInterpolation;

    [SerializeField] protected bool swing;

    private void Update()
    {
        Lerping();
    }

    protected bool Lerping()
    {
        transform.position = Vector3.Lerp(positionA, positionB, currentInterpolation);
        currentInterpolation += interpolationSize * Time.deltaTime;

        if (swing)
        {
            if (currentInterpolation >= 1)
            {
                interpolationSize = -Mathf.Abs(interpolationSize);
                currentInterpolation = 1;
            }
            if (currentInterpolation <= 0)
            {
                interpolationSize = Mathf.Abs(interpolationSize);
                currentInterpolation = 0;
            }
        }
        else
        {
            if (currentInterpolation >= 1)
            {
                currentInterpolation = 0;
                return true;
            }
        }
        return false;
    }
}
