using UnityEngine;

public class prefabScalerForBallShooting : MonoBehaviour
{
    // scaling factor with range from 0.1 to 10f
    [Header("Scaling Factor")]
    [Range(0.1f, 10f)]
    public float scaleFactor = 1f;

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



    // on validate, update the scale of the court and racket prefabs
    public void OnValidate()
    {
        UpdateScale();
    }

    void Start()
    {
        UpdateScale();
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


}
