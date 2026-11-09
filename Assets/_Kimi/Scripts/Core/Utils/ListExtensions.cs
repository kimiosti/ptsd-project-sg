using System;
using System.Collections.Generic;

public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        var randomGenerator = new Random();

        for (int i=0; i < list.Count-2; i++)
        {
            var randomSeed = randomGenerator.Next(i+1, list.Count);
            (list[randomSeed], list[i]) = (list[i], list[randomSeed]);
        }
    }
}