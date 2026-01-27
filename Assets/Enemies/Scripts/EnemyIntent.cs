using UnityEngine;
using System.Collections.Generic;
public abstract class EnemyIntent
{
    public string intentName;
    public Sprite icon;
    public List<TooltipData> tooltipDatas;
    public abstract IntentType GetIntentType();
}
public enum IntentType
{ 
    Attack,
    Move
}
