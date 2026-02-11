using UnityEngine;
using UnityEngine.UIElements;

public class quadSizeVariable : MonoBehaviour
{
    public ScriptableObjectScript ItemData;

    // void OnValidate()
    // {
    //     setScale(quadSize);
    // }

    // Start is called before the first frame update
    void Start()
    {
        setScale(ItemData.quadSize);
    }

    void setScale(float newSize)
    {
        // 2:1 is the aspect ratio of the quad (width:height)
        transform.localScale = new Vector3(2 * ItemData.quadSize, ItemData.quadSize, 1f);
    }

}
