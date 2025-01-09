using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
   
    [Header("Interaction")]
    public float interactRange = 5.0f;
    float distance;
    public Camera playerCamera;

    public GameObject CheckForRange()
    {
        Ray ray = new(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            if (hit.collider.CompareTag("Interactable") || hit.collider.CompareTag("Player") || hit.collider.CompareTag("Enemy"))
            {
                return hit.collider.gameObject;
            }
            else
            {
                Debug.Log($"No interactable here. {hit.collider.gameObject}");
                return null;
            }
        }
        else
        {
            Debug.Log($"Raycast did not hit anything.");
            return null;
        }
    }
    public void Interact()
    {
        Item item = CheckForRange().GetComponent<Item>();
        Door door = CheckForRange().GetComponent<Door>();

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
