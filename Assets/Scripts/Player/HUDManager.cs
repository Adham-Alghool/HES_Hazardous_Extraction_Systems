using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [SerializeField] TextMeshPro SpeedIndicator;

    private GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        float speed = Mathf.Round(gameManager.speed/10)*10;
        SpeedIndicator.text = speed + " M/S";
    }
}
