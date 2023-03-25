using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class AudioMaster : MonoBehaviour
{
    [Header("Starting audio, leave empty if not needed")]
    [SerializeField] AudioClip startClip;
    [SerializeField] float startAudioStartTime;
    [SerializeField] UnityEvent eventsOnFinishingStartingClip;

    [Space(10), Header("Main audiomixer for the audio")]
    [SerializeField] AudioMixerGroup audioMixerGroup;
    [SerializeField] AudioSource playerAudioSource; // source in 3d space would be in the players their head

    [Space(10), Header("Add subtitles aswell if needed")]
    [SerializeField] SubtitleSystem currentSubtitles;

    private void Awake()
    {
        playerAudioSource.outputAudioMixerGroup = audioMixerGroup;
    }

    private void Start()
    {
        if (startClip != null)
        {
            PlayAudioClip(startClip, startAudioStartTime);
            if (eventsOnFinishingStartingClip.GetPersistentEventCount() > 0)
                StartCoroutine(RunEventAfter(startClip.length, eventsOnFinishingStartingClip));

            if (currentSubtitles != null) currentSubtitles.StartSubtitles();
        }
    }

    // Coroutine to wait until a source is finished with playing
    public IEnumerator RunEventAfter(float waitTime, UnityEvent currentEvent)
    {
        yield return new WaitForSeconds(waitTime);
        currentEvent.Invoke();
    }

    // method to call audio on trigger
    public void PlayAudioClip(AudioClip audioClip, float startTime)
    {
        playerAudioSource.clip = audioClip;
        playerAudioSource.time = startTime;
        playerAudioSource.Play();
    }

    // method to call audio on trigger with event overload
    public void PlayAudioClip(AudioClip audioClip, UnityEvent newEvent, float startTime)
    {
        playerAudioSource.clip = audioClip;
        playerAudioSource.time = startTime;
        playerAudioSource.Play();
        StartCoroutine(RunEventAfter(audioClip.length, newEvent));
    }

    public void PlayAudioOneshot(AudioClip audioClip)
    {
        playerAudioSource.PlayOneShot(audioClip);
    }

    // Stop the audio for interuption
    public void StopPlayingCurrentClip()
    {
        playerAudioSource.Stop();
    }

    public void PauseAudio()
    {
        playerAudioSource.Pause();
    }

    public void ResumeAudio()
    {
        playerAudioSource.UnPause();
    }

    public float GetAudioPlayingTime()
    {
        if (playerAudioSource.clip == null) return -1;
        return playerAudioSource.time;
    }

    public void SetNewSubtitles(SubtitleSystem newSubtitles)
    {
        currentSubtitles?.StopActiveSubtitles();
        currentSubtitles = newSubtitles;
    }

    public bool IsPlayingSubtitles()
    {
        return currentSubtitles.isPlaying;
    }
}