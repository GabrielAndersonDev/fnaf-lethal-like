using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : NetworkBehaviour
{
    [SerializeField]
    Player player;

    public float sensX;
    public float sensY;

    float xRotation;
    float yRotation;

    InputAction mouseAction;

    private void Start()
    {
        mouseAction = player.playerActionMap.FindAction("Look");
    }

    public void CameraInput()
    {
        if (player.isPaused) return;

        Vector2 mouseInput = mouseAction.ReadValue<Vector2>();

        float mouseX = mouseInput.x * Time.deltaTime * sensX;
        float mouseY = mouseInput.y * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        player.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
}
