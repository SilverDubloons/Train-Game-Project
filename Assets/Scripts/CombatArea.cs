using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

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
    private Vector2Int currentBoardSize;
    

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
    public void SetPlayerPosition(Vector2Int spawnPosition)
    {
        player.SetPlayerPosition(currentCombatSpaces[spawnPosition.x, spawnPosition.y]);
    }
    public int PreviewSelectableTargets(ToolTargetStyle targetStyle, int adjacentColumnsTarget)
    {
        int targetableSpaces = 0;
        switch (targetStyle)
        {
            case ToolTargetStyle.EntireColumn:
                for (int x = Mathf.Max(0, player.currentSpace.gridPosition.x - adjacentColumnsTarget); x < Mathf.Min(currentCombatSpaces.GetLength(0), player.currentSpace.gridPosition.x + adjacentColumnsTarget); x++)
                {
                    for (int y = 1; y < currentCombatSpaces.GetLength(1); y++)
                    {
                        currentCombatSpaces[x, y].SetTargetable(true);
                        targetableSpaces++;
                    }
                }
                break;
            case ToolTargetStyle.LastInColumn:
                for (int x = Mathf.Max(0, player.currentSpace.gridPosition.x - adjacentColumnsTarget); x < Mathf.Min(currentCombatSpaces.GetLength(0), player.currentSpace.gridPosition.x + adjacentColumnsTarget); x++)
                {
                    for (int y = currentCombatSpaces.GetLength(1) - 1; y >= 1; y--)
                    {
                        if (currentCombatSpaces[x, y].IsTargetable())
                        {
                            currentCombatSpaces[x, y].SetTargetable(true);
                            targetableSpaces++;
                            break;
                        }
                    }
                }
                break;
            case ToolTargetStyle.FirstInColumn:
                for (int x = Mathf.Max(0, player.currentSpace.gridPosition.x - adjacentColumnsTarget); x < Mathf.Min(currentCombatSpaces.GetLength(0), player.currentSpace.gridPosition.x + adjacentColumnsTarget); x++)
                {
                    for (int y = 1; y < currentCombatSpaces.GetLength(1); y++)
                    {
                        if (currentCombatSpaces[x, y].IsTargetable())
                        {
                            currentCombatSpaces[x, y].SetTargetable(true);
                            targetableSpaces++;
                            break;
                        }
                    }
                }
                break;
            case ToolTargetStyle.AnyInColumn:
                for (int x = Mathf.Max(0, player.currentSpace.gridPosition.x - adjacentColumnsTarget); x < Mathf.Min(currentCombatSpaces.GetLength(0), player.currentSpace.gridPosition.x + adjacentColumnsTarget); x++)
                {
                    for (int y = 1; y < currentCombatSpaces.GetLength(1); y++)
                    {
                        if (currentCombatSpaces[x, y].IsTargetable())
                        {
                            currentCombatSpaces[x, y].SetTargetable(true);
                            targetableSpaces++;
                        }
                    }
                }
                break;
        }
        return targetableSpaces;
    }
    public void EndTargetPreview()
    {
        for (int x = 0; x < currentCombatSpaces.GetLength(0); x++)
        {
            for (int y = 1; y < currentCombatSpaces.GetLength(1); y++)
            {
                currentCombatSpaces[x, y].SetTargetable(false);
            }
        }
    }
}