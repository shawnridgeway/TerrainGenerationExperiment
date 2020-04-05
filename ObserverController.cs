using UnityEngine;
using System.Collections;

public class ObserverController : MonoBehaviour {
    public bool isActive = false;
    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;

    private void Start() {
        controller = GetComponent<CharacterController>();
    }

    void Update() {
        if (!isActive) {
            return;
        }
        transform.Rotate(0, Input.GetAxis("Horizontal") * 75 * Time.deltaTime, 0);
        if (controller.isGrounded) {
            moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical") * 2);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            if (Input.GetButton("Jump")) {
                moveDirection.y = jumpSpeed;
            }

        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }
}