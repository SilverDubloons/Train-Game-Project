using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
/*
 * Remeber to set the sprite size to correct size before baking hitboxes
 * Also remember that there is no undo for this operation, so make sure to update enemyName correctly
 */

public class EnemySpriteBaker : MonoBehaviour
{
    public Enemy[] enemies;
    public string enemyTag;

    [ContextMenu("Bake Hitbox Layout")]
    private void BakeHitboxLayout()
    {
        Enemy currentEnemy = null;
        foreach (var enemy in enemies)
        {
            if (enemy.enemyTag == enemyTag)
            {
                currentEnemy = enemy;
                break;
            }
        }
        if (currentEnemy == null)
        {
            Debug.LogWarning("Enemy not found: " + enemyTag + ". Create enemy scriptable object with this tag first and add it to enemies array");
            return;
        }
        LimbInGame[] limbHitboxes = GetComponentsInChildren<LimbInGame>();
        List<LimbInGame> limbHitboxesList = new List<LimbInGame>();
        for(int i  = 0; i < limbHitboxes.Length; i++)
        {
            if (limbHitboxes[i].image.sprite != null && limbHitboxes[i].gameObject.activeSelf)
            {
                limbHitboxesList.Add(limbHitboxes[i]);
            }
        }
        currentEnemy.limbs = new Limb[limbHitboxesList.Count];
        Vector2 bottomLeft = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 topRight = new Vector2(float.MinValue, float.MinValue);
        for (int i = 0; i < limbHitboxesList.Count; i++)
        {
            if (limbHitboxes[i].rt.anchoredPosition.x - limbHitboxes[i].rt.sizeDelta.x / 2 < bottomLeft.x)
            { 
                bottomLeft.x = limbHitboxes[i].rt.anchoredPosition.x - limbHitboxes[i].rt.sizeDelta.x / 2;
            }
            if (limbHitboxes[i].rt.anchoredPosition.y - limbHitboxes[i].rt.sizeDelta.y / 2 < bottomLeft.y)
            {
                bottomLeft.y = limbHitboxes[i].rt.anchoredPosition.y - limbHitboxes[i].rt.sizeDelta.y / 2;
            }
            if( limbHitboxes[i].rt.anchoredPosition.x + limbHitboxes[i].rt.sizeDelta.x / 2 > topRight.x)
            {
                topRight.x = limbHitboxes[i].rt.anchoredPosition.x + limbHitboxes[i].rt.sizeDelta.x / 2;
            }
            if( limbHitboxes[i].rt.anchoredPosition.y + limbHitboxes[i].rt.sizeDelta.y / 2 > topRight.y)
            {
                topRight.y = limbHitboxes[i].rt.anchoredPosition.y + limbHitboxes[i].rt.sizeDelta.y / 2;
            }
            currentEnemy.limbs[i] = new Limb
            {
                limbName = limbHitboxes[i].limbName,
                size = limbHitboxes[i].rt.sizeDelta,
                location = limbHitboxes[i].rt.anchoredPosition,
                limbTags = limbHitboxes[i].limbTags,
                sprite = limbHitboxes[i].image.sprite,
                maxHealth = limbHitboxes[i].maxHealth,
                startingHealth = limbHitboxes[i].currentHealth
            };
        }
        Vector2 spriteCenter = (topRight + bottomLeft) / 2;
        currentEnemy.spriteCenter = spriteCenter;
        currentEnemy.totalSize = new Vector2(topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
    }
    void Start()
    {
        // Destroy(this.gameObject);
    }
}
