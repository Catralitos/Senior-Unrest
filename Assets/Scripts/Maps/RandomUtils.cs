using System;
using Random = UnityEngine.Random;

namespace Maps
{
    public static class RandomUtils
    {
        public static T GetRandomEnumValue<T>() where T : Enum
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            return (T)values.GetValue(Random.Range(0, values.Length));
        }
        
    }
}