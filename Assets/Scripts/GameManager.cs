using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Canvas gameplayCanvas;
    public Camera gameplayCamera;
    public static GameManager instance;
    public void SetupInstance()
    {
        instance = this;
    }
    public int GetMaxHandSize()
    {
        return 15;
    }
    public int GetMaxGapInStraights()
    {
        return 0;
    }
    public bool GetCanStraightsWrap()
    {
        return false;
    }
    public bool GetSuitCanMovePlayer(Suit suit)
    { 
        switch(suit)
        {
            case Suit.Diamond:
                return true;
            case Suit.Rainbow:
                return true;
            default:
                return false;
        }
    }
    public List<CombatSpace> GetSpacesPlayerCanMoveToByHand(List<Card> handCards)
    {
        if(handCards == null || handCards.Count <= 0)
        {
            return null;
        }
        int totalMovement = 0;
        foreach (Card card in handCards)
        {
            if (GetSuitCanMovePlayer(card.cardData.suit))
            {
                totalMovement++;
            }
            else
            {
                return null;
            }
        }
        List<CombatSpace> movableSpaces = new List<CombatSpace>();
        CombatSpace playerSpace = CombatArea.instance.GetPlayerSpace();
        CombatSpace leftSpace = CombatArea.instance.GetCombatSpaceAtPosition(playerSpace.gridPosition + new Vector2Int(-totalMovement, 0));
        if (leftSpace != null)
        {
            movableSpaces.Add(leftSpace);
        }
        CombatSpace rightSpace = CombatArea.instance.GetCombatSpaceAtPosition(playerSpace.gridPosition + new Vector2Int(totalMovement, 0));
        if(rightSpace != null)
        {
            movableSpaces.Add(rightSpace);
        }
        if (movableSpaces.Count <= 0)
        {
            return null;
        }
        return movableSpaces;
    }
}
