using UnityEngine;
public enum UIElementType { backdrop, standardButtonActive, backButtonActive, warningButtonActive, buttonSpecialState, buttonDisabled, buttonMouseOver, combatSpace, enemyMouseOver, shadow, tooltipName, tooltipDamage, tooltipSpecial, tooltipTargetStyle, tooltipBorder, tooltipBody, common, uncommon, rare, legendary }
[CreateAssetMenu(menuName = "UI/ThemeManager")]
public class ThemeManager : ScriptableObject
{
    public int currentThemeIndex;
    [SerializeField] private Theme[] themes;
    public event System.Action OnThemeChanged;
    public Color GetColorFromCurrentTheme(UIElementType elementType)
    {
        Theme theme = themes[currentThemeIndex];
        return elementType switch
        {
            UIElementType.backdrop => theme.backdrop,
            UIElementType.standardButtonActive => theme.standardButtonActive,
            UIElementType.backButtonActive => theme.backButtonActive,
            UIElementType.warningButtonActive => theme.warningButtonActive,
            UIElementType.buttonSpecialState => theme.buttonSpecialState,
            UIElementType.buttonDisabled => theme.buttonDisabled,
            UIElementType.buttonMouseOver => theme.buttonMouseOver,
            UIElementType.combatSpace => theme.combatSpace,
            UIElementType.enemyMouseOver => theme.enemyMouseOver,
            UIElementType.shadow => theme.shadow,
            UIElementType.tooltipName => theme.tooltipName,
            UIElementType.tooltipDamage => theme.tooltipDamage,
            UIElementType.tooltipSpecial => theme.tooltipSpecial,
            UIElementType.tooltipTargetStyle => theme.tooltipTargetStyle,
            UIElementType.tooltipBorder => theme.tooltipBorder,
            UIElementType.tooltipBody => theme.tooltipBody,
            UIElementType.common => theme.common,
            UIElementType.uncommon => theme.uncommon,
            UIElementType.rare => theme.rare,
            UIElementType.legendary => theme.legendary,
            _ => Color.white
        };
    }
    public void ApplyTheme(int newThemeIndex)
    {
        currentThemeIndex = newThemeIndex;
        OnThemeChanged?.Invoke();
    }
}
