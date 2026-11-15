using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputBase : MonoBehaviour
{  
    // Set up our singleton so we need 0 references.
    public static InputBase Instance { get; private set; }

    [SerializeField] private InputActionAsset inputActions;

    InputAction Rotate;
    InputAction Throttle;
    InputAction Halt;

    public Vector2 rotate { get; private set; }
    public float throttle { get; private set; }
    [HideInInspector] public event Action halt; 

    private void Awake()
    {
        SetupSingleton();
    }

    private void OnEnable()
    {
        SetupSingleton();
    }

    void Start()
    {
        var map = inputActions.FindActionMap("Player");
        Rotate = map.FindAction("Rotate");
        Throttle = map.FindAction("Throttle");
        Halt = map.FindAction("Halt");

        map.Enable();
    }
    void Update()
    {
        rotate = Rotate.ReadValue<Vector2>();
        throttle = Mathf.Max(0, Throttle.ReadValue<float>());
        if(Halt.WasPressedThisFrame())
        {
            halt?.Invoke();
        }
    }

    void SetupSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void DismantleSingleton()
    {
        if (Instance == this) Instance = null;
    }

    private void OnDestroy()
    {
        DismantleSingleton();
    }

    private void OnDisable()
    {
        DismantleSingleton();
    }
}
