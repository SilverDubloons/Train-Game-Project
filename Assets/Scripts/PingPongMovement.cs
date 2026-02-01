using UnityEngine;

public class PingPongMovement : MonoBehaviour
{
    [SerializeField] private RectTransform rt;
    [SerializeField] private UnityEngine.UI.Image image;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 endPosition;
    [SerializeField] private float cycleTime;
    // private float awakeFactor;
    private float timeAwake;
    private void Update()
    { 
        rt.anchoredPosition = Vector3.Lerp(startPosition, endPosition, (timeAwake % cycleTime) / cycleTime);
        timeAwake += Time.deltaTime;
    }
/*    private void OnEnable()
    {
        timeAwake = cycleTime * awakeFactor;
    }*/
    public void Setup(float newAwakeFactor, Color newColor)
    {
        gameObject.SetActive(true);
        timeAwake = cycleTime * newAwakeFactor;
        image.color = newColor;
    }
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
