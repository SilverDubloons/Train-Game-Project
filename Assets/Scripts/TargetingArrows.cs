using UnityEngine;

public class TargetingArrows : MonoBehaviour
{
    [SerializeField] private RectTransform rt;
    [SerializeField] private RectTransform arrowParent;
    [SerializeField] private GameObject visibilityObject;
    [SerializeField] private RectTransform[] arrows;
    [SerializeField] private float arrowSpeed;
    private const float arrowMaxDistance = 690f;
    private const float distanceBetweenArrows = 30f;
    public static TargetingArrows instance;
    public void SetupInstance()
    {
        instance = this;
    }
    private void Start()
    {
        GameObject arrowCopy = arrows[0].gameObject;
        int numberOfArrows = Mathf.RoundToInt(arrowMaxDistance / distanceBetweenArrows) + 1;
        System.Array.Resize(ref arrows, numberOfArrows);
        for (int i = 1; i < numberOfArrows; i++)
        {
            RectTransform newArrow = Instantiate(arrowCopy, arrowParent).GetComponent<RectTransform>();
            newArrow.name = $"Arrow{r.i.interf.GetFixedLengthIntString(i, 2)}";
            newArrow.anchoredPosition = new Vector2(arrows[0].anchoredPosition.x, i * distanceBetweenArrows);
            arrows[i] = newArrow;
        }
    }
    void Update()
    {
        if(visibilityObject.activeSelf)
        {
            float moveDistance = arrowSpeed * Time.deltaTime;
            for (int i = 0; i < arrows.Length; i++)
            {
                arrows[i].anchoredPosition = new Vector2(arrows[i].anchoredPosition.x, arrows[i].anchoredPosition.y + moveDistance);
                if(arrows[i].anchoredPosition.y > arrowMaxDistance)
                {
                    arrows[i].anchoredPosition = new Vector2(arrows[i].anchoredPosition.x, arrows[i].anchoredPosition.y - arrowMaxDistance - distanceBetweenArrows);
                }
            }
        }
    }
    public void SetPosition(Vector2 newPosition)
    {
        rt.anchoredPosition = newPosition;
    }
    public void SetVisibility(bool visible)
    {
        visibilityObject.SetActive(visible);
    }
    public void SetTarget(Vector2 targetPosition)
    {
        Vector2 direction = targetPosition - rt.anchoredPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        rt.rotation = Quaternion.Euler(0f, 0f, angle);
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, direction.magnitude);
    }
}
