using UnityEngine;

[CreateAssetMenu(fileName = "PlayerFlightModel", menuName = "Scriptable Objects/FlightModel")]
public class PlayerFlightModel : ScriptableObject
{
    public float engineSpoolTime = 0.8f;
    public float acceleration = 15f;
    public float kickForce = 80f;
    public float angularForce = 8f;
    public float angularMaxSpeed = 10f;
    public float overspeedCondition = 200f;
    public float decelerationForce = 80f;
}
