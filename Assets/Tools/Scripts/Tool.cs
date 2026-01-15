using UnityEngine;

[CreateAssetMenu(fileName = "Tool", menuName = "Scriptable Objects/Tool")]
public class Tool : ScriptableObject
{
    public string toolName;
    public string toolTag;
    public string toolDescription;
    public Sprite toolIcon;
    //public CardRequirement[] cardRequirements;
    public HandType handStyle;
    public int cardsRequired;
    public ToolTargetStyle toolTargetStyle;
    public int adjacentColumnsTarget = 0; // 1 means player can target column plus one column on each side etc
    public int areaOfEffectRadius = 0; // 1 means target plus adjacent tiles in all 4 cardinal directions
    public int damage;
    public ToolSpecialTags[] toolSpecialTags;
    public string GetHandStyleString()
    {
        switch (handStyle)
        {
            case HandType.HighCard:
                return "High Card";
            case HandType.OnePair:
                return "One Pair";
            case HandType.TwoPair:
                return "Two Pair";
            case HandType.ThreeOfAKind:
                return "Three of a Kind";
            case HandType.Straight:
                return r.i.interf.ConvertIntegerToString(cardsRequired, true) + " Card Straight";
            case HandType.Flush:
                return r.i.interf.ConvertIntegerToString(cardsRequired, true) + " Card Flush";
            case HandType.FullHouse:
                return "Full House";
            case HandType.FourOfAKind:
                return "Four of a Kind";
            case HandType.StraightFlush:
                return r.i.interf.ConvertIntegerToString(cardsRequired, true) + " Card Straight Flush";
            case HandType.FiveOfAKind:
                return "Five of a Kind";
            case HandType.TripleDouble:
                return "Triple Double";
            case HandType.DoubleTriple:
                return "DoubleTriple";
            case HandType.StuffedHouse:
                return "Stuffed House";
            case HandType.SixOfAKind:
                return "Six of a Kind";
            case HandType.GuestHouse:
                return "Guest House";
            case HandType.WideHouse:
                return "Wide House";
            case HandType.HugeHouse:
                return "Huge House";
            case HandType.SevenOfAKind:
                return "Seven of a Kind";
            default:
                return "GetHandStyleString ERROR";
        }
    }
}
public enum HandType
{ 
    HighCard,
    OnePair,
    TwoPair,
    ThreeOfAKind,
    Straight,
    Flush,
    FullHouse,
    FourOfAKind,
    StraightFlush,
    RoyalFlush,
    FiveOfAKind,
    TripleDouble,
    DoubleTriple,
    StuffedHouse,
    SixOfAKind,
    GuestHouse,
    WideHouse,
    HugeHouse,
    SevenOfAKind
}
public enum ToolTargetStyle
{ 
    FirstInColumn,
    LastInColumn,
    EntireColumn,
    AnyInColumn
}
public enum ToolSpecialTags
{ 
    AlwaysAim,
    DoubleDamageFrontRow,
    DoubleDamageBackRow
}