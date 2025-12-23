using UnityEngine;

public class Interface : MonoBehaviour
{
    public Sprite backdrop4pxCorners;
    public Sprite backdrop8pxCorners;
    public Sprite shadow4pxCorners;
    public Sprite shadow8pxCorners;

    public static Interface instance;

    private void Awake()
    {
        instance = this;
    }
}
