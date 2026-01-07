using UnityEngine;
using UnityEngine.EventSystems;

public class TickOnMouseOver : MonoBehaviour, IPointerEnterHandler
{
    private bool canTick = true;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!canTick)
        {
            return;
        }
        SoundManager.instance.PlayTickSound();
    }
    public void SetInteractability(bool interactable)
    {
        canTick = interactable;
    }
}
