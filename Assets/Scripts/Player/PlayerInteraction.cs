using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public partial class Player : NetworkBehaviour
{
   
    [Header("Interaction")]
    // Range for interaction possibility of Player, can be changed and tested in the future.
    public float interactRange = 5.0f;
    public Camera playerCamera;

    public GameObject CheckForRange()
    {
        Ray ray = new(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            if (hit.collider.CompareTag("Interactable") 
                || hit.collider.CompareTag("Player") 
                || hit.collider.CompareTag("Enemy"))
            {
                return hit.collider.gameObject;
            }
            else if (hit.collider.CompareTag("Ceiling") 
                     || hit.collider.CompareTag("Floor") 
                     || hit.collider.CompareTag("Wall"))
            {
                // Make sure that anything I hit is a Floor or Skybox Object
                Debug.Log($"No interactable here. {hit.collider.gameObject}");
                return hit.collider.gameObject;
            }
            else
            {
                Debug.LogError($"Hitting something not compensated for. {hit.collider} and {hit.collider.gameObject.name}");
                Debug.Break();
            }
        }
        else if (hit.collider == null) 
        {
            Debug.Log($"Raycast did not hit anything.");
        }
        return null;
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
            Debug.LogError("Doors do not currently have a function under 'Interact()'.");
        }
        else
        {
            Debug.Log("Not a door or item.");
        }
    }
}
