using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubtitleSystem : MonoBehaviour
{
    [SerializeField] bool enableSubtitles;
    [SerializeField] SubtitleInput[] subtitles;

    private void Update()
    {
        if (!enableSubtitles) return;
    }
}

[System.Serializable]
public struct SubtitleInput
{
    [TextArea, SerializeField] string subtitle;
    [SerializeField] float timeStamp;
    [SerializeField] float textSpeed;
}