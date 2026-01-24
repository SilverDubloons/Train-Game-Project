using UnityEngine;

[CreateAssetMenu(menuName = "UI/Theme")]
public class Theme : ScriptableObject
{
    public Color backdrop;
    public Color standardButtonActive;
    public Color backButtonActive;
    public Color warningButtonActive;
    public Color buttonSpecialState;
    public Color buttonDisabled;
    public Color buttonMouseOver;
    public Color combatSpace;
    public Color enemyMouseOver;
    public Color shadow;
    public Color tooltipName; // do we need the name? since it's always on the tool itself? Might need it for other things like baubles, status effects
    public Color tooltipDamage;
    public Color tooltipSpecial;
    public Color tooltipTargetStyle;
    public Color tooltipBorder;
    public Color tooltipBody;
    public Color common;
    public Color uncommon;
    public Color rare;
    public Color legendary;
}
