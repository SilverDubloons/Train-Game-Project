using UnityEngine;

public class Preferences : MonoBehaviour
{
    public int currentTheme;

    [SerializeField] private string preferencesFileName;
    [SerializeField] private string preferencesFileVersion;

    public static Preferences instance;
    public void SetupInstance()
    {
        instance = this;
    }
}
