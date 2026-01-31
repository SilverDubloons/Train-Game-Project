using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "OneAbilityOneMove", menuName = "Combat/Enemy Behaviors/One Ability One Move")]
public class OneAbilityOneMove : EnemyBehavior
{
    // public abstract List<EnemyAction> GetActions(EnemyInGame enemyInGame, CombatSpace[,] combatSpaces, CombatSpace playerSpace);
    public override List<EnemyIntent> GetIntents(EnemyInGame enemyInGame, CombatSpace[,] combatSpaces, CombatSpace playerSpace)
    {
        List<EnemyIntent> intents = new List<EnemyIntent>();
        Enemy baseEnemy = enemyInGame.GetBaseEnemy();
        EnemyAbility bestAbility = null;
        bool dontMove = false;
        AttackAvailabilityData bestAttackAvailabilityData = new AttackAvailabilityData();
        for (int i = 0; i < baseEnemy.abilities.Length; i++)
        {
            EnemyAbility ability = baseEnemy.abilities[i];
            if (ability == null)
            {
                continue;
            }
            switch(ability.GetAbilityType())
            {
                case AbilityType.Attack:
                    EnemyAbilityAttack attackAbility = (EnemyAbilityAttack)ability;
                    bestAttackAvailabilityData = attackAbility.GetAttackAvailabilityData(enemyInGame);
                    if(bestAttackAvailabilityData.isAvailable)
                    {
                        if (bestAbility == null || bestAttackAvailabilityData.playerInRange)
                        {
                            bestAbility = ability;
                        }
                        if (bestAttackAvailabilityData.playerInRange)
                        {
                            // dontMove = true;
                            goto End;
                        }
                    }
                    break;
            }
        }
        End:
        
        if(bestAbility != null)
        {
            switch(bestAbility.GetAbilityType())
            {
                case AbilityType.Attack:
                    EnemyIntentAttack intent = new EnemyIntentAttack();
                    intent.ability = bestAbility;
                    intent.intentName = bestAbility.abilityName;
                    intent.icon = bestAbility.icon;
                    intent.actionAnimation = bestAbility.actionAnimation;
                    intent.tooltipDatas = bestAbility.GetTooltipDataList();
                    // intent.affectedSpaces = bestAttackAvailabilityData.targetableSpaces;
                    EnemyAbilityAttack bestAbilityAttack = (EnemyAbilityAttack)bestAbility;
                    intent.affectedColumns = bestAbilityAttack.affectedColumns;
                    intent.damage = bestAbilityAttack.damage;
                    List<TooltipData> tooltipDatas = new List<TooltipData>();
                    tooltipDatas.Add(new TooltipData(intent.intentName, UIElementType.tooltipName));
                    tooltipDatas.Add(new TooltipData($"{intent.damage.ToString()} Damage", UIElementType.tooltipDamage));
                    if (intent.affectedColumns.Length == 1 && intent.affectedColumns[0] == 0)
                    {
                        tooltipDatas.Add(new TooltipData("Same Column", UIElementType.tooltipSpecial));
                    }
                    intent.tooltipDatas = tooltipDatas;
                    intents.Add(intent);
                    break;
            }
        }
        if(!dontMove)
        {
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
        }
        return intents;
    }
}
