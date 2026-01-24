using UnityEngine;

public class PingPongMovement : MonoBehaviour
{
    [SerializeField] private RectTransform rt;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 endPosition;
    [SerializeField] private float cycleTime;
    [SerializeField] private float awakeFactor;
    private float timeAwake;
    private void Update()
    { 
        rt.anchoredPosition = Vector3.Lerp(startPosition, endPosition, (timeAwake % cycleTime) / cycleTime);
        timeAwake += Time.deltaTime;
    }
    private void OnEnable()
    {
        timeAwake = cycleTime * awakeFactor;
    }
}
