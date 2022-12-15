namespace AoC.Day15;

public class Day15Solver : ISolver
{
    public string DayName => "Beacon Exclusion Zone";

    const char SensorChar = 'S';
    const char BeaconChar = 'B';
    const char NoBeaconChar = '#';

    public long? SolvePart1(PuzzleInput input)
    {
        var sensorReports = ParseSensorReports(input);
        var targetY = sensorReports.Length < 30 ? 10 : 2000000;

        var map = new Dictionary<Vector2, char>();

        foreach (var sensorReport in sensorReports)
        {
            //if (map.ContainsKey(sensorReport.SensorPosition))
            //{
            //    throw new InvalidOperationException($"Sensor at {sensorReport.SensorPosition} would overlap {map[sensorReport.SensorPosition]} which is already there");
            //}

            //if (map.ContainsKey(sensorReport.BeaconPosition))
            //{
            //    throw new InvalidOperationException($"Beacon at {sensorReport.BeaconPosition} would overlap {map[sensorReport.BeaconPosition]} which is already there");
            //}

            //map.Add(sensorReport.SensorPosition, SensorChar);
            //map.Add(sensorReport.BeaconPosition, BeaconChar);

            map[sensorReport.SensorPosition] = SensorChar;
            map[sensorReport.BeaconPosition] = BeaconChar;

            PlotSensorReportCoverage(sensorReport, map, targetY);
        }

        return map.Count(p => (int) p.Key.Y == targetY && p.Value == NoBeaconChar);
    }

    static void PlotSensorReportCoverage(SensorReport sensorReport, Dictionary<Vector2, char> map, int targetY)
    {
        var width = sensorReport.DistanceBetweenSensorAndBeacon * 2 + 1;

        var sensorX = (int) sensorReport.SensorPosition.X;
        var sensorY = (int) sensorReport.SensorPosition.Y;
        var relativeY = 0;

        while (width > 0)
        {
            var northY = sensorY - relativeY;
            var southY = sensorY + relativeY;

            if (northY == targetY)
            {
                PlotPath(
                    new Vector2(sensorX - (int)(width / 2f), northY),
                    new Vector2(sensorX + (int)(width / 2f), northY),
                    map);
            }

            if (southY == targetY)
            {
                PlotPath(
                    new Vector2(sensorX - (int)(width / 2f), southY),
                    new Vector2(sensorX + (int)(width / 2f), southY),
                    map);
            }

            // new Vector2(SandSource.X - (int) (floorWidth / 2f), floorY),
            // new Vector2(SandSource.X + (int)(floorWidth / 2f), floorY)

            width -= 2;
            relativeY++;
        }
    }

    static void PlotPath(Vector2 left, Vector2 right, Dictionary<Vector2, char> map)
    {
        if (right.X < left.X)
        {
            throw new InvalidOperationException("Invalid state");
        }

        var current = left;
        var dir = new Vector2(1, 0); // : Vector2.Normalize(right - left);
        var target = right + dir;
        while (current != target)
        {
            map.TryAdd(current, NoBeaconChar);
            current += dir;
        }
    }

    public long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public record SensorReport(Vector2 SensorPosition, Vector2 BeaconPosition)
    {
        public long DistanceBetweenSensorAndBeacon { get; } = MathUtils.ManhattanDistance(BeaconPosition, SensorPosition);
    }

    static SensorReport[] ParseSensorReports(string input) =>
        ParseLineRegex.Matches(input).Select(match => new SensorReport(
            new Vector2(int.Parse(match.Groups["sX"].Value), int.Parse(match.Groups["sY"].Value)),
            new Vector2(int.Parse(match.Groups["bX"].Value), int.Parse(match.Groups["bY"].Value)))).ToArray();

    // Sensor at x=(?<sX>-?\d+), y=(?<sY>-?\d+): closest beacon is at x=(?<bX>-?\d+), y=(?<bY>-?\d+)
    private static readonly Regex ParseLineRegex = new(
        @"Sensor at x=(?<sX>-?\d+), y=(?<sY>-?\d+): closest beacon is at x=(?<bX>-?\d+), y=(?<bY>-?\d+)",
        RegexOptions.Compiled);
}
