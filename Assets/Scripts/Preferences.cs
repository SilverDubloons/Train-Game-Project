using UnityEngine;

public class Preferences : MonoBehaviour
{
    public bool soundOn;
    public bool musicOn;
    public float soundVolume;
    public float musicVolume;
    public bool muteOnFocusLost;
    public float maxTimeBetweenDoubleClicks;
    public int currentTheme = 0;
    public float gameSpeed = 1f;

    [SerializeField] private string preferencesFileName;
    [SerializeField] private string preferencesFileVersion;

    public static Preferences instance;
    public void SetupInstance()
    {
        instance = this;
    }
    public void CloseMenu()
    {
        // Implementation for closing the preferences menu
    }
}
