using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
public abstract class EnemyIntent
{
    public string intentName;
    public Sprite icon;
    public List<TooltipData> tooltipDatas;
    public EnemyIntentUI enemyIntentUI;
    public EnemyAbility enemyAbility;
    public abstract IntentType GetIntentType();
    public abstract Task ExecuteIntentAsync(EnemyInGame enemyInGame);
    public bool executingIntent = false;
}
public enum IntentType
{
    Attack,
    SelfBuff,
    Shield,
    SelfHealLimb,
    SelfHeal,
    HealOther,
    HealOtherLimb
}
