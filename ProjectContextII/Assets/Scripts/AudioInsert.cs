using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider)), RequireComponent(typeof(SubtitleSystem)), RequireComponent(typeof(MeshRenderer))]
public class AudioInsert : MonoBehaviour
{
    [Header("Always add the audioMaster script here")]
    [SerializeField] AudioMaster audioMaster;

    [Space(10), Header("Audio file for insertion")]
    [SerializeField] AudioClip audioClip;
    [SerializeField] float audioStartTime;
    [SerializeField] UnityEvent events;

    [Space(10), Header("Check if this is audio for interuption")]
    [SerializeField] bool doInterupt = true;

    [Space(10), Header("Check if this hitbox should disapear")]
    [SerializeField] bool doRemove = true;
    BoxCollider myCollider;

    [Space(10), Header("What should be able to trigger this audio?")]
    [SerializeField] TriggerType responseType;

    SubtitleSystem subtitleSystem;

    private void Awake()
    {
        myCollider = GetComponent<BoxCollider>();
        myCollider.isTrigger = true;
        subtitleSystem = GetComponent<SubtitleSystem>();
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        ITrigger collider = other.GetComponent<ITrigger>();
        if (collider == null) return;
        if (collider.triggerType != responseType && responseType != TriggerType.ALL) return;

        if (doInterupt)
        {
            audioMaster.StopPlayingCurrentClip();
        }

        if (events.GetPersistentEventCount() > 0)
        {
            if (audioClip != null)
                audioMaster.PlayAudioClip(audioClip, events, audioStartTime);
        }
        else
        {
            if (audioClip != null)
                audioMaster.PlayAudioClip(audioClip, audioStartTime);
        }

        if (doRemove)
        {
            myCollider.enabled = false;
        }

        subtitleSystem.StartSubtitles();
    }
}
