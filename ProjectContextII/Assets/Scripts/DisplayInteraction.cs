using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(SphereCollider))]
public class DisplayInteraction : MonoBehaviour
{
    /// The plan is to display the interaction button the moment the player is within a certain range and angle of the thing to interact with.
    /// Then if those conditions are met, some sort of balloon pops up with the correct button to press and and arrow pointing towards the thing
    /// to interact with.
    /// 

    public bool displayHints = true; // later in a main file a static bool
    [SerializeField] KeyCode interactionKey = KeyCode.E;
    TMP_Text interactionDisplayText;
    MainCanvasUtils mc;
    GameObject interactionDisplay;
    [SerializeField] float maxInteractionDisplayAngle = 50;
    [SerializeField] float maxInteractionAngle = 15;
    [SerializeField, Range(0f, 0.8f)] float interactionDisplayOffset = 0.125f;
    
    [SerializeField] float interactionRadius = 8.5f;
    SphereCollider myCollider;

    [SerializeField] float interactionDelay = 2;
    float delayTimer;

    Vector2 myScreenPositon;
    Vector2 normalizedTargetVector;
    bool isEnabled;

    [SerializeField, Range(0f, 1f)] float smoothTime = 0.078f;
    Vector2 smoothdampVelocity = Vector2.zero;

    [SerializeField] AudioMaster audioMaster;
    [SerializeField] AudioClip interactionAudio;
    [SerializeField] UnityEvent onInteractionEvent;

    Item myItem;
    ItemHolder myItemHolder;

    private void Awake()
    {
        myCollider = GetComponent<SphereCollider>();
        myCollider.isTrigger = true;
        if (mc == null) mc = FindObjectOfType<MainCanvasUtils>();
        interactionDisplay = Instantiate(mc.interactionDisplay, mc.gameObject.transform);
        interactionDisplayText = interactionDisplay.GetComponentInChildren<TMP_Text>();
        myItem = GetComponent<Item>();
        myItemHolder = FindObjectOfType<ItemHolder>();
        DisableInteraction();
    }

    private void Start()
    {
        myScreenPositon = new Vector2(Screen.width * 2, Screen.height * 2);
        interactionDisplay.transform.position = myScreenPositon;
        interactionDisplay.SetActive(true);
    }

    private void Update()
    {
        myCollider.radius = interactionRadius;

        if (delayTimer > 0f) delayTimer -= Time.deltaTime;

        if (isEnabled) interactionDisplay.transform.position = Vector2.SmoothDamp(interactionDisplay.transform.position, myScreenPositon + normalizedTargetVector * (Screen.width * interactionDisplayOffset), ref smoothdampVelocity, smoothTime);
        else interactionDisplay.transform.position = Vector2.SmoothDamp(interactionDisplay.transform.position, myScreenPositon + normalizedTargetVector * (Screen.width * 1.5f), ref smoothdampVelocity, smoothTime);
    }

    // first a method to detect if the player is in range
    private void OnTriggerStay(Collider other)
    {
        if (!displayHints || delayTimer > 0f) { DisableInteraction(); return; }
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;
        float currentAngle = Vector3.Angle(player.playerCameraTransform.transform.forward, transform.position - player.playerCameraTransform.transform.position);
        if (currentAngle > maxInteractionDisplayAngle) { DisableInteraction(); return; }

        /// we need the position of the centre of the screen and the screen position of the interactable then we create a vector from those two positions,
        /// normalize it and scale it with an offset, also an arrow pointing from the displaying interactable to the actual interactable
        myScreenPositon = player.playerCameraTransform.WorldToScreenPoint(transform.position);
        Vector2 centreOfScreen = new Vector3(Screen.width / 2, Screen.height / 2);
        normalizedTargetVector = (centreOfScreen - myScreenPositon).normalized;
        if (myItem != null && myItemHolder.item != null) { DisableInteraction(); return; }
        EnableInteraction();

        if (Input.GetKey(interactionKey) && currentAngle < maxInteractionAngle)
        {
            delayTimer = interactionDelay;
            OnInteract();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;
        DisableInteraction();
    }

    private void DisableInteraction()
    {
        interactionDisplay.GetComponent<RawImage>().enabled = false;
        interactionDisplayText.enabled = false;
        isEnabled = false;
    }

    private void EnableInteraction()
    {
        if (!isEnabled)
        {
            interactionDisplay.GetComponent<RawImage>().enabled = true;
            interactionDisplayText.enabled = true;
            interactionDisplay.transform.position = myScreenPositon + normalizedTargetVector * (Screen.width * 1.5f);
            interactionDisplayText.text = interactionKey.ToString();
        }
        isEnabled = true;
    }

    private void OnInteract()
    {
        Debug.Log("Interacted!");
        if (interactionAudio != null) audioMaster?.PlayAudioOneshot(interactionAudio);
        onInteractionEvent.Invoke();
    }
}
