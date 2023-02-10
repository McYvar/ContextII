using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMessage : MonoBehaviour
{
    [SerializeField] string testMessage;

    public void LogTestMessage()
    {
        Debug.Log(testMessage);
    }
}
