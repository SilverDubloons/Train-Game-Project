using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] private SlideOnMouseOver slideOnMouseOver;
    [SerializeField] private GameObject clickAgainToConfirmVisibilityObject;
    private List<ToolInGame> playerTools = new List<ToolInGame>();
    private List<ToolInGame> playerPlayableTools = new List<ToolInGame>();
    private HandEvaluation handEvaluation;
    public static Tools instance;
    private Dictionary<HandType, List<Card>> cardsForEachHandType = new Dictionary<HandType, List<Card>>();
    private IEnumerator waitForTargetOfSelectedToolCoroutine;
    private bool waitingForTargetOfSelectedTool;
    public void SetupInstance()
    {
        instance = this;
        handEvaluation = new HandEvaluation();
        for (int i = 0; i < r.i.tools.Length; i++)
        {
            AddNewTool(r.i.tools[i]);
        }
        SelectedCardsUpdated(0);
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
            if (!interactable || !CombatManager.instance.inCombat)
            {
                tool.SetInteractability(interactable);
            }
            else
            {
                tool.SetInteractabilityBasedOnAvailability();
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
    public void DeterminePlayableToolsFromSelectedCards(List<Card> selectedCards)
    {
        if (selectedCards == null || selectedCards.Count <= 0)
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
    public void DeterminePlayableToolsFromCardsInHand(List<Card> hand)
    {
        Logger.instance.Log($"DeterminePlayableToolsFromCardsInHand");
        cardsForEachHandType.Clear();
        foreach (ToolInGame toolInGame in playerTools)
        {
            toolInGame.SetCardsToSelectIfClicked(null);
        }
        if (hand == null || hand.Count == 0)
        {
            // disable interactability for all tools
            if(hand == null)
            {
                Logger.instance.Log($"Hand is null");
            }
            else if(hand.Count == 0)
            {
                Logger.instance.Log($"Hand is empty");
            }
            return;
        }
        Dictionary<int, List<Card>> cardsByRank = new Dictionary<int, List<Card>>();
        Dictionary<Suit, List<Card>> cardsBySuit = new Dictionary<Suit, List<Card>>();
        foreach (Card card in hand)
        {
            int rank = card.cardData.rank;
            Suit suit = card.cardData.suit;
            if (!cardsByRank.ContainsKey(rank))
            {
                cardsByRank[rank] = new List<Card>();
            }
            cardsByRank[rank].Add(card);
            if (!cardsBySuit.ContainsKey(suit))
            {
                cardsBySuit[suit] = new List<Card>();
            }
            cardsBySuit[suit].Add(card);
        }
        // List<KeyValuePair<int, List<Card>>
        List<KeyValuePair<int, List<Card>>> rankedGroups = cardsByRank.OrderByDescending(kvp => kvp.Value.Count).ThenByDescending(kvp => kvp.Key).ToList();
        ProcessXOfAKind(rankedGroups, hand.Count);
        ProcessHouseHands(rankedGroups, hand.Count);
        ProcessPairHands(rankedGroups, hand.Count);
        List<Card> cardsOfMostCommonSuit = new List<Card>();
        foreach (KeyValuePair<Suit, List<Card>> kvp in cardsBySuit)
        {
            if (kvp.Key != Suit.Rainbow && kvp.Value.Count > cardsOfMostCommonSuit.Count)
            {
                cardsOfMostCommonSuit = kvp.Value;
            }
        }
        if(cardsBySuit.ContainsKey(Suit.Rainbow) && cardsBySuit[Suit.Rainbow].Count > 0)
        {
            cardsOfMostCommonSuit.AddRange(cardsBySuit[Suit.Rainbow]);
        }
        foreach (ToolInGame toolInGame in playerTools)
        {
            toolInGame.SetCardsToSelectIfClicked(null);
            if (toolInGame.handType == HandType.Straight)
            {
                StraightFinder straightFinder = new StraightFinder();
                List<Card> straightCards = straightFinder.GetAppropriateStraight(
                    hand,
                    toolInGame.cardsRequired,
                    toolInGame.cardsRequired,
                    GameManager.instance.GetCanStraightsWrap(),
                    GameManager.instance.GetMaxGapLengthInStraights(),
                    GameManager.instance.GetMaxGapsInStraights(),
                    false
                );
                toolInGame.SetCardsToSelectIfClicked(straightCards);
                if (straightCards == null)
                {
                    continue;
                }
                for ( int i = 0; i < straightCards.Count; i++)
                {
                    Logger.instance.Log($"Straight card {i}: {straightCards[i].cardData.ToString()}");
                }
            }
            else if (toolInGame.handType == HandType.StraightFlush)
            {
                StraightFinder straightFinder = new StraightFinder();
                List<Card> straightFlushCards = straightFinder.GetAppropriateStraight(
                    hand,
                    toolInGame.cardsRequired,
                    toolInGame.cardsRequired,
                    GameManager.instance.GetCanStraightsWrap(),
                    GameManager.instance.GetMaxGapLengthInStraights(),
                    GameManager.instance.GetMaxGapsInStraights(),
                    true
                );
                toolInGame.SetCardsToSelectIfClicked(straightFlushCards);
            }
            else if (toolInGame.handType == HandType.Flush)
            {
                if(cardsOfMostCommonSuit.Count >= toolInGame.cardsRequired)
                {
                    toolInGame.SetCardsToSelectIfClicked(cardsOfMostCommonSuit.Take(toolInGame.cardsRequired).ToList());
                }
                else
                {
                    toolInGame.SetCardsToSelectIfClicked(null);
                }
            }
            else
            {
                if (cardsForEachHandType.ContainsKey(toolInGame.handType))
                {
                    toolInGame.SetCardsToSelectIfClicked(cardsForEachHandType[toolInGame.handType]);
                }
                else
                {
                    toolInGame.SetCardsToSelectIfClicked(null);
                }
            }
        }
        if(MovingObjects.instance.mo["Tools"].GetCurrentLocation() == "OnScreen")
        {
            SetToolsInteractability(true);
        }
    }
    private void ProcessXOfAKind(List<KeyValuePair<int, List<Card>>> rankedGroups, int handSize)
    {
        if (rankedGroups.Count == 0)
        {
            return;
        }

        var highestGroup = rankedGroups[0];
        int count = highestGroup.Value.Count;
        if (handSize >= 7 && count >= 7)
        {
            cardsForEachHandType[HandType.SevenOfAKind] = highestGroup.Value.Take(7).ToList();
        }
        if (handSize >= 6 && count >= 6)
        {
            cardsForEachHandType[HandType.SixOfAKind] = highestGroup.Value.Take(6).ToList();
        }
        if (handSize >= 5 && count >= 5)
        {
            cardsForEachHandType[HandType.FiveOfAKind] = highestGroup.Value.Take(5).ToList();
        }
        if (handSize >= 4 && count >= 4)
        {
            cardsForEachHandType[HandType.FourOfAKind] = highestGroup.Value.Take(4).ToList();
        }
        if (handSize >= 3 && count >= 3)
        {
            cardsForEachHandType[HandType.ThreeOfAKind] = highestGroup.Value.Take(3).ToList();
        }
    }

    private void ProcessHouseHands(List<KeyValuePair<int, List<Card>>> rankedGroups, int handSize)
    {
        if (rankedGroups.Count < 2)
        {
            return;
        }
        var first = rankedGroups[0];
        var second = rankedGroups[1];
        if (handSize >= 7 && first.Value.Count >= 5 && second.Value.Count >= 2)
        {
            var cards = new List<Card>();
            cards.AddRange(first.Value.Take(5));
            cards.AddRange(second.Value.Take(2));
            cardsForEachHandType[HandType.HugeHouse] = cards;
        }
        if (handSize >= 7 && first.Value.Count >= 4 && second.Value.Count >= 3)
        {
            var cards = new List<Card>();
            cards.AddRange(first.Value.Take(4));
            cards.AddRange(second.Value.Take(3));
            cardsForEachHandType[HandType.WideHouse] = cards;
        }
        if (handSize >= 6 && first.Value.Count >= 4 && second.Value.Count >= 2)
        {
            var cards = new List<Card>();
            cards.AddRange(first.Value.Take(4));
            cards.AddRange(second.Value.Take(2));
            cardsForEachHandType[HandType.StuffedHouse] = cards;
        }
        if (handSize >= 6 && first.Value.Count >= 3 && second.Value.Count >= 3)
        {
            var cards = new List<Card>();
            cards.AddRange(first.Value.Take(3));
            cards.AddRange(second.Value.Take(3));
            cardsForEachHandType[HandType.DoubleTriple] = cards;
        }
        if (handSize >= 5 && first.Value.Count >= 3 && second.Value.Count >= 2)
        {
            var cards = new List<Card>();
            cards.AddRange(first.Value.Take(3));
            cards.AddRange(second.Value.Take(2));
            cardsForEachHandType[HandType.FullHouse] = cards;
        }
    }

    private void ProcessPairHands(List<KeyValuePair<int, List<Card>>> rankedGroups, int handSize)
    {
        if (handSize >= 7 && rankedGroups.Count >= 3)
        {
            var first = rankedGroups[0];
            var second = rankedGroups[1];
            var third = rankedGroups[2];
            if (first.Value.Count >= 3 && second.Value.Count >= 2 && third.Value.Count >= 2)
            {
                var cards = new List<Card>();
                cards.AddRange(first.Value.Take(3));
                cards.AddRange(second.Value.Take(2));
                cards.AddRange(third.Value.Take(2));
                cardsForEachHandType[HandType.GuestHouse] = cards;
            }
        }
        if (handSize >= 6 && rankedGroups.Count >= 3)
        {
            var first = rankedGroups[0];
            var second = rankedGroups[1];
            var third = rankedGroups[2];

            if (first.Value.Count >= 2 && second.Value.Count >= 2 && third.Value.Count >= 2)
            {
                var cards = new List<Card>();
                cards.AddRange(first.Value.Take(2));
                cards.AddRange(second.Value.Take(2));
                cards.AddRange(third.Value.Take(2));
                cardsForEachHandType[HandType.TripleDouble] = cards;
            }
        }
        if (handSize >= 4 && rankedGroups.Count >= 2)
        {
            var first = rankedGroups[0];
            var second = rankedGroups[1];

            if (first.Value.Count >= 2 && second.Value.Count >= 2)
            {
                var cards = new List<Card>();
                cards.AddRange(first.Value.Take(2));
                cards.AddRange(second.Value.Take(2));
                cardsForEachHandType[HandType.TwoPair] = cards;
            }
        }
        if (handSize >= 2 && rankedGroups[0].Value.Count >= 2)
        {
            cardsForEachHandType[HandType.OnePair] = rankedGroups[0].Value.Take(2).ToList();
        }
    }
    public void SelectedCardsUpdated(int numberOfSelectedCards)
    {
        if (numberOfSelectedCards <= 0 || !CombatManager.instance.inCombat)
        {
            // Logger.instance.Log($"SelectedCardsUpdated numberOfSelectedCards={numberOfSelectedCards}, inCombat={CombatManager.instance.inCombat}, setting TRUE");
            slideOnMouseOver.SetInteractability(true);
            MovingObjects.instance.mo["Tools"].StartMove("OffScreenWithTab");
        }
        else
        {
            // Logger.instance.Log($"SelectedCardsUpdated numberOfSelectedCards={numberOfSelectedCards}, inCombat={CombatManager.instance.inCombat}, setting FALSE");
            slideOnMouseOver.SetInteractability(false);
            MovingObjects.instance.mo["Tools"].StartMove("OffScreen");
        }
    }
    public void AvailableToolSelected(ToolInGame selectedTool)
    { 
        if (waitingForTargetOfSelectedTool)
        {
            StopCoroutine(waitForTargetOfSelectedToolCoroutine);
        }
        waitForTargetOfSelectedToolCoroutine = WaitForTargetOfSelectedTool(selectedTool);
        StartCoroutine(waitForTargetOfSelectedToolCoroutine);
    }
    private IEnumerator WaitForTargetOfSelectedTool(ToolInGame selectedTool)
    {
        waitingForTargetOfSelectedTool = true;
        if (selectedTool.GetToolTargetStyle() == ToolTargetStyle.Self)
        {
            clickAgainToConfirmVisibilityObject.SetActive(true);
        }
        else
        {
            bool aiming = selectedTool.HasSpecialTag(ToolSpecialTag.AlwaysAim);
            CombatManager.instance.SetTargetingTool(selectedTool, aiming);
        }
        while (waitingForTargetOfSelectedTool)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                ToolInGame toolMouseIsOver = GetToolInGameMouseIsOver();
                if (toolMouseIsOver == selectedTool)
                {
                    if (selectedTool.GetToolTargetStyle() == ToolTargetStyle.Self)
                    {
                        CombatManager.instance.ApplySelfTool(selectedTool);
                    }
                }
                else if (toolMouseIsOver != null)
                {
                    // clicked on a different tool, make that the new selected tool

                }
                else
                {
                    // clicked somewhere that is not a tool, check if it's a valid target and if so use the tool on that target
                    // if it's not a valid target, deselect the tool

                }
                clickAgainToConfirmVisibilityObject.SetActive(false);
                waitingForTargetOfSelectedTool = false;
            }
            yield return null;
        }
    }
}
