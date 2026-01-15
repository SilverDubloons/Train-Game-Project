using UnityEngine;
[System.Serializable]
public class CardRequirement
{
    public RankRequirement rankRequirement;
    public SuitRequirement suitRequirement;
}

public enum RankRequirement
{ 
    None,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    Ace,
    Numbered,
    Even,
    Odd,
    FaceCard,
    FaceOrAce
}
public enum SuitRequirement
{
    None,
    Spade,
    Club,
    Heart,
    Diamond,
    Rainbow,
    SpadeOrClub,
    HeartOrDiamond
}