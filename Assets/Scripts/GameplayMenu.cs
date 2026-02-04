using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
// This class is for debugging gameplay features
public class GameplayMenu : MonoBehaviour
{
    [SerializeField] private ButtonPlus startCombatButton;
    [SerializeField] private ButtonPlus endCombatButton;
    [SerializeField] private Backdrop backdrop;
    [SerializeField] private TMP_Dropdown encounterDropdown;
    public static GameplayMenu instance;
    public void SetupInstance()
    {
        instance = this;
        encounterDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < r.i.encounters.Length; i++)
        { 
            TMP_Dropdown.OptionData newOption = new TMP_Dropdown.OptionData(r.i.encounters[i].encounterTag);
            options.Add(newOption);
        }
        encounterDropdown.AddOptions(options);
    }
    public void SetInteractability(bool interactable)
    {
        startCombatButton.SetButtonEnabled(interactable);
        encounterDropdown.interactable = interactable;
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
        MovingObjects.instance.mo["DiscardPile"].StartMove("OnScreen");
        MovingObjects.instance.mo["EndTurnButtonBackdrop"].StartMove("OnScreen");
        MovingObjects.instance.mo["PlayerStatsBackdrop"].StartMove("OnScreen");
        MovingObjects.instance.mo["EndCombatButtonBackdrop"].StartMove("OnScreen");
        // CombatManager.instance.SetupCombat(r.i.encounterDictionary["MixedCrew"]);
        string selectedEncounterTag = encounterDropdown.options[encounterDropdown.value].text;
        CombatManager.instance.SetupCombat(r.i.encounterDictionary[selectedEncounterTag]);
        Player.instance.SetMaxHealth(60);
        Player.instance.SetCurrentHealth(60);
        Player.instance.SetShield(0);
    }
    public void ClickEndCombat()
    {
        MovingObjects.instance.mo["GameplayMenu"].StartMove("OnScreen");
        MovingObjects.instance.mo["CombatArea"].StartMove("OffScreen");
        MovingObjects.instance.mo["DrawPile"].StartMove("OffScreen");
        MovingObjects.instance.mo["DiscardPile"].StartMove("OffScreen");
        MovingObjects.instance.mo["EndTurnButtonBackdrop"].StartMove("OffScreen");
        MovingObjects.instance.mo["PlayerStatsBackdrop"].StartMove("OffScreen");
        MovingObjects.instance.mo["EndCombatButtonBackdrop"].StartMove("OffScreen");
        CombatManager.instance.CleanupCombat();
        HandArea.instance.ReturnCardsInHandToDrawPile();
        endCombatButton.SetButtonEnabled(false);
        GameDeck.instance.StartShuffleDiscardPileIntoDrawPile();
    }
    public void SetEndCombatButtonEnabled(bool enabled)
    { 
        endCombatButton.SetButtonEnabled(enabled);
    }
}
