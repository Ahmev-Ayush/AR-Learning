using UnityEngine;

public class quadSizeVariable : MonoBehaviour
{
    [Range(0.1f, 10f)] // adds a handy slider in the inspector
    public float quadSize = 1f;

    // void OnValidate()
    // {
    //     setScale(quadSize);
    // }

    // Start is called before the first frame update
    void Start()
    {
        setScale(quadSize);
    }

    void setScale(float newSize)
    {
        // 2:1 is the aspect ratio of the quad (width:height)
        transform.localScale = new Vector3(2 * newSize, newSize, 1f);
    }

}
