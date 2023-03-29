using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider)), RequireComponent(typeof(AudioSystem)), RequireComponent(typeof(MeshRenderer))]
public class AudioSystem : MonoBehaviour
{
    [Header("Always add the audioMaster and maincavas script here")]
    [SerializeField] AudioMaster audioMaster;
    [SerializeField] MainCanvasUtils mc;

    [Space(10), Header("Check if this is audio for interuption")]
    [SerializeField] bool doInterupt = true;

    [Space(10), Header("Check if this hitbox should disapear")]
    [SerializeField] bool doRemove = true;
    BoxCollider myCollider;

    [Space(10), Header("What should be able to trigger this system?")]
    [SerializeField] TriggerType responseType;

    PlayerController playerController;
    [SerializeField] bool slowDownPlayer = true;

    public static bool enableSubtitles = true;

    [Space(10), SerializeField] SubtitleInput[] subtitles;

    [SerializeField] UnityEvent finalEvent;

    int subtitleIterator = 0;
    bool goNext = false;

    bool doLast = false;

    [HideInInspector] public bool isPlaying = false;
    bool isTyping = false;


    private void Awake()
    {
        myCollider = GetComponent<BoxCollider>();
        myCollider.isTrigger = true;
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void Update()
    {
        if (enableSubtitles) mc.subtitleText.enabled = true;
        else mc.subtitleText.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        ITrigger collider = other.GetComponent<ITrigger>();
        if (collider == null) return;
        if (collider.triggerType != responseType && responseType != TriggerType.ALL) return;

        playerController = other.GetComponent<PlayerController>();

        if (doInterupt)
        {
            audioMaster.StopPlayingCurrentClip();
        }
        else if (audioMaster.IsPlayingSubtitles()) return;

        if (doRemove)
        {
            myCollider.enabled = false;
        }

        StartSubtitles();
    }

    public void StartSubtitles()
    {
        if (subtitles.Length == 0) return;
        audioMaster.SetNewSubtitles(this);
        subtitleIterator = 0;
        goNext = false;
        doLast = false;
        isPlaying = true;
        isTyping = false;
        if (playerController != null && slowDownPlayer) playerController.SetWalkingSpeed(1f);
        StartCoroutine(CurrentSentence(subtitles[0]));
    }

    public void StopActiveSubtitles()
    {
        if (playerController != null && slowDownPlayer) playerController.SetWalkingSpeed(5f);
        StopAllCoroutines();
    }

    // sentence generator based on input in the subtitleInput, will generate lines with pre-setup timings
    IEnumerator CurrentSentence(SubtitleInput currentSubtitle)
    {
        yield return new WaitUntil(() => !Input.GetMouseButton(0));
        mc.TurnOffAllSubtitleElements();
        subtitleIterator++;

        isTyping = true;
        if (subtitleIterator < subtitles.Length) StartCoroutine(NextSentence(subtitles[subtitleIterator]));
        else doLast = true;

        currentSubtitle.startEvent.Invoke();

        if (currentSubtitle.audioClip != null)
        {
            audioMaster.StopPlayingCurrentClip();
            audioMaster.PlayAudioClip(currentSubtitle.audioClip, 0);
        }

        // case 1: If the narrator subtitle is filled in
        if (currentSubtitle.subtitle.Length > 0)
        {
            int currentSymbol = 0;
            while (currentSubtitle.subtitle.Length != currentSymbol && !goNext)
            {
                mc.subtitleText.text += currentSubtitle.subtitle[currentSymbol];
                currentSymbol++;
                if (!Input.GetMouseButton(0))
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
                if (!Input.GetMouseButton(0))
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
                if (!Input.GetMouseButton(0))
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
                if (!Input.GetMouseButton(0))
                    yield return new WaitForSeconds(currentSubtitle.textSpeed);
            }
        }
        isTyping = false;

        yield return new WaitUntil(() => goNext || doLast);

        if (doLast) StartCoroutine(LastSentence());
        else if (goNext)
        {
            goNext = false;
            StartCoroutine(CurrentSentence(subtitles[subtitleIterator]));
        }
    }

    /// <summary>
    /// Okay so we can only go to the next sentence when the sentence if fully finished, then we can go to the next sentence by the
    /// press of a left mouse button. BUT, we can skip building the sentence, also when pressing left.
    /// So what we need is, left press down finishes the sentence, but wait until left press is released again.
    /// 
    /// We also want a force to the next sentence, this can only ocur when forcing is turned on
    /// </summary>
    IEnumerator NextSentence(SubtitleInput nextSubtitle)
    {
        yield return new WaitUntil(() => !isTyping);
        yield return new WaitUntil(() => (Input.GetMouseButtonDown(0) && !goNext) || 
                                         (nextSubtitle.forceToThisOneOnFinish && !audioMaster.isPlaying));
        goNext = true;
    }

    IEnumerator LastSentence()
    {
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || !audioMaster.isPlaying);
        mc.TurnOffAllSubtitleElements();
        audioMaster.StopPlayingCurrentClip();
        isPlaying = false;

        finalEvent.Invoke();
        if (playerController != null && slowDownPlayer) playerController.SetWalkingSpeed(5f);
    }
}

[System.Serializable]
public struct SubtitleInput
{
    public UnityEvent startEvent;
    public AudioClip audioClip;
    public bool forceToThisOneOnFinish;
    [TextArea] public string subtitle;
    public float textSpeed;
    [Space(10), Header("Assign left char, text = right side and vise versa")]
    public Sprite LeftChar;
    [TextArea] public string textRight;
    public Sprite RightChar;
    [TextArea] public string textLeft;
}