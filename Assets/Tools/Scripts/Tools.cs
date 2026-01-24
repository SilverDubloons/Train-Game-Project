using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tools : MonoBehaviour
{
    [SerializeField] private RectTransform toolsInGameParent;
    [SerializeField] private Backdrop toolsInGameBackdrop;
    [SerializeField] private GameObject toolsInGameVisibilityObject;
    [SerializeField] private RectTransform playableToolsParent;
    [SerializeField] private Backdrop playableToolsBackdrop;
    [SerializeField] private GameObject playableToolsVisibilityObject;
    [SerializeField] private RectTransform spareToolInGamesParent;
    [SerializeField] private GameObject toolInGamePrefab;
    private List<ToolInGame> playerTools = new List<ToolInGame>();
    private List<ToolInGame> playerPlayableTools = new List<ToolInGame>();
    private HandEvaluation handEvaluation;
    public static Tools instance;
    public void SetupInstance()
    {
        instance = this;
        handEvaluation = new HandEvaluation();
        for (int i = 0; i < r.i.tools.Length; i++)
        {
            AddNewTool(r.i.tools[i]);
        }
    }
    public void SetToolsVisibility(bool visible)
    {
        toolsInGameVisibilityObject.SetActive(visible);
    }
    public void SetUsableToolsVisibility(bool visible)
    {
        playableToolsVisibilityObject.SetActive(visible);
    }
    public void SetToolsInteractability(bool interactable)
    {
        foreach (ToolInGame tool in playerTools)
        {
            if (!interactable)
            {
                tool.SetInteractability(interactable);
            }
            else
            { 
            
            }
        }
    }
    public void SetUsableToolsInteractability(bool interactable)
    {
        foreach (ToolInGame tool in playerPlayableTools)
        { 
            tool.SetInteractability(interactable);
        }
    }
    public void AddNewTool(Tool baseTool)
    {
        ToolInGame newToolInGame = GetNewToolInGame(toolsInGameParent);
        newToolInGame.SetupNewToolInGame(baseTool);
        playerTools.Add(newToolInGame);
        ReorganizeToolsInGame();
    }
    private ToolInGame GetNewToolInGame(RectTransform parent)
    {
        ToolInGame newToolInGame;
        if (spareToolInGamesParent.childCount > 0)
        {
            newToolInGame = spareToolInGamesParent.GetChild(spareToolInGamesParent.childCount - 1).GetComponent<ToolInGame>();
            newToolInGame.rt.SetParent(parent);
            newToolInGame.SetVisibility(true);
        }
        else
        {
            newToolInGame = Instantiate(toolInGamePrefab, parent).GetComponent<ToolInGame>();
        }
        return newToolInGame;
    }
    private void ReorganizeToolsInGame()
    { 
        for(int i = 0; i < playerTools.Count; i++)
        {
            playerTools[i].rt.anchoredPosition = new Vector2(-(playerTools.Count - 1) * (r.i.interf.toolInGameSize.x / 2 + r.i.interf.spaceBetweenToolsInGame.x / 2) + i * (r.i.interf.toolInGameSize.x + r.i.interf.spaceBetweenToolsInGame.x), 0);
        }
        toolsInGameBackdrop.SetSize(new Vector2(playerTools.Count * (r.i.interf.toolInGameSize.x + r.i.interf.spaceBetweenToolsInGame.x) + r.i.interf.spaceBetweenToolsInGame.x, r.i.interf.toolInGameSize.y + r.i.interf.spaceBetweenToolsInGame.y * 2));
    }
    private void DisableToolInGame(ToolInGame toolInGame)
    {
        toolInGame.SetVisibility(false);
        toolInGame.rt.SetParent(spareToolInGamesParent);
    }
    public void DeterminePlayableTools(List<Card> selectedCards)
    {
        if (selectedCards.Count <= 0)
        {
            MovingObjects.instance.mo["PlayableTools"].StartMove("OffScreen");
            return;
        }
        List<ToolInGame> correctHandSizeTools = playerTools.Where(tool => tool.baseTool.cardsRequired == selectedCards.Count).ToList();
        if (correctHandSizeTools.Count <= 0)
        {
            MovingObjects.instance.mo["PlayableTools"].StartMove("OffScreen");
            return;
        }
        List<CardData> cardDatas = new List<CardData>();
        for (int i = 0; i < selectedCards.Count; i++)
        {
            cardDatas.Add(selectedCards[i].cardData);
        }
        List<HandType> containedHands = handEvaluation.EvaluateHand(cardDatas);
        List<ToolInGame> playableTools = new List<ToolInGame>();
        for (int i = 0; i < correctHandSizeTools.Count; i++)
        {
            if (containedHands.Contains(correctHandSizeTools[i].baseTool.handStyle))
            {
                playableTools.Add(correctHandSizeTools[i]);
            }
        }
        if (playableTools.Count <= 0)
        {
            MovingObjects.instance.mo["PlayableTools"].StartMove("OffScreen");
            return;
        }
        for (int i = 0; i < playerPlayableTools.Count; i++)
        {
            DisableToolInGame(playerPlayableTools[i]);
        }
        playerPlayableTools.Clear();
        playableTools.Sort((tool1, tool2) =>
        { 
            return tool1.baseTool.toolName.CompareTo(tool2.baseTool.toolName);
        });
        for (int i = 0; i < playableTools.Count; i++)
        {
            ToolInGame playableTool = GetNewToolInGame(playableToolsParent);
            playerPlayableTools.Add(playableTool);
            playableTool.SetupFromToolInGame(playableTools[i]);
            playableTool.rt.anchoredPosition = new Vector2(-(playableTools.Count - 1) * (r.i.interf.toolInGameSize.x / 2 + r.i.interf.spaceBetweenToolsInGame.x / 2) + i * (r.i.interf.toolInGameSize.x + r.i.interf.spaceBetweenToolsInGame.x), 0);
        }
        playableToolsBackdrop.SetSize(new Vector2(playableTools.Count * (r.i.interf.toolInGameSize.x + r.i.interf.spaceBetweenToolsInGame.x) + r.i.interf.spaceBetweenToolsInGame.x, r.i.interf.toolInGameSize.y + r.i.interf.spaceBetweenToolsInGame.y * 2));
        MovingObjects.instance.mo["PlayableTools"].StartMove("OnScreen");
    }
    public ToolInGame GetToolInGameMouseIsOver()
    {
        Vector2 mousePosition = r.i.interf.GetMousePosition();
        for (int i = 0; i < playerTools.Count; i++)
        {
            if (r.i.interf.IsPointInRectTransform(mousePosition, playerTools[i].rt, GameManager.instance.gameplayCanvas))
            {
                return playerTools[i];
            }
        }
        for (int i = 0; i < playerPlayableTools.Count; i++)
        {
            if (r.i.interf.IsPointInRectTransform(mousePosition, playerPlayableTools[i].rt, GameManager.instance.gameplayCanvas))
            {
                return playerPlayableTools[i];
            }
        }
        return null;
    }
}
