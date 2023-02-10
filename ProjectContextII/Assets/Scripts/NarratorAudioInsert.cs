using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NarratorAudioInsert : MonoBehaviour
{
    [Header("Always add the narratorMaster object here")]
    [SerializeField] NarratorMaster narratorMaster;

    [Space(10), Header("Audio file for insertion")]
    [SerializeField] AudioClip audioClip;
    [SerializeField] UnityEvent events;

    [Space(10), Header("Check if this is audio for interuption")]
    [SerializeField] bool doInterupt;

    [Space(10), Header("Do Event after certain amount of time")]
    [SerializeField] float timedEventTime;
    [SerializeField] UnityEvent timedEvent;
    float timedEventTimer;

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        if (doInterupt)
        {
            narratorMaster.StopPlayingCurrentClip();
        }

        if (events.GetPersistentEventCount() > 0) narratorMaster.PlayAudioClip(audioClip, events);
        else narratorMaster.PlayAudioClip(audioClip);

        if (timedEvent.GetPersistentEventCount() > 0) timedEventTimer = timedEventTime;
    }

    private void Update()
    {
        if (timedEventTimer > 0)
        {
            timedEventTime -= Time.deltaTime;            
            if (timedEventTime < 0) timedEvent.Invoke();
        }
    }

    public void DebugMessageEvent()
    {
        Debug.Log("Test!!!");
    }
}