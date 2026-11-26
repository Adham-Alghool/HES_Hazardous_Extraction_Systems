using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class ShipEngineAudioManager : MonoBehaviour
{
    [SerializeField] PlayerEngineAudio soundData;
    [SerializeField] PlayerFlightModel flightModel;

    // Event references so that we can prepare and run looping audio clips
    EventInstance spoolInstance;
    EventInstance haltLoopInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        // Event Subscriptions
        PlayershipMovement.onKick += PlayKickSound;
        PlayershipMovement.onKick += EndSpool;

        PlayershipMovement.onSpool += StartSpool;
        PlayershipMovement.whileSpooling += RunSpool;

        PlayershipMovement.onIdle += EndSpool;
        PlayershipMovement.onIdle += EndHalt;

        PlayershipMovement.onHalt += StartHalt;
        PlayershipMovement.whileHalting += RunHalt;

        // Looping audio fetching
        spoolInstance = RuntimeManager.CreateInstance(soundData.engineSpoolEvent);
        haltLoopInstance = RuntimeManager.CreateInstance(soundData.engineHaltLoopEvent);

        // Loads event into memory. If you don't do this, it reads from disk, and your super cool responsive kick sound actually takes like half an hour to arrive.
        RuntimeManager.GetEventDescription(soundData.engineKickEvent).loadSampleData();
        RuntimeManager.GetEventDescription(soundData.engineSpoolEvent).loadSampleData();
        RuntimeManager.GetEventDescription(soundData.engineHaltLoopEvent).loadSampleData();
    }

    void StartSpool()
    {
        spoolInstance.setParameterByName("Spool", 0f, true);
        spoolInstance.start();
    }

    void RunSpool(float spoolProgress)
    {
        spoolInstance.setParameterByName("Spool", spoolProgress);
    }

    void EndSpool()
    {
        spoolInstance.stop(STOP_MODE.ALLOWFADEOUT);
    }

    void StartHalt(bool success)
    {
        if (success)
        {
            haltLoopInstance.setParameterByName("NormalizedSpeed", 1f, true);
            haltLoopInstance.start();
        }
    }

    void RunHalt(float speed)
    {
        float normalizedSpeed = Mathf.Min((speed / flightModel.overspeedCondition), 1f);
        haltLoopInstance.setParameterByName("NormalizedSpeed", normalizedSpeed);
    }

    void EndHalt()
    {
        haltLoopInstance.stop(STOP_MODE.ALLOWFADEOUT);
    }

    void PlayKickSound()
    {
        RuntimeManager.PlayOneShot(soundData.engineKickEvent, transform.position);
    }

    private void OnDisable()
    {
        PlayershipMovement.onKick -= PlayKickSound;
        PlayershipMovement.onKick -= EndSpool;

        PlayershipMovement.onSpool -= StartSpool;
        PlayershipMovement.whileSpooling -= RunSpool;
        PlayershipMovement.onIdle -= EndSpool;

        // Cleanup all instance garbage
        spoolInstance.release();
    }
}
