using System;

// Serializable so it will show up in the inspector.
[Serializable]
public class IntRange
{
    public int min;       // The minimum value in this range.
    public int max;       // The maximum value in this range.


    // Constructor to set the values.
    public IntRange(int min, int max)
    {
        this.min = min;
        this.max = max;
    }


    // Get a random value from the range.
    public int Random
    {
        get { return UnityEngine.Random.Range(min, max); }
    }
}