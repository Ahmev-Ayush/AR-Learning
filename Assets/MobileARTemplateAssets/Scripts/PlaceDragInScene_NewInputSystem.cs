using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceDragInScene_NewInputSystem : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastmanager;
    bool isPlacing = false;

    // Update is called once per frame
    void Update()
    {
        if (!raycastmanager) return;
        if (isPlacing) return;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame && ARModeController.IsPlacementAllowed)
        {
            isPlacing = true;

            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            PlaceObject(touchPosition);
        }
        else if(Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && ARModeController.IsPlacementAllowed)
        {
            isPlacing = true;
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            PlaceObject(mousePosition);
        }
    }

    private void PlaceObject(Vector2 touchPosition)
    {
        var rayHits = new List<ARRaycastHit>();
        if (raycastmanager.Raycast(touchPosition, rayHits, TrackableType.PlaneWithinPolygon))
        {
            Vector3 hitPosePosition = rayHits[0].pose.position;
            Quaternion hitPoseRotation = rayHits[0].pose.rotation;
            Instantiate(raycastmanager.raycastPrefab, hitPosePosition, hitPoseRotation);
        }
        else
        {
            isPlacing = false;
        }
        StartCoroutine(SetIsPlacingToFalseWithDelay());
    }

    IEnumerator SetIsPlacingToFalseWithDelay()
    {
        yield return new WaitForSeconds(0.25f);
        isPlacing = false;
    }

}
