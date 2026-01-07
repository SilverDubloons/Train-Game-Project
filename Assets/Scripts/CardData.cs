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
}
public enum Suit
{
    Spade,
    Club,
    Heart,
    Diamond,
    Rainbow
}
public enum SpecialCardType
{
    None,
    Aim
}