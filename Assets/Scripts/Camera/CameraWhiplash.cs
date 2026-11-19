using UnityEngine;

public class CameraWhiplash : MonoBehaviour
{
    // Yes, AI "heled" (wrote). No, I'm not sorry. This would've been a huge pain.
    [SerializeField] Transform whiplashTarget;

    [Header("Neck")]
    [Tooltip("How hard your neck pulls your head back to center")]
    [SerializeField] float stiffness = 80f;
    [Tooltip("Wobbliness changer")]
    [SerializeField] float damping = 12f;
    [Tooltip("Decrease if you don't skip neck day.")]
    [SerializeField] float inertia = 0.25f;
    [Tooltip("How much the player's neck can move every second")]
    [SerializeField] float maxHeadSpeed = 3f;

    [Header("Head Tilt")]
    [SerializeField] float tiltPerMeter = 50f;

    GameManager game;

    Vector3 lastShipVelocity;

    Vector3 headOffset; // Where the head is compared to rest, in meters, in ship space
    Vector3 headVelocity;

    void Start()
    {
        game = GameManager.Instance;
        lastShipVelocity = game.velocity;
    }

    // Update, not FixedUpdate: the head moves every rendered frame, so no stutter.
    // Not LateUpdate either: we need to be done before Cinemachine reads the target.
    void Update()
    {
        // How much the ship's velocity changed since last frame, in the ship's own axes.
        Vector3 velocityChange = game.velocity - lastShipVelocity;
        velocityChange = whiplashTarget.parent.InverseTransformDirection(velocityChange);
        lastShipVelocity = game.velocity;

        // Your body moves with the ship but your head lags behind, so it gets thrown the opposite way.
        headVelocity -= velocityChange * inertia;

        // The neck pulls the head back to center, damping calms it down.
        headVelocity += (-stiffness * headOffset - damping * headVelocity) * Time.deltaTime;
        headVelocity = Vector3.ClampMagnitude(headVelocity, maxHeadSpeed);

        headOffset += headVelocity * Time.deltaTime;

        MoveHead();
    }

    void MoveHead()
    {
        // Thrown back = look up. Thrown down = chin to chest. Thrown sideways = tilt that way.
        float pitch = (headOffset.z - headOffset.y) * tiltPerMeter;
        float roll = -headOffset.x * tiltPerMeter;

        whiplashTarget.localPosition = headOffset;
        whiplashTarget.localRotation = Quaternion.Euler(pitch, 0f, roll);
    }
}
