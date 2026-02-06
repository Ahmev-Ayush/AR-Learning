using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
// enhanced touch
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using UnityEngine.EventSystems;

public class cloudpointToPlacePrefab : MonoBehaviour
{
    [SerializeField] private GameObject prefabToPlace;
    private GameObject spawnedObject;
    [SerializeField] private ARRaycastManager aRRaycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputPosition = Vector2.zero;
        bool inputBegan = false;

        // Check for Touch input
        if (Touch.activeTouches.Count > 0)
        {
            var activeTouch = Touch.activeTouches[0];
            if (activeTouch.phase == UnityEngine.InputSystem.TouchPhase.Began && !IsPointerOverUI(activeTouch))
            {
                inputPosition = activeTouch.screenPosition;
                inputBegan = true;
            }
        }
        // Check for Mouse click (useful for Editor testing)
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject())
            {
                inputPosition = Mouse.current.position.ReadValue();
                inputBegan = true;
            }
        }

        if (inputBegan)
        {
            // Perform the raycast and place/move the prefab if a feature point is hit
            if (aRRaycastManager.Raycast(inputPosition, hits, TrackableType.FeaturePoint))
            {
                // Get the pose of the hit point
                Pose hitPose = hits[0].pose;
                Vector3 hitPosition = hitPose.position;
                
                // Rotate the prefab to face the camera while keeping it upright
                Quaternion hitRotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0); 

                if (spawnedObject == null)
                {
                    // If the object doesn't exist, create it at the hit position
                    spawnedObject = Instantiate(prefabToPlace, hitPosition, hitRotation);
                }
                else
                {
                    // If the object already exists, move it smoothly to the new position
                    StopAllCoroutines(); // Stop any ongoing movement to prevent conflicts
                    StartCoroutine(SmoothMove(hitPosition)); // Start the smooth movement coroutine
                }
            }
        }
    }

    private bool IsPointerOverUI(Touch touch)
    {
        // Check if the touch is over a UI element
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.finger.index);
    }

    private System.Collections.IEnumerator SmoothMove(Vector3 targetPos)
    {
        float elapsedTime = 0f;
        float duration = 1.5f; // Duration of the movement in seconds
        Vector3 startingPos = spawnedObject.transform.position;

        while (elapsedTime < duration)
        {
            spawnedObject.transform.position = Vector3.Lerp(startingPos, targetPos, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        spawnedObject.transform.position = targetPos; // Ensure the final position is set
    }
}