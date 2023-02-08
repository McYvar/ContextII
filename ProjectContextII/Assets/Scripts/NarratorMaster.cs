using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class NarratorMaster : MonoBehaviour
{
    [Header("Starting audio, leave empty if not needed")]
    [SerializeField] AudioClip startClip;
    [SerializeField] UnityEvent eventsOnFinishingStartingClip;

    [Space(10), Header("Main audiomixer for the narrator")]
    [SerializeField] AudioMixerGroup narratorMixerGroup;
    [SerializeField] AudioSource playerAudioSource; // source in 3d space would be in the players their head

    private void Awake()
    {
        playerAudioSource.outputAudioMixerGroup = narratorMixerGroup;
    }

    private void Start()
    {
        if (startClip != null)
        {
            PlayAudioClip(startClip);
            if (eventsOnFinishingStartingClip.GetPersistentEventCount() > 0)
                StartCoroutine(RunEventAfter(startClip.length, eventsOnFinishingStartingClip));
        }
    }

    // Coroutine to wait until a source is finished with playing
    public IEnumerator RunEventAfter(float waitTime, UnityEvent currentEvent)
    {
        yield return new WaitForSeconds(waitTime);
        currentEvent.Invoke();
    }

    // method to call audio on trigger
    public void PlayAudioClip(AudioClip audioClip)
    {
        playerAudioSource.PlayOneShot(audioClip);
    }

    // method to call audio on trigger with event overload
    public void PlayAudioClip(AudioClip audioclip, UnityEvent newEvent)
    {
        playerAudioSource.PlayOneShot(audioclip);
        StartCoroutine(RunEventAfter(audioclip.length, newEvent));
    }

    // Stop the audio for interuption
    public void StopPlayingCurrentClip()
    {
        playerAudioSource.Stop();
    }
}