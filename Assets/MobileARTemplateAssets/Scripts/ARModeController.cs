using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem; // REQUIRED FOR MOUSE
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class ARModeController : MonoBehaviour
{
    [Header("AR Components")]
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private ARRaycastManager raycastManager;

    // Global state check for our OTHER scripts
    public static bool IsPlacementAllowed = true;

    // Internal states
    private bool isPlaneDetectionOn = true;
    private bool isPlaneDeletionMode = false;
    private bool isObjectRemovalMode = false;

    // Layer Masks
    private int planeLayerMask;
    private int objectLayerMask;

    private void Awake()
    {
        // Define Layer (make sure these matches your Unity Tags/Layers)
        planeLayerMask = 1 << LayerMask.NameToLayer("ARPlanes");
        objectLayerMask = 1 << LayerMask.NameToLayer("PlacedObjects");
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        // LINK TOUCH: Pass the finger's position to our generic handler
        Touch.onFingerDown += (finger) => HandleInput(finger.screenPosition);
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        Touch.onFingerDown -= (finger) => HandleInput(finger.screenPosition);
    }

    // LINK MOUSE: Check for mouse clicks every frame (For Editor Testing)
    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleInput(Mouse.current.position.ReadValue());
        }
    }

    // Public functions for the UI TOGGLES ---

    // Toggle 1 (Detection of Plane)
    public void SetPlaneDetection(bool isOn)
    {
        isPlaneDetectionOn = isOn;
        planeManager.enabled = isOn;

        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(isOn);
        }
    }

    // Toggle 2 (Delete Plane)
    public void SetPlaneDeletionMode(bool isOn)
    {
        isPlaneDeletionMode = isOn;
        UpdateGlobalState();
    }

    // Toggle 3 (Remove Object)
    public void SetObjectRemovalMode(bool isOn)
    {
        isObjectRemovalMode = isOn;
        UpdateGlobalState();
    }

    // Toggle 4 (Place Object)
    public void SetObjectPlacementMode(bool isOn)
    {
        IsPlacementAllowed = isOn;

        if (isOn)
        {
            isPlaneDeletionMode = false;
            isObjectRemovalMode = false;
        }
    }

    private void UpdateGlobalState()
    {
        if (isPlaneDeletionMode || isObjectRemovalMode) // Fixed logic: Use || (OR) instead of &&
        {
            IsPlacementAllowed = false;
        }
    }

    // _____________Toggle Logic_______________

    // Changed from "OnTouchDetected(Finger)" to "HandleInput(Vector2)"
    private void HandleInput(Vector2 screenPosition)
    {
        // 1. Don't do anything if touching a UI button
        if (IsPointerOverUI(screenPosition)) return;

        // 2. Check for Plane Deletion
        if (isPlaneDeletionMode)
        {
            TryDeletePlane(screenPosition);
            return;
        }

        // 3. Check for Object Removal
        if (isObjectRemovalMode)
        {
            TryRemoveObject(screenPosition);
            return;
        }
    }

    // Changed argument to Vector2
    private void TryDeletePlane(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        // DEBUG RAY: Helps you see the click in the Scene view
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 2f);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, planeLayerMask))
        {
            hit.transform.gameObject.SetActive(false);
        }
    }

    // Changed argument to Vector2
    private void TryRemoveObject(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        // DEBUG RAY: Helps you see the click in the Scene view
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow, 2f);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, objectLayerMask))
        {
            Destroy(hit.transform.gameObject);
        }
    }

    // _________ Utility ____________________
    
    // Changed argument to Vector2
    private bool IsPointerOverUI(Vector2 screenPosition)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = screenPosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}