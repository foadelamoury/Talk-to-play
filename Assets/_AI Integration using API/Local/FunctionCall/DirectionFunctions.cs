using UnityEngine;
// This class contains static methods that return different directions as Vector2
public class DirectionFunctions
{
    // Each method represents a movement direction using Unity's built-in Vector2 values
    public static Vector2 MoveUp()
    {
        return Vector2.up;    // Returns (0, 1)
    }

    public static Vector2 MoveDown()
    {
        return Vector2.down;  // Returns (0, -1)
    }

    public static Vector2 MoveLeft()
    {
        return Vector2.left;  // Returns (-1, 0)
    }

    public static Vector2 MoveRight()
    {
        return Vector2.right; // Returns (1, 0)
    }

    public static Vector2 NoDirectionsMentioned()
    {
        return Vector2.zero;  // Returns (0, 0) when no direction is specified
    }
}
