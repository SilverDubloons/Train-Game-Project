using UnityEngine;

public class ThemedImage : MonoBehaviour
{
    public UIElementType elementType;
    public UnityEngine.UI.Image image;
    private ThemeManager themeManager;
    void Start()
    {
        themeManager.OnThemeChanged += ApplyTheme;
        ApplyTheme();
    }
    void OnDestroy()
    {
        if (themeManager != null)
        {
            themeManager.OnThemeChanged -= ApplyTheme;
        }
    }
    public void ApplyTheme()
    {
        if (image == null)
        {
            return;
        }
        if (themeManager == null)
        {
            themeManager = Resources.Load<ThemeManager>("ThemeManager");
        }
        image.color = themeManager.GetColorFromCurrentTheme(elementType);
    }
    private void OnValidate()
    {
    #if UNITY_EDITOR
        // Delay the font size application to avoid OnValidate conflicts
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this != null) // Make sure the object still exists
            {
                ApplyTheme();
            }
        };
    #endif
    }
    public void SetUIElementType(UIElementType newType)
    {
        elementType = newType;
        ApplyTheme();
    }
}
