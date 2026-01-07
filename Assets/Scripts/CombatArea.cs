using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class CombatArea : MonoBehaviour
{
    // [SerializeField] private ButtonPlus startCombatButton;
    [SerializeField] private Backdrop backdrop;
    [SerializeField] private GameObject combatSpacePrefab;
    [SerializeField] private RectTransform combatSpaceParent;
    private List<CombatSpace> combatSpaces = new List<CombatSpace>();
    private List<CombatSpace> currentCombatSpaces = new List<CombatSpace>();
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
                if (!currentCombatSpaces.Contains(newSpace))
                {
                    currentCombatSpaces.Add(newSpace);
                }
            }
        }
        if (combatSpaces.Count > combatSpaceIndex)
        {
            combatSpaces[combatSpaceIndex].SetVisibility(false);
            if(currentCombatSpaces.Contains(combatSpaces[combatSpaceIndex]))
            {
                currentCombatSpaces.Remove(combatSpaces[combatSpaceIndex]);
            }
        }
    }
    public CombatSpace GetSpawnSpaceForEnemy(Enemy enemy)
    { 
        return enemy.GetSpawnSpace(currentCombatSpaces, currentBoardSize);
    }
}