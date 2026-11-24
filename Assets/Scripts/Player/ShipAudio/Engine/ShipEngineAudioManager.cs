using UnityEngine;
using FMODUnity;

public class ShipEngineAudioManager : MonoBehaviour
{
    [SerializeField] PlayerEngineAudio soundData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        PlayershipMovement.onKick += PlayKickSound;

        RuntimeManager.GetEventDescription(soundData.engineKickEvent).loadSampleData(); // Loads event into memory. If you don't do this, it reads from disk, and your super cool responsive kick sound actually takes like half an hour to arrive.
    }

    void PlayKickSound()
    {
        RuntimeManager.PlayOneShot(soundData.engineKickEvent, transform.position);
    }

    private void OnDisable()
    {
        PlayershipMovement.onKick -= PlayKickSound;
    }
}
