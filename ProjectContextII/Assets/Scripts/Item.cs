using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour, IThrowable, IPickUpable, ITrigger
{
    [SerializeField] Vector3 uiLocation;
    [SerializeField] Quaternion uiRotation;
    [SerializeField] Vector3 uiScale;
    [SerializeField] float throwDistanceOffset;
    LayerMask normalLayer;
    Vector3 normalScale;

    [SerializeField] float throwStrenght;
    [SerializeField] float maxRaycastDist;
    Collider myCollider;

    public TriggerType triggerType { get; set; }
    Rigidbody rb;
    [HideInInspector] public bool hasInteraction;

    [HideInInspector] public CurrentItemState currentState = CurrentItemState.ON_THE_GROUND;

    private void Awake()
    {
        normalScale = transform.localScale;
        normalLayer = gameObject.layer;
        triggerType = TriggerType.THROWABLE_OBJECT;
        rb = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();

        if (GetComponent<DisplayInteraction>() != null)
        {
            hasInteraction = true;
        }
        else hasInteraction = false;
    }

    private void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, maxRaycastDist))
        {
            currentState = CurrentItemState.ON_THE_GROUND;
        }
    }

    // When picking up an object
    public void PickMeUp()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        myCollider.enabled = false;

        currentState = CurrentItemState.PICKED_UP;
        transform.position = uiLocation;
        transform.rotation = uiRotation;
        transform.localScale = uiScale;
        //gameObject.layer = LayerMask.NameToLayer("OverlayUI");
        SetGameLayerRecursive(gameObject, LayerMask.NameToLayer("OverlayUI"));
    }

    public void ThrowMe(Vector3 startThrowLocation, Vector3 throwDirection)
    {
        rb.constraints = RigidbodyConstraints.None;
        myCollider.enabled = true;

        currentState = CurrentItemState.IN_THE_AIR;
        transform.position = startThrowLocation + throwDirection.normalized * throwDistanceOffset;
        transform.rotation = Quaternion.identity;
        //transform.localScale = normalScale;
        SetGameLayerRecursive(gameObject, normalLayer);
        gameObject.layer = normalLayer.value;

        rb.AddForce(throwDirection.normalized * throwStrenght, ForceMode.VelocityChange);
    }

    private void SetGameLayerRecursive(GameObject _go, int _layer)
    {
        _go.layer = _layer;
        foreach (Transform child in _go.transform)
        {
            child.gameObject.layer = _layer;

            Transform _HasChildren = child.GetComponentInChildren<Transform>();
            if (_HasChildren != null)
                SetGameLayerRecursive(child.gameObject, _layer);

        }
    }
}
public enum CurrentItemState { ON_THE_GROUND = 0, PICKED_UP = 1, IN_THE_AIR = 2 }
