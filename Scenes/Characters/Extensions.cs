
using Godot;

public static class Extensions
    /// <summary>
    /// Returns raw input from any vector 2
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
{
    public static Vector2 GetRaw(this Vector2 vector)
    {
        vector.X = vector.X switch
        {
            > 0 => 1,
            < 0 => -1,
            _ => 0
        };
        vector.Y = vector.Y switch
        {
            > 0 => 1,
            < 0 => -1,
            _ => 0
        };
        return vector;
    }
}
