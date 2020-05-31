using UnityEngine;
using System.Collections;

public class ObserverController : MonoBehaviour {
    public bool isActive = false;
    public bool isGrounded = false;
    public float moveSpeed = 6f;
    public float jumpHeight = 8f;
    public float gravity = 20f;
    public float maxVelocityChange = 50f;
    public TestTerrain world;

    private Collider collider;
    private Rigidbody rigidbody;

    void Awake() {
        SetStartPosition();
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
        rigidbody.useGravity = false;
        collider = GetComponent<Collider>();
    }

    void FixedUpdate() {
        if (!isActive) {
            return;
        }

        // Rotate the observer to the new normal
        Vector3 surfaceNormal = world.GetSurfaceNormal(transform.position);
        Quaternion rotation = Quaternion.FromToRotation(transform.up, surfaceNormal);
        transform.rotation = rotation * transform.rotation;

        // Player turn controls
        transform.Rotate(0, Input.GetAxis("Horizontal") * 100 * Time.deltaTime, 0, UnityEngine.Space.Self);

        if (isGrounded) {

            // Player move controls
            Vector3 targetVelocity = new Vector3(0, 0, Input.GetAxis("Vertical"));
            targetVelocity = transform.TransformDirection(targetVelocity);
            targetVelocity *= moveSpeed;

            Vector3 currentVelocity = rigidbody.velocity;
            Vector3 velocityChange = targetVelocity - currentVelocity;
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = Mathf.Clamp(velocityChange.y, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

            // Player jump controls
            if (Input.GetButton("Jump")) {
                rigidbody.velocity += transform.TransformDirection(new Vector3(0, CalculateJumpVerticalSpeed(), 0));
            }
        }

        // We apply gravity manually for more tuning control
        rigidbody.AddForce(gravity * rigidbody.mass * -surfaceNormal * Time.deltaTime);

        isGrounded = false;
    }

    void OnCollisionStay() {
        isGrounded = true;
    }

    private void SetStartPosition() {
        Vector3 targetStartPosition = transform.position;
        Vector3 surfaceNormal = world.GetSurfaceNormal(targetStartPosition);
        transform.position = world.GetHeightAtPosition(targetStartPosition) + surfaceNormal * transform.localScale.y * 1.1f;
    }

    // From the jump height and gravity we deduce the upwards speed 
    // for the character to reach at the apex.
    float CalculateJumpVerticalSpeed() {
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }
}
