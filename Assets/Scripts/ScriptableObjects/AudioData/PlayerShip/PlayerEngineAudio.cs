using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "PlayerEngineAudio", menuName = "Scriptable Objects/PlayerEngineAudio")]
public class PlayerEngineAudio : ScriptableObject
{
    public EventReference engineKickEvent;
    public EventReference engineBurnEvent;
    public EventReference engineIdleEvent;
    public EventReference engineHaltEvent;
    public EventReference engineSpoolEvent;
    public EventReference engineSpoolOneShotEvent;
    public EventReference engineHaltLoopEvent;
}
