using UnityEngine;

public class RandomStrategy
{
    static System.Random random = new System.Random();

    public virtual int NextInt(int start, int end)
    {
        return random.Next(start, end);
    }
}

public class RandomStrategyUnity : RandomStrategy
{
    public override int NextInt(int start, int end)
    {
        return Random.Range(start, end);
    }
}

public static class MyRandom
{
    static RandomStrategy random = new RandomStrategy();

    public static int Next(int start, int end)
    {
        return random.NextInt(start, end);
    }

    public static void UseUnityRandom()
    {
        random = new RandomStrategyUnity();
    }
}
