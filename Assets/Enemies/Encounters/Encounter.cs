using UnityEngine;

[CreateAssetMenu(fileName = "FarBackRandomColumn", menuName = "Combat/Encounter")]
public class Encounter : ScriptableObject
{
    public string encounterName;
    public string encounterTag;
    public string encounterDescription;
    public Enemy[] enemies;
    public Vector2Int boardSize;
}
