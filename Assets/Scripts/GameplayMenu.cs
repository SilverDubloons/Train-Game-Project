using UnityEngine;
// This class is for debugging gameplay features
public class GameplayMenu : MonoBehaviour
{
    [SerializeField] private ButtonPlus startCombatButton;
    [SerializeField] private Backdrop backdrop;
    [SerializeField] private Encounter[] encounters;

    public void SetInteractability(bool interactable)
    {
        startCombatButton.SetButtonEnabled(interactable);
    }
    public void SetVisibility(bool visible)
    {
        backdrop.SetVisibility(visible);
    }
    public void Click_StartCombat()
    {
        MovingObjects.instance.mo["GameplayMenu"].StartMove("OffScreen");
        MovingObjects.instance.mo["CombatArea"].StartMove("OnScreen");
        MovingObjects.instance.mo["DrawPile"].StartMove("OnScreen");
        CombatManager.instance.SetupCombat(encounters[0]);
    }
}
