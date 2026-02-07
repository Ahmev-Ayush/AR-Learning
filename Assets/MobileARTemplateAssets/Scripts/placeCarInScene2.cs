using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
// 1. Essential Input System namespaces
using UnityEngine.InputSystem; 
using UnityEngine.InputSystem.EnhancedTouch;
// 2. Alias for Touch to avoid ambiguity
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch; 
using UnityEngine.EventSystems; // For UI interaction checks


public class placeCarInScene : MonoBehaviour
{
    public GameObject carPrefab;
    public GameObject placementIndicator;

    // We only need the ARRaycastManager, not the whole XROrigin component
    [SerializeField] private ARRaycastManager raycastManager;
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    private void OnEnable()
    {
        // Enable EnhancedTouch for mobile support
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Start()
    {
        // Find the ARRaycastManager in the scene
        raycastManager = FindFirstObjectByType<ARRaycastManager>();

        // DEBUGGING: If this message appears in Console, your AR Session Origin is missing the Raycast Manager script
        if (raycastManager == null)
        {
            Debug.LogError("Error: ARRaycastManager not found! Please add the 'AR Raycast Manager' component to your XR Origin.");
        }
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        // 3. Logic: If position is valid AND (User touched screen OR User clicked mouse)
        if (placementPoseIsValid && (IsTouchInput() || IsMouseInput()) && ARModeController.IsPlacementAllowed)
        {
            PlaceCar();
        }
    }

    // Helper to check for Mobile Touch
    private bool IsTouchInput()
    {
        return Touch.activeTouches.Count > 0 && 
               Touch.activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Began && !IsPointerOverUI(Touch.activeTouches[0]);
    }

    // Helper to check for Mouse Click (Works in Editor)
    // private bool IsMouseInput()
    // {
    //     return Mouse.current != null && 
    //            Mouse.current.leftButton.wasPressedThisFrame ;
    // }

    private bool IsMouseInput()
{
    // 1. Safety check for the mouse device
    if (Mouse.current == null) return false;

    // 2. Check if the button was actually pressed
    bool clicked = Mouse.current.leftButton.wasPressedThisFrame;

    // 3. Check if the mouse is currently over a UI element (EventSystem)
    // If it IS over UI, we return false because we don't want to trigger "game" input.
    bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

    return clicked && !overUI;
}

    private void PlaceCar()
    {
        Instantiate(carPrefab, placementPose.position, placementPose.rotation);
    }

    private void UpdatePlacementPose()
    {
        if (raycastManager == null || Camera.main == null) return;

        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();

        // 4. Raycast using the manager
        // TrackableType.PlaneWithinPolygon ensures we only hit the visible part of the plane
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

        private bool IsPointerOverUI(Touch touch)
    {
        // Check if the touch is over a UI element
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.finger.index);
    }
}