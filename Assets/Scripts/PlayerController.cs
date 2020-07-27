using UnityEngine;

using System.Collections.Generic;

using static UnityMiscUtils.VectorRotation;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10.0f;
    public Camera controllingCamera = null;

    void Start()
    {
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

        transform.Translate(moveDirection.normalized * Time.deltaTime * moveSpeed, Space.Self);

        Vector3 viewTarget = controllingCamera.ScreenToWorldPoint(Input.mousePosition);
        viewTarget.y = transform.position.y;

        transform.LookAt(viewTarget);
    }
}