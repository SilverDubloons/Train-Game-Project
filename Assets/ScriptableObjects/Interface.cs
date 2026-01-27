using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

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
    public Vector2 toolInGameSize;
    public Vector2 spaceBetweenToolsInGame;
    public Suit[] suitOrder = new Suit[5] { Suit.Spade, Suit.Club, Suit.Heart, Suit.Diamond, Suit.Rainbow };
    public Color[] suitColors;
    public Dictionary<Suit, int> SuitOrderDictionary = new Dictionary<Suit, int>();
    public Sprite[] rankSprites;
    public Sprite[] suitSprites; // Spade, Club, Heart, Diamond, Rainbow
    public Sprite[] detailSprites;
    public Vector2 selectedCardOffset;
    public float maxTooltipWidth;
    public Sprite[] directionToMoveIcons; // none, then starting north and moving clockwise
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
    public string ConvertIntegerToString(int val, bool capitalized = false)
    {
        string intString = "intError";
        switch (val)
        {
            case 0:
                intString = "zero";
                break;
            case 1:
                intString = "one";
                break;
            case 2:
                intString = "two";
                break;
            case 3:
                intString = "three";
                break;
            case 4:
                intString = "four";
                break;
            case 5:
                intString = "five";
                break;
            case 6:
                intString = "six";
                break;
            case 7:
                intString = "seven";
                break;
            case 8:
                intString = "eight";
                break;
            case 9:
                intString = "nine";
                break;
            case 10:
                intString = "ten";
                break;
        }
        if (capitalized)
        {
            string capitalizedIntString = char.ToUpper(intString[0]) + intString[1..];
            return capitalizedIntString;
        }
        else
        {
            return intString;
        }
    }
    public Vector2 GetCanvasPositionOfRectTransform(RectTransform rectTransform, Canvas canvas)
    {
        Vector3[] worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners);
        Vector3 worldCenter = (worldCorners[0] + worldCorners[2]) * 0.5f;
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, worldCenter);
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, canvas.worldCamera, out localPoint);
        return localPoint;
    }
    public string GetFixedLengthIntString(int val, int length)
    {
        string intString = val.ToString();
        while (intString.Length < length)
        {
            intString = "0" + intString;
        }
        return intString;
    }
    public bool IsPointInRectTransform(Vector2 point, RectTransform rectTransform, Canvas canvas)
    {
        Vector3[] worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners);
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 bottomLeftScreenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, worldCorners[0]);
        Vector2 bottomLeft = RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, bottomLeftScreenPoint, canvas.worldCamera, out bottomLeft) ? bottomLeft : Vector2.zero;
        Vector2 topRightScreenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, worldCorners[2]);
        Vector2 topRight = RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, topRightScreenPoint, canvas.worldCamera, out topRight) ? topRight : Vector2.zero;
        if (point.x >= bottomLeft.x && point.x <= topRight.x && point.y >= bottomLeft.y && point.y <= topRight.y)
        {
            // Logger.instance.Log($"Interface.IsPointInRectTransform: {rectTransform.name}, point: {point}, bottomLeft: {bottomLeft}, topRight: {topRight} TRUE");
            return true;
        }
        // Logger.instance.Log($"Interface.IsPointInRectTransform: {rectTransform.name}, point: {point}, bottomLeft: {bottomLeft}, topRight: {topRight} FALSE");
        return false;
    }
    public string ConvertDirectionToMoveToString(DirectionToMove directionToMove)
    {
        return directionToMove switch
        {
            DirectionToMove.None => "None",
            DirectionToMove.Up => "Up",
            DirectionToMove.UpRight => "Up & Right",
            DirectionToMove.Right => "Right",
            DirectionToMove.DownRight => "Down & Right",
            DirectionToMove.Down => "Down",
            DirectionToMove.DownLeft => "Down & Left",
            DirectionToMove.Left => "Left",
            DirectionToMove.UpLeft => "Up & Left",
            _ => "ERROR",
        };
    }
    public Sprite ConvertDirectionToMoveToSprite(DirectionToMove directionToMove)
    {
        return directionToMove switch
        {
            DirectionToMove.None => directionToMoveIcons[0],
            DirectionToMove.Up => directionToMoveIcons[1],
            DirectionToMove.UpRight => directionToMoveIcons[2],
            DirectionToMove.Right => directionToMoveIcons[3],
            DirectionToMove.DownRight => directionToMoveIcons[4],
            DirectionToMove.Down => directionToMoveIcons[5],
            DirectionToMove.DownLeft => directionToMoveIcons[6],
            DirectionToMove.Left => directionToMoveIcons[7],
            DirectionToMove.UpLeft => directionToMoveIcons[8],
            _ => directionToMoveIcons[0],
        };
    }
}
