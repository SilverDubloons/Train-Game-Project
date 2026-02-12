using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
public class EnemyIntentAttack : EnemyIntent
{
    public EnemyAbilityAttack abilityAttack;
    public ActionAnimation actionAnimation;
    public override async Task ExecuteIntentAsync(EnemyInGame enemyInGame)
    {
        executingIntent = true;
        List<ActionAnimator> actionAnimators = new List<ActionAnimator>();
        if (actionAnimation != null)
        {
            ActionAnimator actionAnimator = ActionAnimators.instance.StartActionAnimation(actionAnimation, r.i.interf.GetCanvasPositionOfRectTransform(Player.instance.rt, GameManager.instance.gameplayCanvas));
            actionAnimators.Add(actionAnimator);
        }
        await WaitForAnimationsAsync(actionAnimators);
        Player.instance.TakeDamage(GetDamage(enemyInGame));
        executingIntent = false;
    }
    private async Task WaitForAnimationsAsync(List<ActionAnimator> actionAnimators)
    {
        while (ActionAnimatorInListStillRunning(actionAnimators))
        {
            await Task.Yield();
        }
    }
    /*public IEnumerator ExecuteAttack(EnemyInGame enemyInGame)
    {
        executingIntent = true;
        List<ActionAnimator> actionAnimators = new List<ActionAnimator>();
        if (actionAnimation != null)
        {
            ActionAnimator actionAnimator = ActionAnimators.instance.StartActionAnimation(actionAnimation, r.i.interf.GetCanvasPositionOfRectTransform(Player.instance.rt, GameManager.instance.gameplayCanvas));
            actionAnimators.Add(actionAnimator);
        }
        while (ActionAnimatorInListStillRunning(actionAnimators))
        {
            yield return null;
        }
        Player.instance.TakeDamage(GetDamage(enemyInGame));
        executingIntent = false;
    }*/
    private bool ActionAnimatorInListStillRunning(List<ActionAnimator> actionAnimators)
    {
        for (int i = 0; i < actionAnimators.Count; i++)
        {
            if (actionAnimators[i].animating)
            {
                return true;
            }
        }
        return false;
    }
    public override IntentType GetIntentType()
    {
        return IntentType.Attack;
    }
    public int GetDamage(EnemyInGame enemyInGame)
    {
        // adding as a function in case in future there needs to be additional considerations, like status effects that can affect damage
        int totalDamage = abilityAttack.damage;
        totalDamage += enemyInGame.statusEffects.GetStatusMagnitude(Status.DamageBonus);
        return totalDamage;
    }
    public EnemyIntentAttack(EnemyAbilityAttack enemyAbilityAttack)
    {
        abilityAttack = enemyAbilityAttack;
        intentName = enemyAbilityAttack.abilityName;
        icon = enemyAbilityAttack.icon;
        actionAnimation = enemyAbilityAttack.actionAnimation;
        tooltipDatas = enemyAbilityAttack.GetTooltipDataList();
        enemyAbility = enemyAbilityAttack;
    }
}