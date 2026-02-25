using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StraightFinder
{
    public List<Card> GetAppropriateStraight(List<Card> hand, int minLength, int maxLength, bool canWrap, int largestGapAllowed, int maxGaps, bool straightFlush)
    { 
        // Logger.instance.Log($"GetAppropriateStraight hand.Count:{hand.Count}, minLength={minLength}, maxLength={maxLength}, canWrap={canWrap}, largestGapAllowed={largestGapAllowed}, maxGaps={maxGaps}, straightFlush={straightFlush}");
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
        // Logger.instance.Log($"sortedRanks.Count={sortedRanks.Count}");
        // foreach (int rank in sortedRanks)
        for(int i = 0; i < sortedRanks.Count; i++)
        {
            int startingRank = sortedRanks[i];
            // Logger.instance.Log("Checking rank:" + r.i.interf.RankToString(startingRank));
            int totalGaps = 0;
            int lastRank = sortedRanks[i];
            List<int> currentStraight = new List<int>();
            currentStraight.Add(startingRank);
            for (int j = i + 1; j < sortedRanks.Count && currentStraight.Count < maxLength; j++)
            {
                int currentRank = sortedRanks[j];
                // Logger.instance.Log("Checking next rank:" + r.i.interf.RankToString(currentRank));
                int gap = currentRank - lastRank - 1;
                if (gap <= largestGapAllowed && totalGaps + gap <= maxGaps)
                {
                    lastRank = currentRank;
                    totalGaps += gap;
                    currentStraight.Add(currentRank);
                }
                else
                {
                    break;
                }
            }
            if (currentStraight.Count >= minLength)
            {
                straightInfos.Add(new StraightInfo(currentStraight, totalGaps, false));
                if (!straightFlush && currentStraight.Count == minLength && currentStraight.Count == maxLength)
                {
                    return GetLowestStraight(straightInfos, cardsByRank);
                }
            }
            bool straightModified = false;
            if (startingRank == 12 || canWrap)
            {
                for(int j = 0; j < sortedRanks.Count && currentStraight.Count < maxLength; j++)
                {
                    int currentRank = sortedRanks[j];
                    if (currentRank == 12)
                    {
                        break; // so we don't consider the starting Ace again if we started with it, and so we don't consider wrapping to Ace
                    }
                    // Logger.instance.Log("Checking next rank (Ace or wrapping):" + r.i.interf.RankToString(currentRank));
                    int gap;
                    if (j == 0)
                    {
                        gap = (currentRank + 13) - lastRank - 1;
                    }
                    else
                    {
                        gap = currentRank - lastRank - 1;
                    }
                    if (gap <= largestGapAllowed && totalGaps + gap <= maxGaps)
                    {
                        lastRank = currentRank;
                        totalGaps += gap;
                        currentStraight.Add(currentRank);
                        straightModified = true;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (straightModified && currentStraight.Count >= minLength)
            {
                straightInfos.Add(new StraightInfo(currentStraight, totalGaps, false));
                if (!straightFlush && currentStraight.Count == minLength && currentStraight.Count == maxLength)
                {
                    return GetLowestStraight(straightInfos, cardsByRank);
                }
            }
        }
        if (straightInfos.Count <= 0)
        {
            return null;
        }
        straightInfos.Sort((a, b) =>
        {
            return a.CompareTo(b);
        });
        if (!straightFlush)
        {
            return GetLowestStraight(straightInfos, cardsByRank);
        }
        else
        { 
            List<Suit> suits = new List<Suit>() { Suit.Club, Suit.Diamond, Suit.Heart, Suit.Spade };
            for (int i = 0; i < straightInfos.Count; i++)
            {
                foreach (Suit suit in suits)
                {
                    List<Card> straightFlushCards = new List<Card>();
                    for (int j = 0; j < straightInfos[i].ranks.Count; j++)
                    {
                        if (cardsByRank[straightInfos[i].ranks[j]].Any(x => x.cardData.suit == suit || x.cardData.suit == Suit.Rainbow))
                        {
                            straightFlushCards.Add(cardsByRank[straightInfos[i].ranks[j]].First(x => x.cardData.suit == suit || x.cardData.suit == Suit.Rainbow));
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (straightFlushCards.Count == straightInfos[i].ranks.Count)
                    {
                        return straightFlushCards;
                    }
                }
            }
        }
        return null;
    }
    public List<Card> GetLowestStraight(List<StraightInfo> straightInfos, Dictionary<int, List<Card>> cardsByRank)
    {
        straightInfos.Sort((a, b) =>
        {
            return a.CompareTo(b);
        });
        List<Card> bestStraight = new List<Card>();
        foreach (int rank in straightInfos[0].ranks)
        {
            bestStraight.Add(cardsByRank[rank][0]);
        }
        return bestStraight;
    }
}
public class StraightInfo
{
    public List<int> ranks;
    public int gapsUsed;
    public bool isWrapped;
    public int CompareTo(StraightInfo other)
    {
        if (other == null)
        {
            return 1;
        }
        int index = 0;
        while (index < ranks.Count && index < other.ranks.Count)
        {
            if (ranks[ranks.Count - 1 - index] != other.ranks[other.ranks.Count - 1 - index])
            {
                return ranks[ranks.Count - 1 - index].CompareTo(other.ranks[other.ranks.Count - 1 - index]);
            }
            index++;
        }
        if (ranks.Count != other.ranks.Count)
        {
            return other.ranks.Count.CompareTo(ranks.Count);
        }
        return isWrapped.CompareTo(other.isWrapped);
    }
    public StraightInfo(List<int> ranks, int gapsUsed, bool isWrapped)
    {
        this.ranks = ranks;
        this.gapsUsed = gapsUsed;
        this.isWrapped = isWrapped;
    }
}
