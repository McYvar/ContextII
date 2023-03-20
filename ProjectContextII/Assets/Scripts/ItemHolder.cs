using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    public Item item;
    [SerializeField] KeyCode throwKey;
    [SerializeField] Transform orientation;

    private void Update()
    {
        if (Input.GetKeyDown(throwKey) && item != null)
        {
            item.ThrowMe(orientation.position, orientation.forward);
            item = null;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        Item item = collision.collider.GetComponent<Item>();
        if (item != null && this.item == null && !item.hasInteraction) this.item = item;
        else return;
        if (item.currentState == CurrentItemState.IN_THE_AIR ||
            item.currentState == CurrentItemState.PICKED_UP) return;
        item.PickMeUp();
    }

    public void SetItemManually(Item newItem)
    {
        item = newItem;
    }
}
