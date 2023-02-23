using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLerper : MonoBehaviour
{
    [SerializeField] Vector3 positionA;
    [SerializeField] Vector3 positionB;

    [SerializeField, Range(0, 1)] float interpolationSize;
    float currentInterpolation;

    [SerializeField] bool swing;

    private void Update()
    {
        transform.position = Vector3.Lerp(positionA, positionB, currentInterpolation);
        currentInterpolation += interpolationSize * Time.deltaTime;

        if (swing)
        {
            if (currentInterpolation > 1)
            {
                interpolationSize = -Mathf.Abs(interpolationSize);
                currentInterpolation = 1;
            }
            if (currentInterpolation < -1)
            {
                interpolationSize = Mathf.Abs(interpolationSize);
                currentInterpolation = -1;
            }
        }
    }
}
