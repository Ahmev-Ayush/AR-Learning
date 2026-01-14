using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
// Required for the New Input System
using UnityEngine.InputSystem.EnhancedTouch; 
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch; 

public class PlaceCarInScene : MonoBehaviour
{
    [Header("Assets")]
    public GameObject carPrefab;
    public GameObject placementIndicator;

    // We do NOT need XROrigin here, we only need the RaycastManager
    private ARRaycastManager raycastManager;
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    private void OnEnable()
    {
        // 1. Enable Enhanced Touch
        EnhancedTouchSupport.Enable();
        // 2. Subscribe to the tap event
        Touch.onFingerDown += OnFingerDown;
    }

    private void OnDisable()
    {
        Touch.onFingerDown -= OnFingerDown;
        EnhancedTouchSupport.Disable();
    }

    void Start()
    {
        // 3. Find the ARRaycastManager in the scene
        raycastManager = FindFirstObjectByType<ARRaycastManager>();

        if (raycastManager == null)
        {
            Debug.LogError("CRITICAL ERROR: ARRaycastManager not found! Please add the 'AR Raycast Manager' component to your XR Origin game object.");
        }
        
        // Hide indicator at start
        placementIndicator.SetActive(false);
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
    }

    // This runs automatically when you tap the screen
    private void OnFingerDown(Finger finger)
    {
        // Only place if we have a valid plane hit
        if (placementPoseIsValid)
        {
            PlaceCar();
        }
        else
        {
            Debug.Log("Tap detected, but no valid placement pose found (Scan the floor more).");
        }
    }

    private void PlaceCar()
    {
        Debug.Log($"Placing Car at {placementPose.position}");
        Instantiate(carPrefab, placementPose.position, placementPose.rotation);
    }

    private void UpdatePlacementPose()
    {
        if (raycastManager == null || Camera.main == null) return;

        // Shoot a ray from the center of the screen
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();

        // Raycast against detected planes
        raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon);

        placementPoseIsValid = hits.Count > 0;

        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            // Calculate rotation to face the camera but stay flat
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