using UnityEngine;

public class prefabScalerForBallShooting : MonoBehaviour
{
    // scaling factor with range from 0.1 to 10f
    [Header("Scaling Factor")]
    [Range(0.1f, 10f)]
    public float scaleFactor = 1f;
    public float scaleStep = 0.5f; // step size for scaling the prefabs

    [Header("Court Settings")]
    // court Game Object to scale
    public GameObject CourtPrefab;
    // default scale factor for the court
    public Vector3 defaultScaleFactorCourt = new Vector3(2f, 1f, 1f);

    [Header("Racket Settings")]
    // racket prefab to scale
    public GameObject racketPrefab;
    // default scale factor for the racket
    public float defaultScaleFactorRacket = 0.4f;

    [Header("Ball Settings")]
    // ball prefab to scale
    public GameObject ballPrefab;
    // default scale factor for the ball
    public float defaultScaleFactorBall = 0.01167f;

    [Header("Court Rotation and Positioning")]
    public float rotateCourt = 90f; // rotation angle the court to face
    public float value_to_spawn_in_front = 6.0f; // distance in front of the camera to spawn the court

    // on validate, update the scale of the court and racket prefabs
    public void OnValidate()
    {
        UpdateScale();
        PositionObjectInFront();
    }

    void Start()
    {
        UpdateScale();
        PositionObjectInFront();
    }


    private void UpdateScale()
    {
        if (CourtPrefab != null)
        {
            CourtPrefab.transform.localScale = defaultScaleFactorCourt * scaleFactor;
        }

        if (racketPrefab != null)
        {
            racketPrefab.transform.localScale = new Vector3(defaultScaleFactorRacket * scaleFactor, defaultScaleFactorRacket * scaleFactor, defaultScaleFactorRacket * scaleFactor);
        }
        if (ballPrefab != null)
        {
            ballPrefab.transform.localScale = new Vector3(defaultScaleFactorBall * scaleFactor, defaultScaleFactorBall * scaleFactor, defaultScaleFactorBall * scaleFactor);
        }
    }

    // scale up the prefabs by increasing the scale factor (with a maximum of 10)
    public void ScaleUp()
    {
        scaleFactor = Mathf.Min(10f, scaleFactor + scaleStep); // prevent scaling above 10
        UpdateScale();
    }

    // scale down the prefabs by decreasing the scale factor
    public void ScaleDown()
    {
        scaleFactor = Mathf.Max(0.1f, scaleFactor - scaleStep); // prevent scaling below 0.1
        UpdateScale();
    }
    void PositionObjectInFront()
    {
        // Position it 2 units in front of the camera
        Vector3 spawnPos = Camera.main.transform.position + (Camera.main.transform.forward * value_to_spawn_in_front);

        // Optional: Keep it at a specific height (e.g., ground level)
        spawnPos.y = -1.0f; 

        CourtPrefab.transform.position = spawnPos;

        // Make the object look at the user, but stay upright
        CourtPrefab.transform.LookAt(new Vector3(Camera.main.transform.position.x, CourtPrefab.transform.position.y, Camera.main.transform.position.z));
        CourtPrefab.transform.Rotate(0, rotateCourt, 0); // Flip it to face the camera correctly
    }

}
