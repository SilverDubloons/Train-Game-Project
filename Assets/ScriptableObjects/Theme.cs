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
}
