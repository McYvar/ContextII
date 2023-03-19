using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class SubtitleSystem : MonoBehaviour
{
    [Header("Always add the audioMaster script here")]
    [SerializeField] AudioMaster audioMaster;

    public static bool enableSubtitles = true;
    [SerializeField] MainCanvasUtils mc;

    [SerializeField] SubtitleInput[] subtitles;

    float subtitleTimer;
    int subtitleIterator = 0;
    bool goNext = false;
    
    bool doLast = false;
    float lastDisplayTime = 2;


    private void Update()
    {
        if (enableSubtitles) mc.subtitleText.enabled = true;
        else mc.subtitleText.enabled = false;

        subtitleTimer = audioMaster.GetAudioPlayingTime();
    }

    public void StartSubtitles()
    {
        if (subtitles.Length == 0) return;
        audioMaster.SetNewSubtitles(this);
        subtitleIterator = 0;
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
        mc.TurnOffAllSubtitleElements();
        subtitleIterator++;

        /// We need four cases:
        /// 1. The default narrator case where only the narrator text is filled in
        /// 2. The case where there is a character on the left side and text on the right side
        /// 3. The other way around
        /// 4. The case if both text and characters ard filled in, they appear on both side with smaller text boxes

        if (subtitleIterator < subtitles.Length) StartCoroutine(NextSentence(subtitles[subtitleIterator]));
        else doLast = true;

        // case 1: If the narrator subtitle is filled in
        if (currentSubtitle.subtitle.Length > 0)
        {
            int currentSymbol = 0;
            while (currentSubtitle.subtitle.Length != currentSymbol && !goNext)
            {
                mc.subtitleText.text += currentSubtitle.subtitle[currentSymbol];
                currentSymbol++;
                yield return new WaitForSeconds(currentSubtitle.textSpeed);
            }
        }

        // case 2: If theres a character on the left side
        else if (currentSubtitle.LeftChar != null && currentSubtitle.RightChar == null)
        {
            mc.imageLeftCharacter.enabled = true;
            mc.imageLeftCharacter.sprite = currentSubtitle.LeftChar;
            mc.imageRightFullBalloon.enabled = true;

            int currentSymbol = 0;
            while (currentSubtitle.textRight.Length != currentSymbol && !goNext)
            {
                mc.textRightFull.text += currentSubtitle.textRight[currentSymbol];
                currentSymbol++;
                yield return new WaitForSeconds(currentSubtitle.textSpeed);
            }
        }

        // case 3: If theres a character on the right side
        else if (currentSubtitle.RightChar != null && currentSubtitle.LeftChar == null)
        {
            mc.imageRightCharacter.enabled = true;
            mc.imageRightCharacter.sprite = currentSubtitle.RightChar;
            mc.imageLeftFullBalloon.enabled = true;

            int currentSymbol = 0;
            while (currentSubtitle.textLeft.Length != currentSymbol && !goNext)
            {
                mc.textLeftFull.text += currentSubtitle.textLeft[currentSymbol];
                currentSymbol++;
                yield return new WaitForSeconds(currentSubtitle.textSpeed);
            }
        }

        // case 4: If theres a character on both sides
        else if (currentSubtitle.LeftChar != null && currentSubtitle.RightChar != null)
        {
            mc.imageLeftCharacter.enabled = true;
            mc.imageLeftCharacter.sprite = currentSubtitle.LeftChar;
            mc.imageRightBalloon.enabled = true;

            mc.imageRightCharacter.enabled = true;
            mc.imageRightCharacter.sprite = currentSubtitle.RightChar;
            mc.imageLeftBalloon.enabled = true;

            int maxLength = 0;
            if (currentSubtitle.textLeft.Length > currentSubtitle.textRight.Length) maxLength = currentSubtitle.textLeft.Length;
            else maxLength = currentSubtitle.textRight.Length;

            int currentSymbol = 0;
            while (maxLength != currentSymbol && !goNext)
            {
                if (currentSubtitle.textLeft.Length > currentSymbol) mc.textLeft.text += currentSubtitle.textLeft[currentSymbol];
                if (currentSubtitle.textRight.Length > currentSymbol) mc.textRight.text += currentSubtitle.textRight[currentSymbol];
                currentSymbol++;
                yield return new WaitForSeconds(currentSubtitle.textSpeed);
            }
        }

        yield return new WaitUntil(() => goNext || doLast);

        if (doLast) StartCoroutine(LastSentence());
        else if (goNext)
        {
            goNext = false;
            StartCoroutine(CurrentSentence(subtitles[subtitleIterator]));
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
        mc.TurnOffAllSubtitleElements();
    }
}

[System.Serializable]
public struct SubtitleInput
{
    [TextArea] public string subtitle;
    [Header("Time in seconds")]
    public float timeStamp;
    public float textSpeed;
    [Space(10), Header("Assign left char, text = right side and vise versa")]
    public Sprite LeftChar;
    [TextArea] public string textRight;
    public Sprite RightChar;
    [TextArea] public string textLeft;
}