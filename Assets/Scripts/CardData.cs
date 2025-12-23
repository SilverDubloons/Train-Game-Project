using UnityEngine;
public class CardData
{
    public int rank;    // 0 - 12, duece through ace
    public Suit suit;   // 0 = spade, 1 = club, 2 = heart, 3 = diamond, 4 = rainbow
    public CardData(int rank, Suit suit)
    {
        this.rank = rank;
        this.suit = suit;
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