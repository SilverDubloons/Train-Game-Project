using UnityEngine;
public class CardData
{
    public int rank;    // 0 - 12, duece through ace
    public Suit suit;   // 0 = spade, 1 = club, 2 = heart, 3 = diamond, 4 = rainbow
    public SpecialCardType specialCardType;
    public bool isSpecialCard => specialCardType != SpecialCardType.None;
    public CardData(int rank, Suit suit, SpecialCardType specialCardType = SpecialCardType.None)
    {
        this.rank = rank;
        this.suit = suit;
        this.specialCardType = specialCardType;
    }
    public override string ToString()
    {
        if (isSpecialCard)
        {
            return specialCardType.ToString();
        }
        return r.i.interf.ConvertRankAndSuitToString(rank, suit);
    }
}
public enum Suit
{
    Spade,
    Club,
    Heart,
    Diamond,
    Rainbow,
    Undefined
}
public enum SpecialCardType
{
    None,
    Aim
}