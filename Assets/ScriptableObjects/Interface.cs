using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Unity 6000.3.2f1 LTS
[CreateAssetMenu(menuName = "UI/Interface")]
public class Interface : ScriptableObject
{
    public Sprite backdrop4pxCorners;
    public Sprite backdrop8pxCorners;
    public Sprite shadow4pxCorners;
    public Sprite shadow8pxCorners;
    public Vector2 referenceResolution;
    public float animationDuration = 1f;
    public AnimationCurve animationCurve;
    public Vector2 combatSpaceSize;
    public Vector2 distanceBetweenCombatSpaces;
    public Vector2 maxEnemySize;
    public Suit[] suitOrder = new Suit[5] { Suit.Spade, Suit.Club, Suit.Heart, Suit.Diamond, Suit.Rainbow };
    public Color[] suitColors;
    public Dictionary<Suit, int> SuitOrderDictionary = new Dictionary<Suit, int>();
    public Sprite[] rankSprites;
    public Sprite[] suitSprites; // Spade, Club, Heart, Diamond, Rainbow
    public Sprite[] detailSprites;
    public void InitialSetup()
    {
        SetupSuitOrderDictionary();
    }
    public void SetupSuitOrderDictionary()
    {
        Dictionary<Suit, int> suitOrderDict = new Dictionary<Suit, int>();
        for (int i = 0; i < suitOrder.Length; i++)
        {
            suitOrderDict[suitOrder[i]] = i;
        }
    }
    public Vector2 GetMousePosition()
    {   // return mouse position in reference resolution space centered at (0,0)
        // i.e. bottom left is (-referenceResolution.x/2, -referenceResolution.y/2)
        //      top right is (referenceResolution.x/2, referenceResolution.y/2)
        Vector2 mousePos = new Vector2((Mouse.current.position.ReadValue().x / Screen.width) * referenceResolution.x - referenceResolution.x / 2, (Mouse.current.position.ReadValue().y / Screen.height) * referenceResolution.y - referenceResolution.y / 2);
        return mousePos;
    }
    public string ConvertRankAndSuitToString(int rank, Suit suit)
    {
        string cardString = string.Empty;
        switch (rank)
        {
            case 0:
                cardString += "2";
                break;
            case 1:
                cardString += "3";
                break;
            case 2:
                cardString += "4";
                break;
            case 3:
                cardString += "5";
                break;
            case 4:
                cardString += "6";
                break;
            case 5:
                cardString += "7";
                break;
            case 6:
                cardString += "8";
                break;
            case 7:
                cardString += "9";
                break;
            case 8:
                cardString += "T";
                break;
            case 9:
                cardString += "J";
                break;
            case 10:
                cardString += "Q";
                break;
            case 11:
                cardString += "K";
                break;
            case 12:
                cardString += "A";
                break;
            default:
                cardString += "?";
                break;
        }
        switch (suit)
        {
            case Suit.Spade:
                cardString += "s";
                break;
            case Suit.Club:
                cardString += "c";
                break;
            case Suit.Heart:
                cardString += "h";
                break;
            case Suit.Diamond:
                cardString += "d";
                break;
            case Suit.Rainbow:
                cardString += "r";
                break;
            default:
                cardString += "?";
                break;
        }
        return cardString;
    }
    public int SuitToInt(Suit suit)
    {
        switch (suit)
        {
            case Suit.Spade:
                return 0;
            case Suit.Club:
                return 1;
            case Suit.Heart:
                return 2;
            case Suit.Diamond:
                return 3;
            case Suit.Rainbow:
                return 4;
            default:
                return -1;
        }
    }   
}
