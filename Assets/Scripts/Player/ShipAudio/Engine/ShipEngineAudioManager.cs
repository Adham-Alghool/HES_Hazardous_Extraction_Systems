using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class ShipEngineAudioManager : MonoBehaviour
{
    [SerializeField] PlayerEngineAudio soundData;

    // Event references so that we can prepare and run looping audio clips
    EventInstance spoolInstance;
    EventInstance burnInstance;
    EventInstance idleInstance;
    EventInstance haltLoopInstance;
    bool burnLoopActive;
    bool idleLoopActive;
    bool idleLoopStartAttempted;
    bool haltLoopActive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        // Event Subscriptions
        PlayershipMovement.onKick += PlayKickSound;
        PlayershipMovement.onKick += EndSpool;
        PlayershipMovement.onKick += StartBurnLoop;

        PlayershipMovement.onSpool += StartSpool;
        PlayershipMovement.onSpool += StopIdleLoop;
        PlayershipMovement.whileSpooling += RunSpool;
        PlayershipMovement.whileIdling += StartIdleLoop;

        PlayershipMovement.onIdle += EndSpool;
        PlayershipMovement.onIdle += StopBurnLoop;
        PlayershipMovement.onIdle += StopHaltLoop;

        PlayershipMovement.onHalt += StartHalt;
        PlayershipMovement.onSpool += StopHaltLoop;

        // Looping audio fetching
        spoolInstance = RuntimeManager.CreateInstance(soundData.engineSpoolEvent);

        // Loads event into memory. If you don't do this, it reads from disk, and your super cool responsive kick sound actually takes like half an hour to arrive.
        RuntimeManager.GetEventDescription(soundData.engineKickEvent).loadSampleData();
        RuntimeManager.GetEventDescription(soundData.engineSpoolEvent).loadSampleData();
        RuntimeManager.GetEventDescription(soundData.engineHaltLoopEvent).loadSampleData();
    }

    void StartSpool()
    {
        RuntimeManager.PlayOneShot(soundData.engineSpoolOneShotEvent, transform.position);
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

    void StartIdleLoop()
    {
        if (idleLoopActive || idleLoopStartAttempted)
        {
            return;
        }

        idleLoopStartAttempted = true;
        FMOD.Studio.EventDescription idleDescription;
        try
        {
            idleDescription = RuntimeManager.GetEventDescription(soundData.engineIdleEvent);
        }
        catch (EventNotFoundException exception)
        {
            Debug.LogError($"Failed to find engine_idle FMOD event. Ensure its bank is built and loaded. {exception.Message}", this);
            return;
        }

        FMOD.RESULT createResult = idleDescription.createInstance(out idleInstance);
        if (createResult != FMOD.RESULT.OK)
        {
            Debug.LogError($"Failed to create engine_idle FMOD event instance: {createResult}", this);
            if (idleInstance.isValid())
            {
                idleInstance.release();
            }
            idleInstance = default;
            return;
        }

        if (!idleInstance.isValid())
        {
            Debug.LogError("FMOD created an invalid engine_idle event instance.", this);
            idleInstance = default;
            return;
        }

        FMOD.RESULT startResult = idleInstance.start();
        if (startResult != FMOD.RESULT.OK)
        {
            Debug.LogError($"Failed to start engine_idle FMOD event: {startResult}", this);
            if (idleInstance.isValid())
            {
                idleInstance.release();
            }
            idleInstance = default;
            return;
        }

        idleLoopActive = true;
    }

    void StopIdleLoop()
    {
        if (!idleLoopActive)
        {
            idleLoopStartAttempted = false;
            return;
        }

        if (!idleInstance.isValid())
        {
            Debug.LogError("engine_idle FMOD event instance became invalid before stopping.", this);
            idleInstance = default;
            idleLoopActive = false;
            idleLoopStartAttempted = false;
            return;
        }

        FMOD.RESULT stopResult = idleInstance.stop(STOP_MODE.ALLOWFADEOUT);
        if (stopResult != FMOD.RESULT.OK)
        {
            Debug.LogError($"Failed to stop engine_idle FMOD event: {stopResult}", this);
        }

        FMOD.RESULT releaseResult = idleInstance.release();
        if (releaseResult != FMOD.RESULT.OK)
        {
            Debug.LogError($"Failed to release engine_idle FMOD event instance: {releaseResult}", this);
        }

        idleInstance = default;
        idleLoopActive = false;
        idleLoopStartAttempted = false;
    }

    void StartHalt(bool success)
    {
        if (!success || haltLoopActive)
        {
            return;
        }

        RuntimeManager.PlayOneShot(soundData.engineHaltEvent, transform.position);
        StopIdleLoop();

        FMOD.Studio.EventDescription haltDescription;
        try
        {
            haltDescription = RuntimeManager.GetEventDescription(soundData.engineHaltLoopEvent);
        }
        catch (EventNotFoundException exception)
        {
            Debug.LogError($"Failed to find engine_halt_loop FMOD event. Ensure its bank is built and loaded. {exception.Message}", this);
            return;
        }

        FMOD.RESULT createResult = haltDescription.createInstance(out haltLoopInstance);
        if (createResult != FMOD.RESULT.OK)
        {
            Debug.LogError($"Failed to create engine_halt_loop FMOD event instance: {createResult}", this);
            if (haltLoopInstance.isValid())
            {
                haltLoopInstance.release();
            }
            haltLoopInstance = default;
            return;
        }

        if (!haltLoopInstance.isValid())
        {
            Debug.LogError("FMOD created an invalid engine_halt_loop event instance.", this);
            haltLoopInstance = default;
            return;
        }

        FMOD.RESULT startResult = haltLoopInstance.start();
        if (startResult != FMOD.RESULT.OK)
        {
            Debug.LogError($"Failed to start engine_halt_loop FMOD event: {startResult}", this);
            if (haltLoopInstance.isValid())
            {
                haltLoopInstance.release();
            }
            haltLoopInstance = default;
            return;
        }

        haltLoopActive = true;
    }

    void StopHaltLoop()
    {
        if (!haltLoopActive)
        {
            return;
        }

        if (!haltLoopInstance.isValid())
        {
            Debug.LogError("engine_halt_loop FMOD event instance became invalid before stopping.", this);
            haltLoopInstance = default;
            haltLoopActive = false;
            return;
        }

        FMOD.RESULT stopResult = haltLoopInstance.stop(STOP_MODE.ALLOWFADEOUT);
        if (stopResult != FMOD.RESULT.OK)
        {
            Debug.LogError($"Failed to stop engine_halt_loop FMOD event: {stopResult}", this);
        }

        FMOD.RESULT releaseResult = haltLoopInstance.release();
        if (releaseResult != FMOD.RESULT.OK)
        {
            Debug.LogError($"Failed to release engine_halt_loop FMOD event instance: {releaseResult}", this);
        }

        haltLoopInstance = default;
        haltLoopActive = false;
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
        PlayershipMovement.onSpool -= StopIdleLoop;
        PlayershipMovement.whileSpooling -= RunSpool;
        PlayershipMovement.whileIdling -= StartIdleLoop;
        PlayershipMovement.onIdle -= EndSpool;
        PlayershipMovement.onIdle -= StopBurnLoop;
        PlayershipMovement.onIdle -= StopHaltLoop;

        PlayershipMovement.onHalt -= StartHalt;
        PlayershipMovement.onSpool -= StopHaltLoop;

        // Cleanup all instance garbage
        spoolInstance.stop(STOP_MODE.IMMEDIATE);
        spoolInstance.release();
        if (burnInstance.isValid())
        {
            burnInstance.stop(STOP_MODE.IMMEDIATE);
            burnInstance.release();
        }
        burnLoopActive = false;
        if (idleInstance.isValid())
        {
            idleInstance.stop(STOP_MODE.IMMEDIATE);
            idleInstance.release();
        }
        idleLoopActive = false;
        idleLoopStartAttempted = false;
        if (haltLoopInstance.isValid())
        {
            haltLoopInstance.stop(STOP_MODE.IMMEDIATE);
            haltLoopInstance.release();
        }
        haltLoopActive = false;
    }
}
// test push
// test push 237469128746