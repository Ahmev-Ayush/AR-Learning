using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using UnityEngine.InputSystem; 
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch; 

public class placeDrivingCarInScene : MonoBehaviour
{
    public GameObject carPrefab;
    public GameObject placementIndicator;
    
    // NEW: Reference to the selector script
    public CarSelector carSelector; 

    [SerializeField] private ARRaycastManager raycastManager;
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Start()
    {
        raycastManager = FindFirstObjectByType<ARRaycastManager>();
        
        // NEW: Auto-find the selector if you forgot to drag it in
        if (carSelector == null) carSelector = FindFirstObjectByType<CarSelector>();
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (placementPoseIsValid && (IsTouchInput() || IsMouseInput()) && ARModeController.IsPlacementAllowed)
        {
            PlaceCar();
        }
    }

    private bool IsTouchInput()
    {
        return Touch.activeTouches.Count > 0 && 
               Touch.activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Began;
    }

    private bool IsMouseInput()
    {
        return Mouse.current != null && 
               Mouse.current.leftButton.wasPressedThisFrame;
    }

    private void PlaceCar()
    {
        GameObject newCar = Instantiate(carPrefab, placementPose.position, placementPose.rotation);
        
        // NEW: Tell the selector about this new car
        if (carSelector != null)
        {
            carSelector.RegisterCar(newCar);
        }
    }

    private void UpdatePlacementPose()
    {
        if (raycastManager == null || Camera.main == null) return;

        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();

        raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon);

        placementPoseIsValid = hits.Count > 0;
        
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;
            var cameraForward = Camera.main.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }
}