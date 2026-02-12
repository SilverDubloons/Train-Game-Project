using UnityEngine;

[CreateAssetMenu(fileName = "FarBackRandomColumn", menuName = "Combat/Encounter")]
public class Encounter : ScriptableObject
{
    public string encounterName;
    public string encounterTag;
    public string encounterDescription;
    public EncounterEnemy[] enemies;
    public Vector2Int boardSize;
}
[System.Serializable]
public struct EncounterEnemy
{
    public Enemy enemy;
    public Vector2 spawnPosition;
}