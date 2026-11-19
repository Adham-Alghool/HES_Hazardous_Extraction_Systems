using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Rigidbody player;

    // Kinematics

    [HideInInspector] public float speed;
    [HideInInspector] public float acceleration;
    [HideInInspector] public Vector3 velocity = Vector3.zero;
    [HideInInspector] public Vector3 accelerationVector = Vector3.zero;

    private float speed_last_frame = 0f;
    private Vector3 velocity_last_frame = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        velocity_last_frame = player.linearVelocity;
        speed_last_frame = velocity_last_frame.magnitude;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        AdjustKinematics();
    }

    void AdjustKinematics()
    {
        velocity = player.linearVelocity;
        speed = velocity.magnitude;

        accelerationVector = (velocity - velocity_last_frame) / Time.fixedDeltaTime;

        acceleration = (speed - speed_last_frame) / Time.fixedDeltaTime;

        velocity_last_frame = velocity;
        speed_last_frame = speed;
    }
}
