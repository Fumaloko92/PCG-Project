using System;

public static class StaticRandom
{
    static readonly System.Random random = new System.Random();

    public static int Rand(int f, int t)
    {
        return random.Next(f, t);
    }

    public static double Sample()
    {
        return random.NextDouble();
    }
}
