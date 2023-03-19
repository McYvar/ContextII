using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvasUtils : MonoBehaviour
{
    public TMP_Text subtitleText;

    public Image imageLeftCharacter;
    public Image imageRightFullBalloon;
    public TMP_Text textRightFull;
    public Image imageRightBalloon;
    public TMP_Text textRight;

    public Image imageRightCharacter;
    public Image imageLeftFullBalloon;
    public TMP_Text textLeftFull;
    public Image imageLeftBalloon;
    public TMP_Text textLeft;

    public GameObject interactionDisplay;

    public void TurnOffAllSubtitleElements()
    {
        subtitleText.text = "";

        imageLeftCharacter.enabled = false;
        imageRightFullBalloon.enabled = false;
        textRightFull.text = "";
        imageRightBalloon.enabled = false;
        textRight.text = "";

        imageRightCharacter.enabled = false;
        imageLeftFullBalloon.enabled = false;
        textLeftFull.text = "";
        imageLeftBalloon.enabled = false;
        textLeft.text = "";
    }
}
