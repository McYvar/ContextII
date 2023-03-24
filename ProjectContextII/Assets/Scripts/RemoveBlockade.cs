using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBlockade : MonoBehaviour
{
    [SerializeField] int requiredInteractions = 0;
    int totalInteractions = 0;
    public void TurnOffBlockade()
    {
        totalInteractions++;
        if (totalInteractions >= requiredInteractions)
            GetComponent<Collider>().enabled = false;
    }
}
