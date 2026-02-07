using UnityEngine;

public class SimpleRotate : MonoBehaviour
{
    public Vector3 rotationAxis = Vector3.up; // The axis to rotate around
    public float speed = -50f;                // Degrees per second

    void Update()
    {
        // Rotates the object every frame
        transform.Rotate(rotationAxis * speed * Time.deltaTime);
    }
}