using UnityEngine;
using Unity.Cinemachine;


public class CameraShake : MonoBehaviour
{
    [SerializeField] CinemachineCamera cockpitCamera;

    [Header("Impulse Sources")]
    [SerializeField] CinemachineImpulseSource kickImpulse;
    [SerializeField] CinemachineImpulseSource haltImpulse;

    [Header("Speed Rumble Effects")]
    [Tooltip("The max speed at which the camera's max speed-related shake will take place")]
    [SerializeField] private float maxSpeedCap = 250f;
    [SerializeField] private float maxAmplitude = 2f;

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

    [SerializeField] float maxHaltShakeSpeed = 1000f;
    [SerializeField] float maxHaltShakeStrength = 3f;

    void Halt(bool successful)
    {
        if (!successful || game.speed < 20f) return; // No reason to halt effect

        float intensity = Mathf.InverseLerp(20f, maxHaltShakeSpeed, game.speed);
        haltImpulse.GenerateImpulseWithForce(intensity * maxHaltShakeStrength);
    }
}
