using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "ActionAnimation", menuName = "Scriptable Objects/ActionAnimation")]
public class ActionAnimation : ScriptableObject
{
    public string actionAnimationTag;
    public Sprite[] sprites;
    public float framesPerSecond = 30;
    public AudioClip audioClip;
    public float audioClipVolumeFactor = 1f;
}
