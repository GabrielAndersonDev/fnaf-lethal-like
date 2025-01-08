using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
   
    [Header("Interaction")]
    public float interactRange = 5.0f;
    float distance;
    Vector3 playerLocation;
    public Camera playerCamera;

    void CheckForInteractable()
    {
        Ray ray = new(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                Interact(hit.collider.gameObject);
            }
            else
            {
                Debug.Log($"No interactable here. {hit.collider.gameObject}");
            }
        }
        else
        {
            Debug.Log($"Raycast did not hit anything.");
        }
    }
    public void Interact(GameObject interactable)
    {
        Item item = interactable.GetComponent<Item>();
        Door door = interactable.GetComponent<Door>();

        if (item != null)
        {
            AddItem(item);
        }
        else if (door != null)
        {

        }
        else
        {
            Debug.Log("Not a door or item.");
        }
    }
}
