using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TapToPlaceObject : MonoBehaviour
{
    public GameObject objectToPlace; // Drag your prefab here
    [SerializeField] private ARRaycastManager raycastManager; //Drag your XR Origin here

    // A list to store the things we hit
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // Update is called once per frame
    void Update()
    {
        // 1. Check if the user touched the screen
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // 2. Shoot a ray from the touch position, only hits inside the shapes of detected plane
            if (raycastManager.Raycast(Input.GetTouch(0).position, hits, TrackableType.PlaneWithinPolygon))
            {   
                //3. Get the first point the ray hits
                Pose hitPose = hits[0].pose;

                // 4. Place the object there
                Instantiate(objectToPlace, hitPose.position, hitPose.rotation);
            }
        }
    }
}
