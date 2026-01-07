using UnityEngine;
using System;

public class RandomNumbers : MonoBehaviour
{
    private System.Random rng;
    private int callCount = 0;
    public void ChangeSeed(int seed)
    {
        rng = new System.Random(seed);
        callCount = 0;
    }
    public int Range(int min, int max)
    {
        callCount++;
        return rng.Next(min, max);
    }
    public float Range(float min, float max)
    {
        callCount++;
        return Mathf.Lerp(min, max, (float)rng.NextDouble());
    }
    public void RestoreState(int seed, int savedCallCount)
    {
        rng = new System.Random(seed);
        callCount = 0;
        for (int i = 0; i < savedCallCount; i++)
        {
            int r = Range(0, 2);
        }
    }
    public int GetCurrentCallCount()
    {
        return callCount;
    }
}