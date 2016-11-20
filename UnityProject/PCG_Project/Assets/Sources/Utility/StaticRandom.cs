using System;

public static class StaticRandom
{
    static readonly System.Random random = new System.Random();

    public static int Rand(int f, int t)
    {
        return (f+random.Next(0,10000000))%t;
    }
}
