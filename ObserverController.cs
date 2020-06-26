using UnityEngine;
using System.Collections;

public class ObserverController : MonoBehaviour {
    public bool isActive = false;
    public bool isGrounded = false;
    public float moveSpeed = 6f;
    public float jumpHeight = 0.001f;
    public float gravity = 20f;
    public float maxVelocityChange = 50f;
    public TestTerrain world;
    public bool jetPackMode = false;

    private Collider collider;
    private Rigidbody rigidbody;
    private Camera camera;

    void Awake() {
        SetStartPosition();
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
        rigidbody.useGravity = false;
        collider = GetComponent<Collider>();
        camera = GetComponentInChildren<Camera>();
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

        if (isGrounded || jetPackMode) {
            // Player move controls
            Vector3 targetVelocity = new Vector3(0, 0, Input.GetAxis("Vertical"));
            targetVelocity = transform.TransformDirection(targetVelocity);
            targetVelocity *= moveSpeed;

            Vector3 currentVelocity = rigidbody.velocity;
            Vector3 velocityChange = targetVelocity - currentVelocity;
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = Mathf.Clamp(velocityChange.y, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            // Player jump controls
            if (Input.GetButton("Jump")) {
                velocityChange += transform.TransformDirection(new Vector3(0, CalculateJumpVerticalSpeed(), 0));
            }
            rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
            

        }

        // We apply gravity manually for more tuning control
        rigidbody.AddForce(gravity * rigidbody.mass * -surfaceNormal * Time.deltaTime);

        isGrounded = false;

        // Camera controls
        float altitudeRatio = (world.GetDistanceFromSurface(transform.position) - 20f) / 100f;
        Vector3 newRotation = camera.transform.localRotation.eulerAngles;
        newRotation.x = Mathf.Lerp(15f, 90f, Mathf.Clamp(altitudeRatio, 0f, 1f));
        camera.transform.localRotation = Quaternion.Euler(newRotation);
        camera.transform.localPosition = new Vector3(camera.transform.localPosition.x, camera.transform.localPosition.y, Mathf.Lerp(-20f, 0f, altitudeRatio));
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
        return Mathf.Sqrt(2 * jumpHeight * gravity * (jetPackMode ? 0.05f : 1f));
    }
}
