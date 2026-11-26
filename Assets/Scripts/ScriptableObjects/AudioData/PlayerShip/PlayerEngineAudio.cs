using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "PlayerEngineAudio", menuName = "Scriptable Objects/PlayerEngineAudio")]
public class PlayerEngineAudio : ScriptableObject
{
    public EventReference engineKickEvent;
    public EventReference engineSpoolEvent;
    public EventReference engineHaltLoopEvent;
}
