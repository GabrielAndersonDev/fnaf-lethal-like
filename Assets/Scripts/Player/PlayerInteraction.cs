using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
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
            if (hit.collider.CompareTag("Untagged"))
            {
                Debug.LogError($"Hitting something not compensated for. {hit.collider} and {hit.collider.gameObject.name}");
                Debug.Break();
            }

            return hit.collider.gameObject;
        }

        return null;
    }
    public void Interact()
    {
        GameObject hitObj = CheckForRange();

        if (hitObj == null)
        {
            return;
        }

        switch (hitObj.tag)
        {
            case "Item":
                if (hitObj.TryGetComponent<Item>(out Item outItem))
                {
                    AddItemSlotCheck(outItem);
                }
                else
                {
                    Debug.LogWarning("Unable to get component 'Item' on GameObject with Item tag. " + hitObj.name);
                    Debug.Break();
                }
                break;
            case "Door":
                Debug.LogWarning("Door tag unimplemented.");
                Debug.Break();
                break;
            case "Button":
                if (hitObj.TryGetComponent<MapButton>(out MapButton outBtn))
                {
                    outBtn.ButtonInteract();
                    Debug.Log("Pressed button");
                }
                else
                {
                    Debug.LogError("Unable to get component 'MapButton' on GameObject with Button tag." + hitObj.name);
                }
                break;
            case "Player":
                Debug.Log("Hit player " + hitObj.GetComponent<Player>().playerName);
                break;
            case "Enemy":
                Debug.Log("Hit enemy " + hitObj.GetComponent<Enemy>().name);
                break;
            case "Ceiling":
                break;
            case "Floor":
                break;
            case "Wall":
                break;
            default:
                Debug.Log(hitObj.tag + "is unaccounted for.");
                Debug.Break();
                break;
        }
    }
}
