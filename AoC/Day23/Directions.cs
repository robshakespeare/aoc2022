namespace AoC.Day23;

// ReSharper disable InconsistentNaming
public static class Directions
{
    public static Vector2 N => GridUtils.North;

    public static Vector2 S => GridUtils.South;

    public static Vector2 W => GridUtils.West;

    public static Vector2 E => GridUtils.East;

    public static Vector2 NE { get; } = N + E;

    public static Vector2 NW { get; } = N + W;

    public static Vector2 SE { get; } = S + E;

    public static Vector2 SW { get; } = S + W;
}
