using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class SubtitleSystem : MonoBehaviour
{
    [Header("Always add the narratorMaster script here")]
    [SerializeField] NarratorMaster narratorMaster;

    public static bool enableSubtitles = true;
    [SerializeField] TMP_Text subtitleText;
    [SerializeField] SubtitleInput[] subtitles;

    float subtitleTimer;
    int currentIteration = 0;
    bool goNext = false;
    
    bool doLast = false;
    float lastDisplayTime = 2;


    private void Update()
    {
        if (enableSubtitles) subtitleText.enabled = true;
        else subtitleText.enabled = false;

        subtitleTimer = narratorMaster.GetAudioPlayingTime();
    }

    public void StartSubtitles()
    {
        if (subtitles.Length == 0) return;
        narratorMaster.SetNewSubtitles(this);
        currentIteration = 0;
        goNext = false;
        doLast = false;
        StartCoroutine(CurrentSentence(subtitles[0]));
    }

    public void StopActiveSubtitles()
    {
        StopAllCoroutines();
    }

    // sentence generator based on input in the subtitleInput, will generate lines with pre-setup timings
    IEnumerator CurrentSentence(SubtitleInput currentSubtitle)
    {
        currentIteration++;
        if (currentIteration < subtitles.Length) StartCoroutine(NextSentence(subtitles[currentIteration]));
        else doLast = true;

        int currentSymbol = 0;
        subtitleText.text = "";
        while (currentSubtitle.subtitle.Length != currentSymbol && !goNext)
        {
            subtitleText.text += currentSubtitle.subtitle[currentSymbol];
            currentSymbol++;
            yield return new WaitForSeconds(currentSubtitle.textSpeed);
        }

        yield return new WaitUntil(() => goNext || doLast);

        if (doLast) StartCoroutine(LastSentence());
        else if (goNext)
        {
            goNext = false;
            StartCoroutine(CurrentSentence(subtitles[currentIteration]));
        }
    }

    IEnumerator NextSentence(SubtitleInput nextSubtitle)
    {
        yield return new WaitUntil(() => subtitleTimer >= nextSubtitle.timeStamp);
        goNext = true;
    }

    IEnumerator LastSentence()
    {
        yield return new WaitForSeconds(lastDisplayTime);
        subtitleText.text = "";
    }
}

[System.Serializable]
public struct SubtitleInput
{
    [TextArea] public string subtitle;
    [Header("Time in seconds")]
    public float timeStamp;
    public float textSpeed;
}