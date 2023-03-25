using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBlockade : MonoBehaviour
{
    [SerializeField] int requiredInteractions = 0;
    int totalInteractions = 0;

    private void OnEnable()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    public void TurnOffBlockade()
    {
        totalInteractions++;
        if (totalInteractions >= requiredInteractions)
        {
            GetComponent<Collider>().enabled = false;
        }
    }
}
