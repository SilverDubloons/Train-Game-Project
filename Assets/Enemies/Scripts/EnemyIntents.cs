using UnityEngine;

public class EnemyIntents : MonoBehaviour
{
    [SerializeField] private GameObject enemyIntentUIPrefab;
    [SerializeField] private RectTransform spareEnemyIntentUIParent;
    public static EnemyIntents instance;
    public void SetupInstance()
    {
        instance = this;
    }
    public EnemyIntentUI GetEnemyIntentUI()
    {
        if (spareEnemyIntentUIParent.childCount > 0)
        {
            return spareEnemyIntentUIParent.GetChild(spareEnemyIntentUIParent.childCount - 1).GetComponent<EnemyIntentUI>();
        }
        return Instantiate(enemyIntentUIPrefab, spareEnemyIntentUIParent).GetComponent<EnemyIntentUI>();
    }
    public void RetireEnemyIntentUI(EnemyIntentUI enemyIntentUI)
    {
        enemyIntentUI.rt.SetParent(spareEnemyIntentUIParent);
        enemyIntentUI.SetVisibility(false);
    }
}
