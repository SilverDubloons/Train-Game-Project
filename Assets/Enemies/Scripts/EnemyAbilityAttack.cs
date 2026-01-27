using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Build;
[CreateAssetMenu(fileName = "EnemyAbilityAttack", menuName = "Combat/EnemyAbilities/Enemy Ability Attack")]
public class EnemyAbilityAttack : EnemyAbility
{
    public int damage;
    public bool canBeUsedFromFrontRow;
    public bool canBeUsedFromBackRow;
    public int[] affectedColumns; // relative to enemy position, eg 0 is enemy column, -1 is one to the left, 1 is one to the right
    public AttackAvailabilityData GetAttackAvailabilityData(EnemyInGame enemy)
    {
        List<CombatSpace> targetableSpaces = new List<CombatSpace>();
        AttackAvailabilityData attackAvailabilityData = new AttackAvailabilityData(false, false, targetableSpaces);
        CombatSpace currentCombatSpace = enemy.GetCurrentCombatSpace();
        if ((currentCombatSpace.gridPosition.y == 1 && !canBeUsedFromFrontRow) ||
           (currentCombatSpace.gridPosition.y == 2 && !canBeUsedFromBackRow))
        {
            return attackAvailabilityData;
        }
        bool atLeastOneTargetable = false;
        for (int i = 0; i < affectedColumns.Length; i++)
        {
            int targetColumn = currentCombatSpace.gridPosition.x + affectedColumns[i];
            if (CombatArea.instance.IsPositionInCombatArea(new Vector2Int(targetColumn, 0)))
            {
                atLeastOneTargetable = true;
                targetableSpaces.Add(CombatArea.instance.GetCombatSpaceAtPosition(new Vector2Int(targetColumn, 0)));
                if(CombatArea.instance.GetCombatSpaceAtPosition(new Vector2Int(targetColumn, 0)) == CombatArea.instance.GetPlayerSpace())
                {
                    attackAvailabilityData.playerInRange = true;
                }
            }
        }
        if (!atLeastOneTargetable)
        {
            return attackAvailabilityData;
        }
        if (!enemy.EnemyMeetsLimbRequirements(limbRequirements))
        {
            return attackAvailabilityData;
        }
        attackAvailabilityData.isAvailable = true;
        attackAvailabilityData.targetableSpaces = targetableSpaces;
        return attackAvailabilityData;
    }
    public override List<TooltipData> GetTooltipDataList()
    { 
        List<TooltipData> tooltipDataList = new List<TooltipData>();
        
        return tooltipDataList;
    }
    public override AbilityType GetAbilityType()
    {
        return AbilityType.Attack;
    }
    public DirectionAndConfident GetBestDirectionToMove(EnemyInGame enemy, CombatSpace combatSpace) // i = column to consider
    {
        DirectionAndConfident directionAndConfident = new DirectionAndConfident(DirectionToMove.None, false);
        for (int i = 0; i < affectedColumns.Length; i++)
        {
            directionAndConfident = GetBestDirectionToMoveGivenColumn(enemy, combatSpace, i);
            if (directionAndConfident.confident)
            { 
                return directionAndConfident;
            }
        }
        return directionAndConfident;
    }
    public DirectionAndConfident GetBestDirectionToMoveGivenColumn(EnemyInGame enemy, CombatSpace combatSpace, int columnToConsider) // i = column to consider
    {
        if (!enemy.EnemyMeetsLimbRequirements(limbRequirements))
        {
            return new DirectionAndConfident(DirectionToMove.None, false);
        }
        if (affectedColumns.Length == 0)
        {
            return new DirectionAndConfident(DirectionToMove.None, false);
        }
        int targetColumn = combatSpace.gridPosition.x + affectedColumns[columnToConsider];
        if (targetColumn < 0 || targetColumn >= CombatArea.instance.currentBoardSize.x)
        {
            return new DirectionAndConfident(DirectionToMove.None, false);
        }
        int currentRow = combatSpace.gridPosition.y;
        int playerColumn = CombatArea.instance.GetPlayerSpace().gridPosition.x;
        bool canMoveDiagonally = enemy.GetBaseEnemy().canMoveDiagonally;
        if (targetColumn == playerColumn)
        {
            if (currentRow == 1) // front row
            {
                bool enemyInSpaceUp = CombatArea.instance.GetCombatSpaceAtPosition(new Vector2Int(combatSpace.gridPosition.x, combatSpace.gridPosition.y + 1)).EnemyInSpace();
                if (canBeUsedFromFrontRow && !canBeUsedFromBackRow)
                {
                    return new DirectionAndConfident(DirectionToMove.None, true);
                }
                if (canBeUsedFromBackRow && !canBeUsedFromFrontRow)
                {
                    return new DirectionAndConfident(DirectionToMove.Up, !enemyInSpaceUp);
                }
                else if (canBeUsedFromBackRow)
                {
                    return new DirectionAndConfident(DirectionToMove.Up, !enemyInSpaceUp);
                }
                return new DirectionAndConfident(DirectionToMove.None, false);
            }
            else // back row
            {
                bool enemyInSpaceDown = CombatArea.instance.GetCombatSpaceAtPosition(new Vector2Int(combatSpace.gridPosition.x, combatSpace.gridPosition.y - 1)).EnemyInSpace();
                if (canBeUsedFromBackRow && !canBeUsedFromFrontRow)
                {
                    return new DirectionAndConfident(DirectionToMove.None, true);
                }
                else if (canBeUsedFromFrontRow && !canBeUsedFromBackRow)
                {
                    return new DirectionAndConfident(DirectionToMove.Down, !enemyInSpaceDown);
                }
                else if (canBeUsedFromFrontRow)
                {
                    return new DirectionAndConfident(DirectionToMove.Down, !enemyInSpaceDown);
                }
                return new DirectionAndConfident(DirectionToMove.None, false);
            }
        }
        else if (targetColumn < playerColumn) // player is to the right
        {
            bool enemyInSpaceToTheRight = CombatArea.instance.GetCombatSpaceAtPosition(new Vector2Int(combatSpace.gridPosition.x + 1, combatSpace.gridPosition.y)).EnemyInSpace();
            if (currentRow == 1) // front row, player to the right
            {
                bool enemyInSpaceToTheUpRight = CombatArea.instance.GetCombatSpaceAtPosition(new Vector2Int(combatSpace.gridPosition.x + 1, combatSpace.gridPosition.y + 1)).EnemyInSpace();
                bool enemyInSpaceUp = CombatArea.instance.GetCombatSpaceAtPosition(new Vector2Int(combatSpace.gridPosition.x, combatSpace.gridPosition.y + 1)).EnemyInSpace();
                if (canBeUsedFromFrontRow && !canBeUsedFromBackRow)
                {
                    return new DirectionAndConfident(DirectionToMove.Right, !enemyInSpaceToTheRight);
                }
                if (canBeUsedFromBackRow && !canBeUsedFromFrontRow)
                {
                    if (canMoveDiagonally && !enemyInSpaceToTheUpRight)
                    {
                        return new DirectionAndConfident(DirectionToMove.UpRight, true);
                    }
                    if (!enemyInSpaceToTheRight)
                    {
                        return new DirectionAndConfident(DirectionToMove.Right, true);
                    }
                    if (!enemyInSpaceUp)
                    {
                        return new DirectionAndConfident(DirectionToMove.Up, true);
                    }
                    if (canMoveDiagonally)
                    {
                        return new DirectionAndConfident(DirectionToMove.UpRight, false);
                    }
                    return new DirectionAndConfident(DirectionToMove.Right, false);
                }
                if (canBeUsedFromFrontRow && canBeUsedFromBackRow)
                {
                    if (enemyInSpaceToTheRight && canMoveDiagonally && !enemyInSpaceToTheUpRight)
                    {
                        return new DirectionAndConfident(DirectionToMove.UpRight, false);
                    }
                    return new DirectionAndConfident(DirectionToMove.Right, false);
                }
                return new DirectionAndConfident(DirectionToMove.None, false);
            }
            else // back row, player to the right
            {
                bool enemyInSpaceToTheDownRight = CombatArea.instance.GetCombatSpaceAtPosition(new Vector2Int(combatSpace.gridPosition.x + 1, combatSpace.gridPosition.y - 1)).EnemyInSpace();
                bool enemyInSpaceDown = CombatArea.instance.GetCombatSpaceAtPosition(new Vector2Int(combatSpace.gridPosition.x, combatSpace.gridPosition.y - 1)).EnemyInSpace();
                if (canBeUsedFromBackRow && !canBeUsedFromFrontRow)
                {
                    return new DirectionAndConfident(DirectionToMove.Right, !enemyInSpaceToTheRight);
                }
                if (canBeUsedFromFrontRow && !canBeUsedFromBackRow)
                {
                    if (canMoveDiagonally && !enemyInSpaceToTheDownRight)
                    {
                        return new DirectionAndConfident(DirectionToMove.DownRight, true);
                    }
                    if (!enemyInSpaceToTheRight)
                    {
                        return new DirectionAndConfident(DirectionToMove.Right, true);
                    }
                    if (!enemyInSpaceDown)
                    {
                        return new DirectionAndConfident(DirectionToMove.Down, true);
                    }
                    if (canMoveDiagonally)
                    {
                        return new DirectionAndConfident(DirectionToMove.DownRight, false);
                    }
                    return new DirectionAndConfident(DirectionToMove.Right, false);
                }
                if (canBeUsedFromBackRow && canBeUsedFromFrontRow)
                {
                    if (enemyInSpaceToTheRight && canMoveDiagonally && !enemyInSpaceToTheDownRight)
                    {
                        return new DirectionAndConfident(DirectionToMove.DownRight, false);
                    }
                    return new DirectionAndConfident(DirectionToMove.Right, false);
                }
                return new DirectionAndConfident(DirectionToMove.None, false);
            }
        }
        else // player is to the left
        {
            bool enemyInSpaceToTheLeft = CombatArea.instance.GetCombatSpaceAtPosition(new Vector2Int(combatSpace.gridPosition.x - 1, combatSpace.gridPosition.y)).EnemyInSpace();
            if (currentRow == 1) // front row, player to the left
            {
                bool enemyInSpaceToTheUpLeft = CombatArea.instance.GetCombatSpaceAtPosition(new Vector2Int(combatSpace.gridPosition.x - 1, combatSpace.gridPosition.y + 1)).EnemyInSpace();
                bool enemyInSpaceUp = CombatArea.instance.GetCombatSpaceAtPosition(new Vector2Int(combatSpace.gridPosition.x, combatSpace.gridPosition.y + 1)).EnemyInSpace();
                if (canBeUsedFromFrontRow && !canBeUsedFromBackRow)
                {
                    return new DirectionAndConfident(DirectionToMove.Left, !enemyInSpaceToTheLeft);
                }
                if (canBeUsedFromBackRow && !canBeUsedFromFrontRow)
                {
                    if (canMoveDiagonally && !enemyInSpaceToTheUpLeft)
                    {
                        return new DirectionAndConfident(DirectionToMove.UpLeft, true);
                    }
                    if (!enemyInSpaceToTheLeft)
                    {
                        return new DirectionAndConfident(DirectionToMove.Left, true);
                    }
                    if (!enemyInSpaceUp)
                    {
                        return new DirectionAndConfident(DirectionToMove.Up, true);
                    }
                    if (canMoveDiagonally)
                    {
                        return new DirectionAndConfident(DirectionToMove.UpLeft, false);
                    }
                    return new DirectionAndConfident(DirectionToMove.Left, false);
                }
                if (canBeUsedFromFrontRow && canBeUsedFromBackRow)
                {
                    if (enemyInSpaceToTheLeft && canMoveDiagonally && !enemyInSpaceToTheUpLeft)
                    {
                        return new DirectionAndConfident(DirectionToMove.UpLeft, false);
                    }
                    return new DirectionAndConfident(DirectionToMove.Left, false);
                }
                return new DirectionAndConfident(DirectionToMove.None, false);
            }
            else // back row, player to the left
            {
                bool enemyInSpaceToTheDownLeft = CombatArea.instance.GetCombatSpaceAtPosition(new Vector2Int(combatSpace.gridPosition.x - 1, combatSpace.gridPosition.y - 1)).EnemyInSpace();
                bool enemyInSpaceDown = CombatArea.instance.GetCombatSpaceAtPosition(new Vector2Int(combatSpace.gridPosition.x, combatSpace.gridPosition.y - 1)).EnemyInSpace();
                if (canBeUsedFromBackRow && !canBeUsedFromFrontRow)
                {
                    return new DirectionAndConfident(DirectionToMove.Left, !enemyInSpaceToTheLeft);
                }
                if (canBeUsedFromFrontRow && !canBeUsedFromBackRow)
                {
                    if (canMoveDiagonally && !enemyInSpaceToTheDownLeft)
                    {
                        return new DirectionAndConfident(DirectionToMove.DownLeft, true);
                    }
                    if (!enemyInSpaceToTheLeft)
                    {
                        return new DirectionAndConfident(DirectionToMove.Left, true);
                    }
                    if (!enemyInSpaceDown)
                    {
                        return new DirectionAndConfident(DirectionToMove.Down, true);
                    }
                    if (canMoveDiagonally)
                    {
                        return new DirectionAndConfident(DirectionToMove.DownLeft, false);
                    }
                    return new DirectionAndConfident(DirectionToMove.Left, false);
                }
                if (canBeUsedFromBackRow && canBeUsedFromFrontRow)
                {
                    if (enemyInSpaceToTheLeft && canMoveDiagonally && !enemyInSpaceToTheDownLeft)
                    {
                        return new DirectionAndConfident(DirectionToMove.DownLeft, false);
                    }
                    return new DirectionAndConfident(DirectionToMove.Left, false);
                }
                return new DirectionAndConfident(DirectionToMove.None, false);
            }
        }
    }
}
public struct AttackAvailabilityData
{ 
    public bool isAvailable;
    public bool playerInRange;
    public List<CombatSpace> targetableSpaces;
    public AttackAvailabilityData(bool isAvailable, bool playerInRange, List<CombatSpace> targetableSpaces)
    {
        this.isAvailable = isAvailable;
        this.playerInRange = playerInRange;
        this.targetableSpaces = targetableSpaces;
    }
}
public enum DirectionToMove
{
    None,
    Up,
    UpRight,
    Right,
    DownRight,
    Down,
    DownLeft,
    Left,
    UpLeft
}
public struct DirectionAndConfident
{
    public DirectionToMove directionToMove;
    public bool confident;
    public DirectionAndConfident(DirectionToMove directionToMove, bool confident)
    { 
        this.directionToMove = directionToMove;
        this.confident = confident;
    }
}
/* 
    Grid looks like this:
        Back Row    (0,2)   (1,2)   (2,2)   (3,2)
        Front Row   (0,1)   (1,1)   (2,1)   (3,1)
        Player Row  (0,0)   (1,0)   (2,0)   (3,0)
    
    On enemy movement, enemy should always move to the space that gets it closer to being able to use their abiliy that has the highest priority and that is available. Their ability list should be sorted by priority.

    When determining best direction to move, enemies prefer to be in the appropriate column before being in the appropriate row.
*/