using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
public class SlideOnMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform rt;
    [SerializeField] private Vector2 origin;
    [SerializeField] private Vector2 destination;
    private bool movingToOrigin = false;
    private bool movingToDestination = false;
    private bool isMoving = false;
    private IEnumerator moveCoroutine;
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        movingToDestination = true;
        movingToOrigin = false;
        if (!isMoving)
        {
            moveCoroutine = MoveCoroutine();
            StartCoroutine(moveCoroutine);
        }
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        movingToDestination = false;
        movingToOrigin = true;
        if (!isMoving)
        {
            moveCoroutine = MoveCoroutine();
            StartCoroutine(moveCoroutine);
        }
    }
    private IEnumerator MoveCoroutine()
    {
        isMoving = true;
        float moveTime = 0.5f;
        float t = movingToDestination ? 0 : moveTime;
        while ((movingToDestination && t < moveTime) || (movingToOrigin && t > 0))
        {
            if (movingToOrigin)
            {
                t = Mathf.Clamp(t - Time.deltaTime * Preferences.instance.gameSpeed, 0, moveTime);
            }
            else if (movingToDestination)
            {
                t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, moveTime);
            }
            float normalizedTime = t / moveTime;
            rt.anchoredPosition = Vector2.Lerp(origin, destination, normalizedTime);
            yield return null;
        }
        isMoving = false;
    }
}
