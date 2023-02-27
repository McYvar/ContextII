using UnityEngine;

public class ObjectLerperList : ObjectLerper
{
    [Header("If you use the object lerper list swing is disabled by default")]
    [SerializeField] ObjectLerperPositionStruct[] list;
    int listIterator = 0;

    private void Start()
    {
        SetNewPositions(listIterator);
        swing = false;
    }

    private void Update()
    {
        if (Lerping())
        {
            if (listIterator == list.Length - 1) listIterator = 0;
            else listIterator++;
            SetNewPositions(listIterator);
        }
    }

    private void SetNewPositions(int iterator)
    {
        positionA = list[iterator].positionA;
        positionB = list[iterator].positionB;
        transform.rotation = Quaternion.Euler(list[iterator].newRotation);
        interpolationSize = list[iterator].newInterpolationSize;
    }
}

[System.Serializable]
public struct ObjectLerperPositionStruct
{
    public Vector3 positionA;
    public Vector3 positionB;
    public Vector3 newRotation;
    [Range(0, 1)] public float newInterpolationSize;
}
