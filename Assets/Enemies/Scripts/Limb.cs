using UnityEngine;
[System.Serializable]
public class Limb
{
    public string limbName;
    public Vector2 size;
    public Vector2 location;
    public LimbTag[] limbTags;
    public Sprite sprite;
}

public enum LimbTag
{
    Arm,
    Leg,
    Head,
    Torso,
    Tail
}