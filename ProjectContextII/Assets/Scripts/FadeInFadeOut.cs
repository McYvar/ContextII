using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInFadeOut : MonoBehaviour
{
    [SerializeField] RawImage overlayImage;
    [SerializeField] ObjectLerper objectLerper;
    [SerializeField, Range(0, 1)] float fadeGap;

    float alpha;

    private void Update()
    {
        float x = -1 + 2 * objectLerper.currentInterpolation;
        alpha = (1 / 0.5f * (x*x)) - fadeGap;
        overlayImage.color = new Color(overlayImage.color.r, overlayImage.color.g, overlayImage.color.b, alpha);
    }
}
