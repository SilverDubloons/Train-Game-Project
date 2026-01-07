using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Backdrop : MonoBehaviour
{
    public enum BackdropShape { corner4, corner8 }
    [SerializeField] private BackdropShape backdropShape = BackdropShape.corner4;
    [SerializeField] private UIElementType backdropUIElementType = UIElementType.backdrop;
    [SerializeField] private BackdropStyle style;
    [SerializeField] private ThemeManager themeManager;

    [SerializeField] private RectTransform rt;
    [SerializeField] private RectTransform rtShadow;
    [SerializeField] private RectTransform rtBackdrop;
    [SerializeField] private Image backdrop;
    [SerializeField] private Image shadow;
    [SerializeField] private GameObject visibilityObject;

    private IEnumerator scaleCoroutine;
    private bool changingScale = false;
    private IEnumerator colorCoroutine;
    private bool changingColor = false;
    private IEnumerator moveCoroutine;
    private bool movingImage = false;

    private Vector2 backdropOrigin;

    private void OnValidate()
    {
        ApplyImages();
        ApplyTheme();
    }
    private void Start()
    {
        themeManager.OnThemeChanged += ApplyTheme;
        backdropOrigin = rtBackdrop.anchoredPosition;
    }
    private void ApplyImages()
    {
        switch (backdropShape)
        {
            case BackdropShape.corner4:
            default:
                backdrop.sprite = style.backdrop4;
                shadow.sprite = style.shadow4;
                rtShadow.sizeDelta = new Vector2(rtShadow.sizeDelta.x, 6f);
                break;
            case BackdropShape.corner8:
                backdrop.sprite = style.backdrop8;
                shadow.sprite = style.shadow8;
                rtShadow.sizeDelta = new Vector2(rtShadow.sizeDelta.x, 10f);
                break;
        }
    }
    public void ApplyTheme()
    {
        if (backdrop != null)
        {
            backdrop.color = themeManager.GetColorFromCurrentTheme(backdropUIElementType);
        }
    }
    public void UpdateBackdropUIElementType(UIElementType newUIElementType)
    {
        backdropUIElementType = newUIElementType;
        ApplyTheme();
    }
    public bool IsMouseOver()
    {
        if(backdrop == null || shadow == null || EventSystem.current == null || Mouse.current == null)
        {
            return false;
        }
        List<RaycastResult> results = new List<RaycastResult>();
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Mouse.current.position.ReadValue();
        EventSystem.current.RaycastAll(pointerEventData, results);
        foreach (RaycastResult result in results)
        {
            if (result.gameObject == backdrop.gameObject || result.gameObject == shadow.gameObject)
            {
                return true;
            }
        }
        return false;
    }
    public Color GetCurrentBaseColor()
    {
        return themeManager.GetColorFromCurrentTheme(backdropUIElementType);
    }
    public void StartChangeScale(Vector3 destinationScale, float duration)
    {
        if (changingScale)
        {
            StopCoroutine(scaleCoroutine);
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
            t += Time.deltaTime;
            rt.localScale = new Vector3(Mathf.Lerp(startingScale.x, destinationScale.x, t / duration), Mathf.Lerp(startingScale.y, destinationScale.y, t / duration), 1f);
            yield return null;
        }
        rt.localScale = destinationScale;
        changingScale = false;
    }
    public void StartColorDarken(float duration)
    { 
        StartChangeColor(themeManager.GetColorFromCurrentTheme(backdropUIElementType) * 
            themeManager.GetColorFromCurrentTheme(UIElementType.buttonMouseOver), duration);
    }
    public void StartColorReset(float duration)
    {
        StartChangeColor(themeManager.GetColorFromCurrentTheme(backdropUIElementType), duration);
    }
    public void StartChangeColor(Color destinationColor, float duration)
    {
        if (changingColor)
        {
            StopCoroutine(colorCoroutine);
        }
        colorCoroutine = ChangeColor(destinationColor, duration);
        StartCoroutine(colorCoroutine);
    }
    public void StopChangingColor()
    { 
        if(changingColor)
        {
            StopCoroutine(colorCoroutine);
            changingColor = false;
        }
    }
    private IEnumerator ChangeColor(Color destinationColor, float duration)
    {
        changingColor = true;
        Color originColor = backdrop.color;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            backdrop.color = Color.Lerp(originColor, destinationColor, t / duration);
            yield return null;
        }
        backdrop.color = destinationColor;
        changingColor = false;
    }
    public void StartMoveImage(Vector2 destinationFromOrigin, float duration)
    {
        Vector2 destination = backdropOrigin + destinationFromOrigin;
        if (movingImage)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = MoveImage(destination, duration);
        StartCoroutine(moveCoroutine);
    }
    private IEnumerator MoveImage(Vector2 destination, float duration)
    {
        movingImage = true;
        Vector2 origin = rtBackdrop.anchoredPosition;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            rtBackdrop.anchoredPosition = Vector2.Lerp(origin, destination, t / duration);
            yield return null;
        }
        rtBackdrop.anchoredPosition = destination;
        movingImage = false;
    }
    public void ResetBackdrop()
    {
        if (changingScale)
        {
            StopCoroutine(scaleCoroutine);
            changingScale = false;
        }
        if (changingColor)
        {
            StopCoroutine(colorCoroutine);
            changingColor = false;
        }
        if (movingImage)
        {
            StopCoroutine(moveCoroutine);
            movingImage = false;
        }
        rt.localScale = Vector3.one;
        rtBackdrop.anchoredPosition = backdropOrigin;
    }
    public void SetVisibility(bool visible)
    {
        if (visibilityObject != null)
        {
            visibilityObject.SetActive(visible);
        }
    }
}
