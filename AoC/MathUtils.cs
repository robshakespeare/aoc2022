namespace AoC;

public static class MathUtils
{
    /// <summary>
    /// Finds the greatest common divisor (GCD) of the two specified numbers.
    /// </summary>
    public static long GreatestCommonDivisor(long a, long b) => (long) BigInteger.GreatestCommonDivisor(a, b);

    /// <summary>
    /// Returns the least common multiple (LCM) of the two specified numbers.
    /// </summary>
    public static long LeastCommonMultiple(long a, long b) => a * b / GreatestCommonDivisor(a, b);

    /// <summary>
    /// Returns the least common multiple (LCM) of the three specified numbers.
    /// </summary>
    public static long LeastCommonMultiple(long a, long b, long c) => LeastCommonMultiple(LeastCommonMultiple(a, b), c);

    /// <summary>
    /// Calculates the angle, in degrees, between the two specified vectors.
    /// </summary>
    public static double AngleBetween(Vector2 vector1, Vector2 vector2)
    {
        var sin = vector1.X * vector2.Y - vector2.X * vector1.Y;
        var cos = vector1.X * vector2.X + vector1.Y * vector2.Y;

        var angleBetween = Math.Atan2(sin, cos) * (180 / Math.PI);
        return angleBetween < 0 ? 360 + angleBetween : angleBetween;
    }

    private const double DegreesToRadiansFactor = Math.PI / 180;

    /// <summary>
    /// Converts the specified degrees in to radians.
    /// </summary>
    public static float DegreesToRadians(this int degrees) => Convert.ToSingle(degrees * DegreesToRadiansFactor);

    /// <summary>
    /// Rotates the specified direction vector, around the zero vector (0,0) center point, by the specified number of degrees.
    /// </summary>
    /// <remarks>
    /// Note: This method expects the direction vector to be centered around the zero vector (0,0).
    /// It could probably be extended, using the Matrix3x2 CreateRotation(float radians, Vector2 centerPoint) method,
    /// to cater for rotation around a non-zero center point.
    /// </remarks>
    public static Vector2 RotateDirection(Vector2 direction, int degrees)
    {
        var radians = degrees.DegreesToRadians();
        var rotationMatrix = Matrix3x2.CreateRotation(radians);
        return Vector2.Transform(direction, rotationMatrix);
    }

    /// <summary>
    /// Rounds the floating-point value to the nearest integer value, rounding midpoint values away from zero.
    /// </summary>
    public static long Round(this float f) => (long) MathF.Round(f, MidpointRounding.AwayFromZero);

    /// <summary>
    /// Rounds the specified Vector2's X and Y components to the nearest integer value, rounding midpoint values away from zero.
    /// </summary>
    public static Vector2 Round(this Vector2 v) => new(v.X.Round(), v.Y.Round());

    /// <summary>
    /// Returns the Manhattan Distance between two cartesian coordinates.
    /// The cartesian coordinates are expected to be on a integer grid,
    /// and will be rounded to the nearest integer before calculating the the Manhattan Distance calculation.
    /// </summary>
    public static long ManhattanDistance(Vector2 a, Vector2 b) => Math.Abs(a.X.Round() - b.X.Round()) + Math.Abs(a.Y.Round() - b.Y.Round());

    /// <summary>
    /// Returns the Manhattan Distance between the two specified 3D coordinates.
    /// The coordinates are expected to be in an integer world space,
    /// and will be rounded to the nearest integer before calculating the the Manhattan Distance calculation.
    /// </summary>
    public static long ManhattanDistance(Vector3 a, Vector3 b) =>
        Math.Abs(a.X.Round() - b.X.Round()) + Math.Abs(a.Y.Round() - b.Y.Round()) + Math.Abs(a.Z.Round() - b.Z.Round());

    /// <summary>
    /// Returns the Manhattan Distance between the specified cartesian coordinates and the zero vector (0,0).
    /// The cartesian coordinates are expected to be on a integer grid,
    /// and will be rounded to the nearest integer before calculating the the Manhattan Distance calculation.
    /// </summary>
    public static long ManhattanDistanceFromZero(this Vector2 vector) => ManhattanDistance(vector, Vector2.Zero);

    /// <summary>
    /// Calculates and returns the median of the specified values.
    /// The median being the middle value of the given list of data, when arranged in an order.
    /// If the specified set of values is empty, an `InvalidOperationException` is thrown.
    /// </summary>
    public static long Median(this IEnumerable<long> values)
    {
        var sorted = values.OrderBy(x => x).ToArray();
        if (sorted.Length == 0)
        {
            throw new InvalidOperationException("The median of an empty data set is undefined.");
        }

        var (midPoint, remainder) = Math.DivRem(sorted.Length, 2);
        return remainder == 0
            ? (sorted[midPoint - 1] + sorted[midPoint]) / 2
            : sorted[midPoint];
    }
}
