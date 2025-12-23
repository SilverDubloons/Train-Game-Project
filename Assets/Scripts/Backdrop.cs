using UnityEngine;
using UnityEngine.UI;

public class Backdrop : MonoBehaviour
{
    public enum BackdropShape { corner4, corner8 }

    [SerializeField] private BackdropShape backdropShape = BackdropShape.corner4;
    [SerializeField] private UIElementType backdropStyle = UIElementType.backdrop;
    [SerializeField] private BackdropStyle style;
    [SerializeField] private ThemeManager themeManager;

    public RectTransform rt;
    public RectTransform rtShadow;
    public Image backdrop;
    public Image shadow;

    private void OnValidate()
    {
        ApplyImages();
        ApplyTheme();
    }
    private void Start()
    {
        themeManager.OnThemeChanged += ApplyTheme;
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
            backdrop.color = themeManager.GetColorFromCurrentTheme(backdropStyle);
        }
    }
}
