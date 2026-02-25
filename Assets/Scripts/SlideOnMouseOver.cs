using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class SlideOnMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform rt;
    [SerializeField] private Vector2 origin;
    [SerializeField] private Vector2 destination;
    private bool movingToOrigin = false;
    private bool movingToDestination = false;
    private bool isMoving = false;
    private IEnumerator moveCoroutine;
    [SerializeField] private UnityEvent startMovingToOriginEvent;
    [SerializeField] private UnityEvent finishMovingToOriginEvent;
    [SerializeField] private UnityEvent startMovingToDestinationEvent;
    [SerializeField] private UnityEvent finishMovingToDestinationEvent;
    private bool interactable;
    public void SetInteractability(bool newInteractableState)
    { 
        interactable = newInteractableState;
        if (!interactable)
        {
            if(IsAtOrigin() || (isMoving && movingToOrigin))
            {
                return;
            }
            movingToDestination = false;
            movingToOrigin = true;
            if (!isMoving && IsAtDestination())
            {
                startMovingToOriginEvent.Invoke();
                moveCoroutine = MoveCoroutine();
                StartCoroutine(moveCoroutine);
            }
            else
            {
                // rt.anchoredPosition = origin;
            }
        }
    }
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (!interactable)
        {
            return;
        }
        movingToDestination = true;
        movingToOrigin = false;
        if (!isMoving)
        {
            startMovingToDestinationEvent.Invoke();
            moveCoroutine = MoveCoroutine();
            StartCoroutine(moveCoroutine);
        }
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (!interactable)
        {
            return;
        }
        movingToDestination = false;
        movingToOrigin = true;
        if (!isMoving)
        {
            startMovingToOriginEvent.Invoke();
            moveCoroutine = MoveCoroutine();
            StartCoroutine(moveCoroutine);
        }
    }
    private IEnumerator MoveCoroutine()
    {
        // Logger.instance.Log($"SlideOnMouseOver MoveCoroutine movingToDestination={movingToDestination}, movingToOrigin={movingToOrigin}");
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
        if (movingToDestination)
        { 
            finishMovingToDestinationEvent.Invoke();
        }
        if(movingToOrigin)
        {
            finishMovingToOriginEvent.Invoke();
        }
        isMoving = false;
    }
    private bool IsAtOrigin()
    {
        return Mathf.Abs(rt.anchoredPosition.x - origin.x) < 0.1f && Mathf.Abs(rt.anchoredPosition.y - origin.y) < 0.1f;
    }
    private bool IsAtDestination()
    {
        return Mathf.Abs(rt.anchoredPosition.x - destination.x) < 0.1f && Mathf.Abs(rt.anchoredPosition.y - destination.y) < 0.1f;
    }
}
