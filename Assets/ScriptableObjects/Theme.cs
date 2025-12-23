using UnityEngine;

[CreateAssetMenu(menuName = "UI/Theme")]
public class Theme : ScriptableObject
{
    public Color backdrop;
    public Color standardButtonActive;
    public Color backButtonActive;
    public Color warningButtonActive;
    public Color buttonSpecialState;
}
