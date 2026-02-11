using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObjectScript", menuName = "Scriptable Objects/ScriptableObjectScript")]
public class ScriptableObjectScript : ScriptableObject
{
    [Range(0.1f, 10f)] // adds a handy slider in the inspector
    public float quadSize = 1f;

}


