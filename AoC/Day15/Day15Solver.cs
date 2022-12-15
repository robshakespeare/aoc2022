namespace AoC.Day15;

public partial class Day15Solver : ISolver
{
    public string DayName => "Beacon Exclusion Zone";

    public long? SolvePart1(PuzzleInput input)
    {
        var sensorZones = ParseSensorZones(input);
        var targetY = IsExample(sensorZones) ? 10 : 2000000;
        var sensorsOrBeacons = new HashSet<Vector2>();
        var minimumsOnTargetY = new List<int>();
        var maximumsOnTargetY = new List<int>();

        foreach (var sensorZone in sensorZones)
        {
            sensorsOrBeacons.Add(sensorZone.SensorPosition);
            sensorsOrBeacons.Add(sensorZone.BeaconPosition);

            var bounds = sensorZone.GetMinMaxXForY(targetY);
            if (bounds != null)
            {
                minimumsOnTargetY.Add(bounds.Value.MinX);
                maximumsOnTargetY.Add(bounds.Value.MaxX);
            }
        }

        var numSensorsOrBeaconsOnLine = sensorsOrBeacons.Count(p => (int) p.Y == targetY);

        return maximumsOnTargetY.Max() - minimumsOnTargetY.Min() + 1 - numSensorsOrBeaconsOnLine;
    }

    public long? SolvePart2(PuzzleInput input)
    {
        var sensorZones = ParseSensorZones(input);
        var boundsSize = IsExample(sensorZones) ? 20 : 4000000;
        var inclusionBoundingBox = new BoundingBox(new Vector2(0, 0), new Vector2(boundsSize, boundsSize));

        foreach (var sensorZone in sensorZones)
        {
            var distressBeacon = sensorZone.TryFindDistressBeacon(inclusionBoundingBox, sensorZones);

            if (distressBeacon != null)
            {
                Console.WriteLine($"Distress beacon located: {distressBeacon}");

                // Its tuning frequency can be found by multiplying its x coordinate by 4000000 and then adding its y coordinate.
                return (long) distressBeacon.Value.X * 4000000 + (long) distressBeacon.Value.Y;
            }
        }

        throw new InvalidOperationException("Unexpected: distress beacon not found!");
    }

    /// <summary>
    /// Note that the bounds (min and max) are inclusive.
    /// </summary>
    public record BoundingBox(Vector2 Min, Vector2 Max)
    {
        public bool Contains(Vector2 position) =>
            position.X >= Min.X && position.X <= Max.X &&
            position.Y >= Min.Y && position.Y <= Max.Y;
    }

    public class SensorZone
    {
        /// <summary>
        /// Sensor Position, i.e. center of the sensor zone.
        /// </summary>
        public Vector2 SensorPosition { get; }

        public Vector2 BeaconPosition { get; }
        public int ZoneSize { get; }
        public int MinSensorY { get; }
        public int MaxSensorY { get; }
        public int MinSensorX { get; }
        public int MaxSensorX { get; }

        public SensorZone(Vector2 sensorPosition, Vector2 beaconPosition)
        {
            SensorPosition = sensorPosition;
            BeaconPosition = beaconPosition;
            ZoneSize = (int) MathUtils.ManhattanDistance(BeaconPosition, SensorPosition);
            MinSensorY = (int) SensorPosition.Y - ZoneSize;
            MaxSensorY = (int) SensorPosition.Y + ZoneSize;
            MinSensorX = (int) SensorPosition.X - ZoneSize;
            MaxSensorX = (int) SensorPosition.X + ZoneSize;
        }

        public (int MinX, int MaxX)? GetMinMaxXForY(int y)
        {
            if (y >= MinSensorY && y <= MaxSensorY)
            {
                var delta = (int) Math.Abs(y - SensorPosition.Y);

                var minX = (int) SensorPosition.X - ZoneSize + delta;
                var maxX = (int) SensorPosition.X + ZoneSize - delta;

                return (minX, maxX);
            }

            return null;
        }

        /// <summary>
        /// The distress beacon is not detected by any sensor, and must be within our main bounds.
        /// Assuming that there is only one point, as in the example, the point must be right next to the edge of a scan zone.
        /// The point will not be inside any other scan zone if the manhattan distance is greater than the size of scan zone.
        /// </summary>
        public Vector2? TryFindDistressBeacon(
            BoundingBox inclusionBoundingBox,
            IReadOnlyList<SensorZone> sensorZones)
        {
            var path = new[]
            {
                new Vector2(SensorPosition.X, MinSensorY - 1), // top
                new Vector2(MaxSensorX + 1, SensorPosition.Y), // right
                new Vector2(SensorPosition.X, MaxSensorY + 1), // bottom
                new Vector2(MinSensorX - 1, SensorPosition.Y) // left
            };

            bool IsDistressBeacon(Vector2 point) => inclusionBoundingBox.Contains(point) && sensorZones.All(zone => !zone.Contains(point));

            var current = path[0];
            if (IsDistressBeacon(current))
            {
                return current;
            }

            foreach (var target in path.Skip(1))
            {
                var dir = Vector2.Clamp(target - current, new(-1, -1), new(1, 1)); // Move diagonally

                while (current != target)
                {
                    current += dir;
                    if (IsDistressBeacon(current))
                    {
                        return current;
                    }
                }
            }

            return null;
        }

        public bool Contains(Vector2 point) => MathUtils.ManhattanDistance(SensorPosition, point) <= ZoneSize;
    }

    static bool IsExample(SensorZone[] sensorZones) => sensorZones.Length == 14;

    static SensorZone[] ParseSensorZones(string input) =>
        ParseLineRegex.Matches(input).Select(match => new SensorZone(
            new Vector2(int.Parse(match.Groups["sX"].Value), int.Parse(match.Groups["sY"].Value)),
            new Vector2(int.Parse(match.Groups["bX"].Value), int.Parse(match.Groups["bY"].Value)))).ToArray();

    private static readonly Regex ParseLineRegex = BuildParseLineRegex();

    [GeneratedRegex(@"Sensor at x=(?<sX>-?\d+), y=(?<sY>-?\d+): closest beacon is at x=(?<bX>-?\d+), y=(?<bY>-?\d+)", RegexOptions.Compiled)]
    private static partial Regex BuildParseLineRegex();
}
