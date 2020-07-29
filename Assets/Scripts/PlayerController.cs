// Title: PlayerController.cs
// Author: Harry Donovan
// Collaborators:
// License: GNU General Public License v3.0
// Date Last Edited: 29/07/20
// Last Edited By: Harry Donovan
// References: 
// File Source: https://github.com/HDonovan96/ViewConeVisualization
// Description: A very simple 2D character controller. The character faces the mouse and moves with WASD relative to world axis.

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10.0f;
    public Camera controllingCamera = null;

    void Start()
    {
        // Fetches the scene main camera if it hasn't been set in inspector.
        if (controllingCamera == null)
        {
            controllingCamera = Camera.main;
        }
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveDirection.z += 1.0f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection.z += -1.0f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection.x += 1.0f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection.x += -1.0f;
        }

        transform.Translate(moveDirection.normalized * Time.deltaTime * moveSpeed, Space.World);

        Vector3 viewTarget = controllingCamera.ScreenToWorldPoint(Input.mousePosition);
        viewTarget.y = transform.position.y;

        transform.LookAt(viewTarget);
    }
}