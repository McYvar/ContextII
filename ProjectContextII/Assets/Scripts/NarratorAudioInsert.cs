using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider)), RequireComponent(typeof(SubtitleSystem))]
public class NarratorAudioInsert : MonoBehaviour
{
    [Header("Always add the narratorMaster script here")]
    [SerializeField] NarratorMaster narratorMaster;

    [Space(10), Header("Audio file for insertion")]
    [SerializeField] AudioClip audioClip;
    [SerializeField] float audioStartTime;
    [SerializeField] UnityEvent events;

    [Space(10), Header("Check if this is audio for interuption")]
    [SerializeField] bool doInterupt = true;

    [Space(10), Header("Check if this hitbox should disapear")]
    [SerializeField] bool doRemove = true;
    BoxCollider myCollider;

    [Space(10), Header("Do Event after certain amount of time")]
    [SerializeField] float timedEventTime;
    [SerializeField] UnityEvent timedEvent;
    float timedEventTimer;

    SubtitleSystem subtitleSystem;

    private void Awake()
    {
        myCollider = GetComponent<BoxCollider>();
        subtitleSystem = GetComponent<SubtitleSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        if (doInterupt)
        {
            narratorMaster.StopPlayingCurrentClip();
        }

        if (events.GetPersistentEventCount() > 0) narratorMaster.PlayAudioClip(audioClip, events, audioStartTime);
        else narratorMaster.PlayAudioClip(audioClip, audioStartTime);

        if (timedEvent.GetPersistentEventCount() > 0) timedEventTimer = timedEventTime;

        if (doRemove)
        {
            myCollider.enabled = false;
        }

        subtitleSystem.StartSubtitles();
    }

    private void Update()
    {
        if (timedEventTimer > 0)
        {
            timedEventTimer -= Time.deltaTime;
            if (timedEventTimer < 0) timedEvent.Invoke();
        }
    }

    public void DebugMessageEvent()
    {
        Debug.Log("Test!!!");
    }
}