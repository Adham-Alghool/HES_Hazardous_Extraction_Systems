using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [SerializeField] TextMeshPro SpeedIndicator;
    [SerializeField] TextMeshPro EngineIndicator;

    private GameManager gameManager;

    float ArrowLoadingBarTimer = 0f;
    int ArrowLoadingBarCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void OnEnable()
    {
        PlayershipMovement.onIdle += ShowIdle;
        PlayershipMovement.whileSpooling += ShowSpool;
        PlayershipMovement.whileBurning += ShowBurn;
        PlayershipMovement.whileHalting += ShowHalting;
    }

    // Update is called once per frame
    void Update()
    {
        float speed = Mathf.Round(gameManager.speed/10)*10;
        SpeedIndicator.text = speed + " M/S";
    }

    void ShowIdle()
    {
        EngineIndicator.text = "IDLE";
    }

    void ShowSpool(float progress)
    {
        EngineIndicator.text = ArrowProgressBar(progress, 3, "SPOOLING");
    }

    void ShowBurn()
    {
        EngineIndicator.text = "BURNING";
    }

    void ShowHalting(float disposeOfMe) // Pass this float in so our event thing stays happy. We COULD use it for a progress bar tho.
    {
        EngineIndicator.text = ArrowLoadingBar("HALTING", 3, 0.33f);
    }

    string ArrowProgressBar(float progressNormalized, int maxIncrements, string Message) // This is used to show a progress bar. Good for stuff like spooling the engine.
    {
        int incrementsToUse = Mathf.FloorToInt(progressNormalized * maxIncrements);

        string startOfMessage = new string('>', incrementsToUse);
        string endOfMessage = new string('<', incrementsToUse);

        return startOfMessage + Message + endOfMessage;
    }
    string ArrowLoadingBar(string message, int maxArrows, float updateRate)
    {
        ArrowLoadingBarTimer += Time.deltaTime;

        if (ArrowLoadingBarTimer >= updateRate)
        {
            ArrowLoadingBarTimer = 0f;
            ArrowLoadingBarCount++;

            if (ArrowLoadingBarCount > maxArrows) ArrowLoadingBarCount = 0;
        }

        string left = new string('>', ArrowLoadingBarCount);
        string right = new string('<', ArrowLoadingBarCount);
        return left + message + right;
    }

    private void OnDisable()
    {
        PlayershipMovement.onIdle -= ShowIdle;
        PlayershipMovement.whileSpooling -= ShowSpool;
        PlayershipMovement.whileBurning -= ShowBurn;
        PlayershipMovement.whileHalting -= ShowHalting;
    }
}
