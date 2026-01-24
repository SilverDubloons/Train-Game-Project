using UnityEngine;

public class MainMenuStart : MonoBehaviour
{
    [SerializeField] private r references;
    [SerializeField] private Preferences preferences;
    [SerializeField] private MovingObjects movingObjects;
    [SerializeField] private TransitionStinger transitionStinger;
    [SerializeField] private Tooltip tooltip;
    void Awake()
    {
        if (r.i == null) // only setup if not already done, these ones are persistent
        {
            references.SetupInstance();
            references.interf.InitialSetup();
            preferences.SetupInstance();
            transitionStinger.SetupInstance();
            tooltip.SetupInstance();
        }
        movingObjects.SetupInstance();
        MovingObjects.instance.mo["MainMenu"].TeleportTo("OffScreen");
        MovingObjects.instance.mo["MainMenu"].StartMove("OnScreen");
    }
}
