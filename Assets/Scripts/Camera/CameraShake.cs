using Unity.Cinemachine;
using UnityEngine;


public class CameraShake : MonoBehaviour
{
    [SerializeField] CinemachineCamera cockpitCamera;

    [Header("Impulse Sources")]
    [SerializeField] CinemachineImpulseSource kickImpulse;
    [SerializeField] CinemachineImpulseSource haltImpulse;

    [Header("High Speed Effects")]
    [Tooltip("The max speed at which the camera's max speed-related shake will take place")]
    [SerializeField] float maxSpeedCap = 250f;
    [Tooltip("Self explanatory. Are you stupid or something?")]
    [SerializeField] float maxAmplitude = 2f;

    [Header("Halt Initiation Effects")]
    [Tooltip("The maximum speed at which maximum shake should be used")]
    [SerializeField] float maxHaltShakeSpeed = 1000f;
    [Tooltip("You are REALLY stupid")]
    [SerializeField] float maxHaltShakeStrength = 3f;

    private CinemachineBasicMultiChannelPerlin baseShake;

    GameManager game;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        baseShake = cockpitCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        game = GameManager.Instance;
    }

    void OnEnable()
    {
        PlayershipMovement.onKick += Kick;
        PlayershipMovement.onHalt += Halt;
    }

    void OnDisable()
    {
        PlayershipMovement.onKick -= Kick;
        PlayershipMovement.onHalt -= Halt;
    }

    // Update is called once per frame
    void Update()
    {
        float intensity = Mathf.InverseLerp(0f, maxSpeedCap, game.speed);
        baseShake.AmplitudeGain = intensity * maxAmplitude;
    }

    void Kick()
    {
        kickImpulse.GenerateImpulseWithForce(1f);
    }

    void Halt(bool successful)
    {
        if (!successful || game.speed < 20f) return; // No reason to halt effect

        float intensity = Mathf.InverseLerp(20f, maxHaltShakeSpeed, game.speed);
        haltImpulse.GenerateImpulseWithForce(intensity * maxHaltShakeStrength);
    }
}
