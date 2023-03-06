using System;
using Maps;
using UnityEngine;

namespace Extensions
{
    public static class DirectionExtensions
    {
        public static Vector2Int ToCoordinates(this Direction self)
        {
            return self switch
            {
                Direction.North => new Vector2Int(0, 1),
                Direction.South => new Vector2Int(0, -1),
                Direction.East => new Vector2Int(1, 0),
                Direction.West => new Vector2Int(-1, 0),
                _ => throw new ArgumentOutOfRangeException(nameof(self), self, null)
            };
        }

        public static Direction ToDirection(this Vector2Int self)
        {
            if (self == new Vector2Int(0, 1)) return Direction.North;
            if (self == new Vector2Int(0, -1)) return Direction.South;
            if (self == new Vector2Int(1, 0)) return Direction.East;
            if (self == new Vector2Int(-1, 0)) return Direction.West;
            throw new ArgumentException(nameof(self), self.ToString(), null);
        }
    }
}