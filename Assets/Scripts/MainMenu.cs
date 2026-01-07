using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private ButtonPlus newGameButton;
    [SerializeField] private Backdrop backdrop;

    public void SetInteractability(bool interactable)
    {
        newGameButton.SetButtonEnabled(interactable);
    }
    public void SetVisibility(bool visible)
    {
        backdrop.SetVisibility(visible);
    }
    public void Click_NewGame()
    {
        MovingObjects.instance.mo["MainMenu"].StartMove("OffScreen");
        TransitionStinger.instance.StartStinger("GameplayScene");
    }
}
