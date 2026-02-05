using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageTrackingManager : MonoBehaviour
{
    [Header("Prefab Setup")]
    [SerializeField] private List<GameObject> prefabsToSpawn;

    private ARTrackedImageManager trackedImageManager;
    private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void Start()
    {
        if (trackedImageManager.referenceLibrary == null) return;

        // Map the library index to your prefab list
        for (int i = 0; i < trackedImageManager.referenceLibrary.count; i++)
        {
            if (i < prefabsToSpawn.Count)
            {
                string imageName = trackedImageManager.referenceLibrary[i].name;
                prefabDictionary.Add(imageName, prefabsToSpawn[i]);
            }
        }
    }

    // Instead of the obsolete event, we use Update to check the state of trackables
    void Update()
    {
        // Iterate through all currently tracked images
        foreach (var trackedImage in trackedImageManager.trackables)
        {
            UpdateImage(trackedImage);
        }
    }

    private void UpdateImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;

        // Check if the reference image or its name is null before proceeding
        if (trackedImage.referenceImage == null || string.IsNullOrEmpty(trackedImage.referenceImage.name))
        {
            // Debug.LogWarning("Found a tracked image with no reference name!");
            return; 
        }

        // 1. Instantiate if missing
        if (!spawnedPrefabs.ContainsKey(name) && prefabDictionary.ContainsKey(name))
        {
            GameObject newInstance = Instantiate(prefabDictionary[name], trackedImage.transform);
            spawnedPrefabs.Add(name, newInstance);
        }

        // 2. Update status based on TrackingState
        if (spawnedPrefabs.TryGetValue(name, out GameObject instance))
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                instance.SetActive(true);
                instance.transform.SetPositionAndRotation(trackedImage.transform.position, trackedImage.transform.rotation);
            }
            // else
            // {
            //     // Hide if tracking is "Limited" or "None"
            //     instance.SetActive(false);
            // }
        }
    }
}