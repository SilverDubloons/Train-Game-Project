using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

public class CombatArea : MonoBehaviour
{
    // [SerializeField] private ButtonPlus startCombatButton;
    [SerializeField] private Backdrop backdrop;
    [SerializeField] private GameObject combatSpacePrefab;
    [SerializeField] private RectTransform combatSpaceParent;
    [SerializeField] private Player player;
    private List<CombatSpace> combatSpaces = new List<CombatSpace>();
    // private List<CombatSpace> currentCombatSpaces = new List<CombatSpace>();
    private CombatSpace[,] currentCombatSpaces;
    public static CombatArea instance;
    public Vector2Int currentBoardSize;
    

    public void SetupInstance()
    {
        instance = this;
    }
    public void SetInteractability(bool interactable)
    {
        // startCombatButton.SetButtonEnabled(interactable);
    }
    public void SetVisibility(bool visible)
    {
        backdrop.SetVisibility(visible);
    }
    public void SetupBoard(Vector2Int boardSize)
    {
        currentBoardSize = boardSize;
        int combatSpaceIndex = 0;
        currentCombatSpaces = new CombatSpace[boardSize.x, boardSize.y];
        for (int x = 0; x < boardSize.x; x++)
        {
            for (int y = 0; y < boardSize.y; y++)
            {
                CombatSpace newSpace;
                if (combatSpaces.Count > combatSpaceIndex)
                {
                    newSpace = combatSpaces[combatSpaceIndex];
                    newSpace.SetVisibility(true);
                }
                else
                {
                    newSpace = Instantiate(combatSpacePrefab, combatSpaceParent).GetComponent<CombatSpace>();
                    combatSpaces.Add(newSpace);
                }
                combatSpaceIndex++;
                newSpace.name = $"Combat Space ({x}, {y})";
                newSpace.SetPosition(new Vector2((boardSize.x - 1) * (-r.i.interf.combatSpaceSize.x / 2f - r.i.interf.distanceBetweenCombatSpaces.x / 2f) + r.i.interf.combatSpaceSize.x * x + r.i.interf.distanceBetweenCombatSpaces.x * x, (boardSize.y - 1) * (-r.i.interf.combatSpaceSize.y / 2f - r.i.interf.distanceBetweenCombatSpaces.y / 2f) + r.i.interf.combatSpaceSize.y * y + r.i.interf.distanceBetweenCombatSpaces.y * y));
                newSpace.gridPosition = new Vector2Int(x, y);
                newSpace.SetInteractability(false);
                /*if (!currentCombatSpaces.Contains(newSpace))
                {
                    currentCombatSpaces.Add(newSpace);
                }*/
                currentCombatSpaces[x, y] = newSpace;
            }
        }
        if (combatSpaces.Count > combatSpaceIndex)
        {
            combatSpaces[combatSpaceIndex].SetVisibility(false);
            /*if(currentCombatSpaces.Contains(combatSpaces[combatSpaceIndex]))
            {
                currentCombatSpaces.Remove(combatSpaces[combatSpaceIndex]);
            }*/
        }
    }
    public CombatSpace GetSpawnSpaceForEnemy(Enemy enemy)
    { 
        return enemy.GetSpawnSpace(currentCombatSpaces);
    }
    public void SetPlayerPosition(Vector2Int newPosition)
    {
        player.SetPlayerPosition(currentCombatSpaces[newPosition.x, newPosition.y]);
    }
    public void SetPlayerPosition(CombatSpace combatSpace)
    {
        player.SetPlayerPosition(combatSpace);
    }
    public int PreviewSelectableTargets(ToolTargetStyle targetStyle, int adjacentColumnsTarget, bool aiming)
    {
        Logger.instance.Log($"Previewing selectable targets with style {targetStyle}, adjacent columns {adjacentColumnsTarget}, aiming {aiming}");
        CombatSpace playerSpace = player.GetCurrentSpace();
        if (!CombatManager.instance.inCombat)
        {
            return -1;
        }
        int targetableSpaces = 0;
        int leftMostColumn = Mathf.Max(0, playerSpace.gridPosition.x - adjacentColumnsTarget);
        int rightMostColumn = Mathf.Min(currentCombatSpaces.GetLength(0), playerSpace.gridPosition.x + adjacentColumnsTarget);
        for (int x = leftMostColumn; x <= rightMostColumn; x++)
        {
            switch (targetStyle)
            {
                case ToolTargetStyle.EntireColumn:
                    for (int y = 1; y < currentCombatSpaces.GetLength(1); y++)
                    {
                        currentCombatSpaces[x, y].SetTargetable(true, aiming);
                        targetableSpaces++;
                    }
                break;
                case ToolTargetStyle.LastInColumn:
                    for (int y = currentCombatSpaces.GetLength(1) - 1; y >= 1; y--)
                    {
                        if (currentCombatSpaces[x, y].IsTargetable())
                        {
                            currentCombatSpaces[x, y].SetTargetable(true, aiming);
                            targetableSpaces++;
                            break;
                        }
                    }
                break;
                case ToolTargetStyle.FirstInColumn:
                    for (int y = 1; y < currentCombatSpaces.GetLength(1); y++)
                    {
                        if (currentCombatSpaces[x, y].IsTargetable())
                        {
                            currentCombatSpaces[x, y].SetTargetable(true, aiming);
                            targetableSpaces++;
                            break;
                        }
                    }
                break;
                case ToolTargetStyle.AnyInColumn:
                    for (int y = 1; y < currentCombatSpaces.GetLength(1); y++)
                    {
                        if (currentCombatSpaces[x, y].IsTargetable())
                        {
                            currentCombatSpaces[x, y].SetTargetable(true, aiming);
                            targetableSpaces++;
                        }
                    }
                break;
            }
        }
        return targetableSpaces;
    }
    public void EndTargetPreview()
    {
        Logger.instance.Log("Ending target preview");
        for (int x = 0; x < currentCombatSpaces.GetLength(0); x++)
        {
            for (int y = 1; y < currentCombatSpaces.GetLength(1); y++)
            {
                currentCombatSpaces[x, y].SetTargetable(false, false);
            }
        }
    }
    public CombatSpace GetCombatSpaceAtPosition(Vector2Int position)
    {
        if(position.x < 0 || position.x >= currentCombatSpaces.GetLength(0) || position.y < 0 || position.y >= currentCombatSpaces.GetLength(1))
        {
            return null;
        }
        return currentCombatSpaces[position.x, position.y];
    }
    public bool IsPositionInCombatArea(Vector2Int position)
    {
        return position.x >= 0 && position.x < currentCombatSpaces.GetLength(0) && position.y >= 0 && position.y < currentCombatSpaces.GetLength(1);
    }
    public CombatSpace GetPlayerSpace()
    {
        return player.GetCurrentSpace();
    }
    public void SetMovableSpaces(List<CombatSpace> movableSpaces)
    {
        if (movableSpaces == null)
        {
            for (int i = 0; i < currentCombatSpaces.GetLength(0); i++)
            {
                currentCombatSpaces[i, 0].SetInteractability(false);
            }
            return;
        }
        for (int i = 0; i < currentCombatSpaces.GetLength(0); i++)
        {
            currentCombatSpaces[i, 0].SetInteractability(movableSpaces.Contains(currentCombatSpaces[i, 0]));
        }
    }
    public CombatSpace[,] GetCurrentCombatSpaces()
    {
        return currentCombatSpaces;
    }
}