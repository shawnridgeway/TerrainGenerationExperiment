using UnityEngine;
using System.Collections;

public class ObserverController : MonoBehaviour {
    public bool isActive = false;
    public float speed = 6f;
    public float jumpSpeed = 8f;
    public float gravity = 20f;
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;
    public TestTerrain world;

    private void Start() {
        controller = GetComponent<CharacterController>();
    }

    void Update() {
        if (!isActive) {
            return;
        }
        transform.Rotate(0, Input.GetAxis("Horizontal") * 100 * Time.deltaTime, 0);
        if (controller.isGrounded) {
            moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            if (Input.GetButton("Jump")) {
                moveDirection.y = jumpSpeed;
            }

        }
        Vector3 gravityDirection = world.GetSurfaceNormal(transform.position) * -1;
        moveDirection += gravity * gravityDirection * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }
}