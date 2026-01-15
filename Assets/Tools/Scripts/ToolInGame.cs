using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolInGame : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform rt;
    [SerializeField] private GameObject visibilityObject;
    [SerializeField] private Image toolImage;
    [SerializeField] private Label toolNameLabel;
    [SerializeField] private Label handStyleLabel;
    // [SerializeField] private RectTransform requiredCardsParent;
    [SerializeField] private ButtonPlus buttonPlus;
    private ToolInGame baseToolInGame;
    public Tool baseTool;
    private bool isUsable;
    public void SetVisibility(bool visible)
    { 
        visibilityObject.SetActive(visible);
    }
    public void SetInteractability(bool interactable)
    {
        buttonPlus.SetButtonEnabled(interactable);
    }
    public void SetupNewToolInGame(Tool newBaseTool)
    {
        baseToolInGame = null;
        baseTool = newBaseTool;
        toolImage.sprite = newBaseTool.toolIcon;
        toolNameLabel.ChangeText(newBaseTool.toolName);
        handStyleLabel.ChangeText(newBaseTool.GetHandStyleString());
        isUsable = false;
    }
    public void SetupFromToolInGame(ToolInGame toolInGame)
    {
        baseToolInGame = toolInGame;
        baseTool = toolInGame.baseTool;
        toolImage.sprite = toolInGame.toolImage.sprite;
        toolNameLabel.ChangeText(toolInGame.toolNameLabel.GetText());
        handStyleLabel.ChangeText(toolInGame.handStyleLabel.GetText());
        isUsable = true;
    }
    public void Click()
    { 
    
    }
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (CombatManager.instance.inCombat)
        {
            CombatArea.instance.PreviewSelectableTargets(baseTool.toolTargetStyle, baseTool.adjacentColumnsTarget);
        }
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        CombatArea.instance.EndTargetPreview();
    }
    public void PreviewSelectableTargets()
    { 
    
    }
    public void EndTargetPreview()
    { 
    
    }
}
