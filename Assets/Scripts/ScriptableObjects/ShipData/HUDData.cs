using UnityEngine;

[CreateAssetMenu(fileName = "HUDData", menuName = "Scriptable Objects/HUDData")]
public class HUDData : ScriptableObject
{
    public int maxArrows = 4;
    public float arrowLoadingBarUpdateRate = 0.3f; // How many seconds it takes for a loading bar to switch the number of arrows yk
}
