using System;
using UnityEngine;
using UnityEngine.XR;

public class PlayershipMovement : MonoBehaviour
{
    InputBase input;
    [SerializeField] PlayerFlightModel flightModel;
    
    enum burnerStatus {spooling, kicking, burning, halting, idling}
    burnerStatus currentStatus = burnerStatus.idling;

    private Rigidbody rb;

    float spoolingTimer = 0;

    // Public events for audiovisual changes and whatnot

    public static event Action onIdle;
    public static event Action onSpool;
    public static event Action<float> whileSpooling; // Feed in spool progress.
    public static event Action onKick;
    public static event Action whileBurning; // We're skipping on the 'On Burning' event because practically, the kick and the start of the burn are the same thing.
    public static event Action<bool> onHalt; // Feed in whether or not halt request was successful.
    public static event Action<float> whileHalting; // Feed in velocity here.
    public static event Action finishedHalting;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.inertiaTensorRotation = Quaternion.identity;
    }

    private void OnEnable()
    {
        input = InputBase.Instance;
        input.halt += InitiateHalt;
    }

    void FixedUpdate()
    {
        switch (currentStatus)
        {
            case burnerStatus.idling:
                handleIdling();
                break;
            case burnerStatus.spooling:
                handleSpooling();
                break;
            case burnerStatus.kicking:
                handleKick();
                break;
            case burnerStatus.burning:
                HandleBurn();
                break;
            case burnerStatus.halting:
                handleHalting();
                break;
            default:
                Debug.LogError("BURNER STATUS CASE NOT CONSIDERED");
                break;
        }

        HandleRotation();
    }
    
    void handleIdling()
    {
        if (input.throttle == 0)
        {
            // CONTINUOUS BLOCK
            return;
        }
        else
        {
            // DISCRETE BLOCK
            onSpool?.Invoke();
            currentStatus = burnerStatus.spooling;
        }
    }

    void handleSpooling()
    {
        float spoolProgress = spoolingTimer / flightModel.engineSpoolTime;
        whileSpooling?.Invoke(spoolProgress);

        if (input.throttle > 0)
        {
            // CONTINUOUS BLOCK
            spoolingTimer += Time.deltaTime;
            // DISCRETE BLOCK
            if (spoolingTimer >= flightModel.engineSpoolTime)
            {
                currentStatus = burnerStatus.kicking;
                spoolingTimer = 0;
            }
        }
        else
        {
            // CONTINUOUS BLOCK
            spoolingTimer -= Time.deltaTime;
            // DISCRETE BLOCK
            if (spoolingTimer <= 0)
            {
                onIdle?.Invoke();
                currentStatus = burnerStatus.idling;
                spoolingTimer = 0;
            }
        }
    }

    void handleKick()
    {
        onKick?.Invoke();

        Vector3 engineKickForce = transform.forward * flightModel.kickForce;
        rb.AddForce(engineKickForce, ForceMode.Impulse);
        currentStatus = burnerStatus.burning;
    }

    void HandleBurn()
    {
        whileBurning?.Invoke();

        if (input.throttle > 0)
        {
            Vector3 burnForce = transform.forward * flightModel.acceleration * input.throttle;
            rb.AddForce(burnForce);
        }
        else
        {
            currentStatus = burnerStatus.idling;
            onIdle?.Invoke();
        }
    }

    void InitiateHalt()
    {
        // DISCRETE BLOCK
        if (input.throttle > 0)
        {
            Debug.LogError("CANNOT HALT WHILE THROTTLE IS APPLIED");
            onHalt?.Invoke(false);
            return; // CANNOT HALT WHILE THROTTLE APPLIED.
        }
        if (currentStatus == burnerStatus.halting)
        {
            return; // No error message really needed. Halt is already happening.
        }
        onHalt?.Invoke(true);
        currentStatus = burnerStatus.halting;
    }

    void handleHalting()
    {
        if (input.throttle > 0) // EXIT CONDITION #1
        {
            // DISCRETE BLOCK
            onSpool?.Invoke();
            currentStatus = burnerStatus.spooling;
        }
        else
        {
            if (rb.linearVelocity.magnitude < 0.05f) // EXIT CONDITION #2
            {
                // DISCRETE BLOCK
                rb.linearVelocity = Vector3.zero;
                currentStatus = burnerStatus.idling;
                finishedHalting?.Invoke();
                onIdle?.Invoke();
                return;
            }
            // CONTINUOUS BLOCK
            Vector3 desiredAcceleration = -rb.linearVelocity * rb.mass / Time.fixedDeltaTime;
            Vector3 brakingAcceleration = Vector3.ClampMagnitude(desiredAcceleration, flightModel.decelerationForce);
            rb.AddForce(brakingAcceleration);
            whileHalting?.Invoke(rb.linearVelocity.magnitude);
        }
    }

    void HandleRotation()
    {
        rb.maxAngularVelocity = flightModel.angularMaxSpeed;

        float pitchForce = input.rotate.y * flightModel.angularForce;
        float rollForce = -input.rotate.x * flightModel.angularForce;

        Vector3 RotationalForce = new Vector3(pitchForce, 0f, rollForce);

        rb.AddRelativeTorque(RotationalForce);
    }

    private void OnDestroy()
    {
        input.halt -= InitiateHalt;
    }
    private void OnDisable()
    {
        input.halt -= InitiateHalt;
    }
}
