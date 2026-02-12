using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StraightFinder
{
    public List<StraightInfo> GetStraightsInHand(List<Card> hand, int minLength, int maxLength, bool canWrap, int largestGapAllowed, int maxGaps)
    { 
        List<StraightInfo> straightInfos = new List<StraightInfo>();
        Dictionary<int, List<Card>> cardsByRank = new Dictionary<int, List<Card>>();
        foreach (Card card in hand)
        {
            int rank = card.cardData.rank;
            if (!cardsByRank.ContainsKey(rank))
            { 
                cardsByRank[rank] = new List<Card>();
            }
            cardsByRank[rank].Add(card);
        }
        List<int> sortedRanks = cardsByRank.Keys.OrderBy(x => x).ToList();
        foreach (int rank in sortedRanks)
        {
            int startingRank = rank;
            int lowRank = rank;
            int highRank = rank;
            int totalGaps = 0;
            int lastRank = rank;
            List<int> currentStraight = new List<int>();
            for (int i = startingRank + 1; i < sortedRanks.Count; i++)
            {
                int currentRank = sortedRanks[i];
                int gap = currentRank - lastRank - 1;
                totalGaps += gap;
                if (gap <= largestGapAllowed && totalGaps <= maxGaps)
                { 
                    
                }
            }
        }


        return straightInfos;
    }
}
public class StraightInfo
{
    public List<Card> cards;
    public int length;
    public int gapsUsed;
    public int highRank;
    public int lowRank;
    public bool isWrapped;
    public int CompareTo(StraightInfo other)
    {
        if (other == null)
        {
            return 1;
        }
        if (length != other.length)
        {
            return other.length.CompareTo(length);
        }
        return isWrapped.CompareTo(other.isWrapped);
    }
}
