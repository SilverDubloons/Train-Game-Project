using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ScaleOnMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform rt;
    public Vector3 destinationScale;
    public float duration = 0.1f;
    private bool canScale = true;
    private IEnumerator scaleCoroutine;
    private bool changingScale = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        StartExpand();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        StartRetract();
    }
    public void StartExpand()
    {
        StartChangeScale(destinationScale, duration);
    }
    public void StartRetract()
    {
        StartChangeScale(Vector3.one, duration);
    }
    private void StartChangeScale(Vector3 destinationScale, float duration)
    {
        if (changingScale)
        {
            StopCoroutine(scaleCoroutine);
        }
        if(!canScale)
        {
            return;
        }
        scaleCoroutine = ChangeScale(destinationScale, duration);
        StartCoroutine(scaleCoroutine);
    }
    private IEnumerator ChangeScale(Vector3 destinationScale, float duration)
    {
        changingScale = true;
        Vector3 startingScale = rt.localScale;
        float t = 0;
        while (t < duration)
        {
            t = Mathf.Clamp(t + Time.deltaTime, 0, duration);
            rt.localScale = new Vector3(Mathf.Lerp(startingScale.x, destinationScale.x, t / duration), Mathf.Lerp(startingScale.y, destinationScale.y, t / duration), 1f);
            yield return null;
        }
        changingScale = false;
    }
    public void SetInteractability(bool interactable)
    {
        canScale = interactable;
        if (!interactable)
        {
            ResetScale();
        }
    }
    private void ResetScale()
    {
        rt.localScale = Vector3.one;
    }
}
