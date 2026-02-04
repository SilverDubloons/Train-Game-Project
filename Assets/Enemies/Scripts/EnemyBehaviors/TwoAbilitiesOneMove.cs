using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "TwoAbilitiesOneMove", menuName = "Combat/Enemy Behaviors/Two Abilities One Move")]
public class TwoAbilitiesOneMove : EnemyBehavior
{
    // public abstract List<EnemyAction> GetActions(EnemyInGame enemyInGame, CombatSpace[,] combatSpaces, CombatSpace playerSpace);
    public override List<EnemyIntent> GetIntents(EnemyInGame enemyInGame, CombatSpace[,] combatSpaces, CombatSpace playerSpace)
    {
        List<EnemyIntent> intents = new List<EnemyIntent>();
        Enemy baseEnemy = enemyInGame.GetBaseEnemy();
        EnemyAbility[] bestAbilities = new EnemyAbility[2];
        AttackAvailabilityData[] bestAttackAvailabilityDatas = new AttackAvailabilityData[2];
        for (int i = 0; i < baseEnemy.abilities.Length; i++)
        {
            EnemyAbility ability = baseEnemy.abilities[i];
            if (ability == null)
            {
                continue;
            }
            switch (ability.GetAbilityType())
            {
                case AbilityType.Attack:
                    EnemyAbilityAttack attackAbility = (EnemyAbilityAttack)ability;
                    AttackAvailabilityData attackAvailabilityData = attackAbility.GetAttackAvailabilityData(enemyInGame);
                    if (attackAvailabilityData.isAvailable)
                    {
                        if ((bestAbilities[0] == null && bestAbilities[1] == null) || (bestAbilities[0] != null  && bestAbilities[1] != null && attackAvailabilityData.playerInRange && !bestAttackAvailabilityDatas[0].playerInRange))
                        {
                            bestAttackAvailabilityDatas[0] = attackAvailabilityData;
                            bestAbilities[0] = ability;
                        }
                        else if (bestAbilities[1] == null ||  (attackAvailabilityData.playerInRange! && bestAttackAvailabilityDatas[1].playerInRange))
                        {
                            bestAttackAvailabilityDatas[1] = attackAvailabilityData;
                            bestAbilities[1] = ability;
                        }
                    }
                break;
            }
        }
        for (int i = 0; i < bestAbilities.Length; i++)
        {
            if (bestAbilities[i] != null)
            {
                switch (bestAbilities[i].GetAbilityType())
                {
                    case AbilityType.Attack:
                        EnemyIntentAttack intent = new EnemyIntentAttack();
                        intent.ability = bestAbilities[i];
                        intent.intentName = bestAbilities[i].abilityName;
                        intent.icon = bestAbilities[i].icon;
                        intent.actionAnimation = bestAbilities[i].actionAnimation;
                        intent.tooltipDatas = bestAbilities[i].GetTooltipDataList();
                        // intent.affectedSpaces = bestAttackAvailabilityData.targetableSpaces;
                        EnemyAbilityAttack bestAbilityAttack = (EnemyAbilityAttack)bestAbilities[i];
                        intent.affectedColumns = bestAbilityAttack.affectedColumns;
                        intent.damage = bestAbilityAttack.damage;
                        List<TooltipData> tooltipDatas = new List<TooltipData>();
                        tooltipDatas.Add(new TooltipData(intent.intentName, UIElementType.tooltipName));
                        tooltipDatas.Add(new TooltipData($"{intent.damage.ToString()} Damage", UIElementType.tooltipDamage));
                        if (intent.affectedColumns.Length == 1)
                        {
                            if (intent.affectedColumns[0] == 0)
                            {
                                tooltipDatas.Add(new TooltipData("Same Column", UIElementType.tooltipSpecial));
                            }
                            else if (intent.affectedColumns[0] == -1)
                            {
                                tooltipDatas.Add(new TooltipData("Column to the Left", UIElementType.tooltipSpecial));
                            }
                            else if (intent.affectedColumns[0] == 1)
                            {
                                tooltipDatas.Add(new TooltipData("Column to the Right", UIElementType.tooltipSpecial));
                            }
                        }
                        intent.tooltipDatas = tooltipDatas;
                        intents.Add(intent);
                        break;
                }
            }
        }
        DirectionAndConfident directionAndConfident = new DirectionAndConfident(DirectionToMove.None, false);
        for (int i = 0; i < enemyInGame.GetBaseEnemy().abilities.Length; i++)
        {
            EnemyAbility ability = baseEnemy.abilities[i];
            AbilityType abiltyType = enemyInGame.GetBaseEnemy().abilities[i].GetAbilityType();
            switch (abiltyType)
            {
                case (AbilityType.Attack):
                    EnemyAbilityAttack attackAbility = (EnemyAbilityAttack)ability;
                    directionAndConfident = attackAbility.GetBestDirectionToMove(enemyInGame, enemyInGame.GetCurrentCombatSpace());
                    break;
            }
            if (directionAndConfident.confident)
            {
                break;
            }
        }
        if (directionAndConfident.directionToMove != DirectionToMove.None)
        {
            EnemyIntentMove moveIntent = new EnemyIntentMove();
            moveIntent.directionToMove = directionAndConfident.directionToMove;
            moveIntent.icon = r.i.interf.ConvertDirectionToMoveToSprite(moveIntent.directionToMove);
            moveIntent.intentName = $"Move {r.i.interf.ConvertDirectionToMoveToString(directionAndConfident.directionToMove)}";
            List<TooltipData> tooltipDatas = new List<TooltipData>();
            tooltipDatas.Add(new TooltipData(moveIntent.intentName, UIElementType.tooltipName));
            moveIntent.tooltipDatas = tooltipDatas;
            intents.Add(moveIntent);
        }
        return intents;
    }
}
