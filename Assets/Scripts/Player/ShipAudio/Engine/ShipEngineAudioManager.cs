using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class ShipEngineAudioManager : MonoBehaviour
{
    [SerializeField] PlayerEngineAudio soundData;
    [SerializeField] PlayerFlightModel flightModel;

    // Event references so that we can prepare and run looping audio clips
    EventInstance spoolInstance;
    EventInstance burnInstance;
    EventInstance haltLoopInstance;
    bool burnLoopActive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        // Event Subscriptions
        PlayershipMovement.onKick += PlayKickSound;
        PlayershipMovement.onKick += EndSpool;
        PlayershipMovement.onKick += StartBurnLoop;

        PlayershipMovement.onSpool += StartSpool;
        PlayershipMovement.whileSpooling += RunSpool;

        PlayershipMovement.onIdle += EndSpool;
        PlayershipMovement.onIdle += StopBurnLoop;
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

    void StartBurnLoop()
    {
        if (burnLoopActive)
        {
            return;
        }

        FMOD.Studio.EventDescription burnDescription = RuntimeManager.GetEventDescription(soundData.engineBurnEvent);
        FMOD.RESULT createResult = burnDescription.createInstance(out burnInstance);
        if (createResult != FMOD.RESULT.OK)
        {
            Debug.LogError($"Failed to create engine_burn FMOD event instance: {createResult}", this);
            if (burnInstance.isValid())
            {
                burnInstance.release();
            }
            burnInstance = default;
            return;
        }

        if (!burnInstance.isValid())
        {
            Debug.LogError("FMOD created an invalid engine_burn event instance.", this);
            burnInstance = default;
            return;
        }

        FMOD.RESULT startResult = burnInstance.start();
        if (startResult != FMOD.RESULT.OK)
        {
            Debug.LogError($"Failed to start engine_burn FMOD event: {startResult}", this);
            if (burnInstance.isValid())
            {
                FMOD.RESULT releaseResult = burnInstance.release();
                if (releaseResult != FMOD.RESULT.OK)
                {
                    Debug.LogError($"Failed to release engine_burn FMOD event instance: {releaseResult}", this);
                }
            }
            burnInstance = default;
            return;
        }

        burnLoopActive = true;
    }

    void StopBurnLoop()
    {
        if (!burnLoopActive)
        {
            return;
        }

        if (!burnInstance.isValid())
        {
            Debug.LogError("engine_burn FMOD event instance became invalid before stopping.", this);
            burnInstance = default;
            burnLoopActive = false;
            return;
        }

        FMOD.RESULT stopResult = burnInstance.stop(STOP_MODE.ALLOWFADEOUT);
        if (stopResult != FMOD.RESULT.OK)
        {
            Debug.LogError($"Failed to stop engine_burn FMOD event: {stopResult}", this);
        }

        FMOD.RESULT releaseResult = burnInstance.release();
        if (releaseResult != FMOD.RESULT.OK)
        {
            Debug.LogError($"Failed to release engine_burn FMOD event instance: {releaseResult}", this);
        }

        burnInstance = default;
        burnLoopActive = false;
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
        PlayershipMovement.onKick -= StartBurnLoop;

        PlayershipMovement.onSpool -= StartSpool;
        PlayershipMovement.whileSpooling -= RunSpool;
        PlayershipMovement.onIdle -= EndSpool;
        PlayershipMovement.onIdle -= StopBurnLoop;
        PlayershipMovement.onIdle -= EndHalt;

        PlayershipMovement.onHalt -= StartHalt;
        PlayershipMovement.whileHalting -= RunHalt;

        // Cleanup all instance garbage
        spoolInstance.stop(STOP_MODE.IMMEDIATE);
        spoolInstance.release();
        if (burnInstance.isValid())
        {
            burnInstance.stop(STOP_MODE.IMMEDIATE);
            burnInstance.release();
        }
        burnLoopActive = false;
        haltLoopInstance.stop(STOP_MODE.IMMEDIATE);
        haltLoopInstance.release();
    }
}
// test push
// test push 237469128746