using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CarSelector : MonoBehaviour
{
    [Header("Input References")]
    [SerializeField] private InputActionAsset inputActions;
    
    [Header("UI References")]
    [SerializeField] private GameObject carUI; 

    private InputAction touchPositionAction;
    private InputAction touchPressAction;
    
    // NEW: Actions to read the Joystick/Brake
    private InputAction moveAction;
    private InputAction brakeAction;

    private Camera arCamera;
    private List<CarController> allCars = new List<CarController>();
    private CarController currentSelectedCar;

    void Awake()
    {
        arCamera = Camera.main;

        // 1. Setup Touch (for selecting cars)
        var uiMap = inputActions.FindActionMap("UI"); 
        if (uiMap == null) uiMap = inputActions.FindActionMap("Player");

        touchPositionAction = inputActions.FindAction("Point"); 
        touchPressAction = inputActions.FindAction("Click");
        
        // 2. Setup Driving (Reading the Joystick)
        // Ensure your Input Asset has a "Player" map with "Move" and "Brake"
        var playerMap = inputActions.FindActionMap("Player");
        moveAction = playerMap.FindAction("Move");
        brakeAction = playerMap.FindAction("Brake");
    }

    void OnEnable()
    {
        // Enable everything
        touchPositionAction.Enable();
        touchPressAction.Enable();
        moveAction.Enable();
        brakeAction.Enable();

        touchPressAction.performed += OnTouchPerformed;
    }

    void OnDisable()
    {
        touchPressAction.performed -= OnTouchPerformed;
        touchPositionAction.Disable();
        touchPressAction.Disable();
        moveAction.Disable();
        brakeAction.Disable();
    }

    // Runs every physics frame to drive the car
    void FixedUpdate()
    {
        if (currentSelectedCar != null)
        {
            // Read Joystick (Vector2)
            Vector2 input = moveAction.ReadValue<Vector2>();
            
            // Read Brake Button (Float 0 or 1)
            float brakeVal = brakeAction.ReadValue<float>();

            // Send to Car
            currentSelectedCar.SetMobileMove(input.y);   // Vertical is Gas
            currentSelectedCar.SetMobileSteer(input.x);  // Horizontal is Steer
            currentSelectedCar.SetMobileBrake(brakeVal > 0.5f);
        }
    }

    public void RegisterCar(GameObject carObj)
    {
        CarController controller = carObj.GetComponent<CarController>();
        if (controller != null)
        {
            allCars.Add(controller);
            controller.enabled = false; // Disable until selected
        }
    }

    private void OnTouchPerformed(InputAction.CallbackContext context)
    {
        Vector2 touchPos = touchPositionAction.ReadValue<Vector2>();
        Ray ray = arCamera.ScreenPointToRay(touchPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            CarController selectedCar = hit.collider.GetComponentInParent<CarController>();

            // Only select if we actually hit a car
            if (selectedCar != null)
            {
                SelectCar(selectedCar);
            }
        }
    }

    private void SelectCar(CarController car)
    {
        // Disable old cars
        foreach (var c in allCars)
        {
            c.enabled = false;
            c.SetMobileMove(0);
            c.SetMobileSteer(0);
            c.SetMobileBrake(true); 
        }

        // Enable new car
        currentSelectedCar = car;
        currentSelectedCar.enabled = true;
        currentSelectedCar.SetMobileBrake(false);

        Debug.Log($"Selected car: {car.name}");
        if(carUI != null) carUI.SetActive(true);
    }

        // ---------------------------------------------------------
    // PUBLIC METHODS FOR UI EVENT TRIGGERS (The Relay Station)
    // ---------------------------------------------------------

    // Link "Accelerate" and "Reverse" buttons here
    // Accelerate PointerDown: val = 1
    // Reverse PointerDown: val = -1
    // PointerUp: val = 0
    public void MobileMoveInput(float val)
    {
        Debug.Log("Mobile Move Input Received: " + val);
        if (currentSelectedCar != null)
        {
            currentSelectedCar.SetMobileMove(val);
        }
    }

    // Link "Left" and "Right" buttons here
    // Right PointerDown: val = 1
    // Left PointerDown: val = -1
    // PointerUp: val = 0
    public void MobileSteerInput(float val)
    {
        if (currentSelectedCar != null)
        {
            currentSelectedCar.SetMobileSteer(val);
        }
    }

    // Link "Brake" button here
    // PointerDown: state = true
    // PointerUp: state = false
    public void MobileBrakeInput(bool state)
    {
        if (currentSelectedCar != null)
        {
            currentSelectedCar.SetMobileBrake(state);
        }
    }
}