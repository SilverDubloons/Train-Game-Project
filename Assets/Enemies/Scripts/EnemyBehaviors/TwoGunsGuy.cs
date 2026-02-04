using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TwoGunsGuy", menuName = "Combat/Enemy Behaviors/Two Guns Guy")]
public class TwoGunsGuy : EnemyBehavior
{
    public override List<EnemyIntent> GetIntents(EnemyInGame enemyInGame, CombatSpace[,] combatSpaces, CombatSpace playerSpace)
    {
        List<EnemyIntent> intents = new List<EnemyIntent>();
        Enemy baseEnemy = enemyInGame.GetBaseEnemy();
        bool dontMove = false;
        bool canShootToTheRight = false;
        bool canShootToTheLeft = false;
        for (int i = 0; i < baseEnemy.abilities.Length; i++)
        {
            EnemyAbility ability = baseEnemy.abilities[i];
            EnemyAbilityAttack attackAbility = (EnemyAbilityAttack)ability;
            AttackAvailabilityData attackAvailabilityData = attackAbility.GetAttackAvailabilityData(enemyInGame);
            if (attackAvailabilityData.isAvailable)
            {
                if (i == 0)
                {
                    canShootToTheRight = true;
                }
                else
                {
                    canShootToTheLeft = true;
                }
                EnemyIntentAttack intent = new EnemyIntentAttack();
                intent.ability = ability;
                intent.intentName = ability.abilityName;
                intent.icon = ability.icon;
                intent.actionAnimation = ability.actionAnimation;
                intent.tooltipDatas = ability.GetTooltipDataList();
                intent.affectedColumns = attackAbility.affectedColumns;
                intent.damage = attackAbility.damage;
                List<TooltipData> tooltipDatas = new List<TooltipData>();
                tooltipDatas.Add(new TooltipData(intent.intentName, UIElementType.tooltipName));
                tooltipDatas.Add(new TooltipData($"{intent.damage.ToString()} Damage", UIElementType.tooltipDamage));
                tooltipDatas.Add(new TooltipData(r.i.interf.ConvertAffectedColumnsToString(intent.affectedColumns), UIElementType.tooltipSpecial));
                intent.tooltipDatas = tooltipDatas;
                intents.Add(intent);
            }
            if (attackAvailabilityData.playerInRange)
            {
                dontMove = true;
            }
        }
        if (dontMove)
        {
            return intents;
        }
        EMD emd = new EMD(playerSpace, enemyInGame.GetCurrentCombatSpace());
        // Logger.instance.Log($"{enemyInGame.GetBasicInfo()} playerColumn:{emd.playerColumn}, enemyColumn:{emd.enemyColumn}, enemyRow:{emd.enemyRow}, inSameColumnAsPlayer:{emd.inSameColumnAsPlayer}, playerIsToTheLeft:{emd.playerIsToTheLeft}, playerIsToTheRight:{emd.playerIsToTheRight}, enemyOnLeftHalfOfBoard:{emd.enemyOnLeftHalfOfBoard}, enemyOnLeftEdgeOfBoard:{emd.enemyOnLeftEdgeOfBoard}, enemyOnRightEdgeOfBoard:{emd.enemyOnRightEdgeOfBoard}");
        EnemyIntentMove moveIntent = new EnemyIntentMove();
        if (emd.enemyRow == 1) // Front Row
        { 
            if(!emd.spaceUp.enemyInSpace)
            {
                moveIntent.directionToMove = DirectionToMove.Up;
                goto End;
            }
        }
        if (emd.inSameColumnAsPlayer)
        {
            if (canShootToTheRight && canShootToTheLeft)
            {
                if (emd.enemyOnLeftHalfOfBoard)
                {
                    if(!emd.spaceRight.enemyInSpace)
                    {
                        moveIntent.directionToMove = DirectionToMove.Right;
                    }
                    else if (emd.enemyOnLeftEdgeOfBoard)
                    {
                        moveIntent.directionToMove = DirectionToMove.Left;
                    }
                }
                else
                {
                    if(!emd.spaceLeft.enemyInSpace)
                    {
                        moveIntent.directionToMove = DirectionToMove.Left;
                    }
                    else if (!emd.enemyOnRightEdgeOfBoard)
                    {
                        moveIntent.directionToMove = DirectionToMove.Right;
                    }
                }
            }
            else if (canShootToTheRight)
            {
                if (!emd.enemyOnLeftEdgeOfBoard)
                {
                    moveIntent.directionToMove = DirectionToMove.Left;
                }
                else
                {
                    moveIntent.directionToMove = DirectionToMove.Right;
                }
            }
            else if (canShootToTheLeft)
            {
                if (!emd.enemyOnRightEdgeOfBoard)
                {
                    moveIntent.directionToMove = DirectionToMove.Right;
                }
                else
                {
                    moveIntent.directionToMove = DirectionToMove.Left;
                }
            }
        }
        else if (emd.playerIsToTheLeft) // also not attacking player this turn, sice !dontMove
        {
            moveIntent.directionToMove = DirectionToMove.Left;
        }
        else if (emd.playerIsToTheRight)
        {
            moveIntent.directionToMove = DirectionToMove.Right;
        }
        End:
        if (moveIntent.directionToMove != DirectionToMove.None)
        {
            moveIntent.icon = r.i.interf.ConvertDirectionToMoveToSprite(moveIntent.directionToMove);
            moveIntent.intentName = $"Move {r.i.interf.ConvertDirectionToMoveToString(moveIntent.directionToMove)}";
            List<TooltipData> tooltipDatas = new List<TooltipData>();
            tooltipDatas.Add(new TooltipData(moveIntent.intentName, UIElementType.tooltipName));
            moveIntent.tooltipDatas = tooltipDatas;
            intents.Add(moveIntent);
        }
        return intents;
    }
}
