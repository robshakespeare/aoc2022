using Microsoft.VisualBasic;

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

        var minList = new List<int>();
        var maxList = new List<int>();

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

            ////=====
            map[sensorReport.SensorPosition] = SensorChar;
            map[sensorReport.BeaconPosition] = BeaconChar;

            var bounds = sensorReport.GetMinMaxXForY(targetY);
            if (bounds != null)
            {
                minList.Add(bounds.Value.MinX);
                maxList.Add(bounds.Value.MaxX);
            }
            //PlotSensorReportCoverage(sensorReport, map, targetY);
        }

        var numSensorsOrBeaconsOnLine = map.Count(p => (int)p.Key.Y == targetY);

        Console.WriteLine(new { numSensorsOrBeaconsOnLine });

        return maxList.Max() - minList.Min() + 1 - numSensorsOrBeaconsOnLine;

        //return map.Count(p => (int) p.Key.Y == targetY && p.Value == NoBeaconChar);
    }

    static void PlotSensorReportCoverage(SensorReport sensorReport, Dictionary<Vector2, char> map, int? targetY)
    {
        var width = sensorReport.DistanceBetweenSensorAndBeacon * 2 + 1;

        var sensorX = (int) sensorReport.SensorPosition.X;
        var sensorY = (int) sensorReport.SensorPosition.Y;
        var relativeY = 0;

        while (width > 0)
        {
            var northY = sensorY - relativeY;
            var southY = sensorY + relativeY;

            if (northY == targetY || targetY == null)
            {
                PlotPath(
                    new Vector2(sensorX - (int)(width / 2f), northY),
                    new Vector2(sensorX + (int)(width / 2f), northY),
                    map);
            }

            if (southY == targetY || targetY == null)
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

    /*
     * Where there are >1 sensors for that y
     * Work out whether there is a gap in X
     * Can easily get MinX and MaxX of each sensor
     * Can easily get MinX and MaxX of the whole line
     *
     *
     */
    public long? SolvePart2(PuzzleInput input)
    {
        var sensorReports = ParseSensorReports(input);
        var target = sensorReports.Length < 30 ? 20 : 4000000;

        var minY = sensorReports.Min(x => x.MinSensorY);
        var maxY = sensorReports.Max(x => x.MaxSensorY);
        Console.WriteLine(new { minY, maxY });

        var minX = sensorReports.Min(x => x.MinSensorX);
        var maxX = sensorReports.Max(x => x.MaxSensorX);
        Console.WriteLine(new { minX, maxX });

        var count = 0;

        for (var i = 0; i <= target; i++)
        {
            //count += sensorReports.Count(x => x.MinSensorY >= y && x.MaxSensorY <= y);
            var sensorsOnThisLine = sensorReports.Count(x => i >= x.MinSensorX && i <= x.MaxSensorX &&
                                                             i >= x.MinSensorY && i <= x.MaxSensorY);

            if (sensorsOnThisLine > 1)
            {
                count++;
            }
        }

        Console.WriteLine(new { count });

        foreach (var sensorReport in sensorReports)
        {
            Console.WriteLine(sensorReport.DistanceBetweenSensorAndBeacon);
        }

        return null;
    }

    //public long? SolvePart2(PuzzleInput input)
    //{
    //    var sensorReports = ParseSensorReports(input);
    //    //var targetY = sensorReports.Length < 30 ? 10 : 2000000;

    //    var map = new Dictionary<Vector2, char>();

    //    foreach (var sensorReport in sensorReports)
    //    {
    //        PlotSensorReportCoverage(sensorReport, map, null);

    //        map[sensorReport.BeaconPosition] = BeaconChar;
    //        map[sensorReport.SensorPosition] = SensorChar;
    //    }

    //    var minBounds = new Vector2(map.Min(i => i.Key.X), map.Min(i => i.Key.Y));
    //    var maxBounds = new Vector2(map.Max(i => i.Key.X), map.Max(i => i.Key.Y));

    //    Console.WriteLine(minBounds);
    //    map.ToStringGrid(p => p.Key, p => p.Value, '.').RenderGridToConsole();
    //    Console.WriteLine(maxBounds);

    //    return null;
    //}

    public record SensorReport(Vector2 SensorPosition, Vector2 BeaconPosition)
    {
        public long DistanceBetweenSensorAndBeacon { get; } = MathUtils.ManhattanDistance(BeaconPosition, SensorPosition);

        public long MinSensorY { get; } = (int) SensorPosition.Y - MathUtils.ManhattanDistance(BeaconPosition, SensorPosition);

        public long MaxSensorY { get; } = (int) SensorPosition.Y + MathUtils.ManhattanDistance(BeaconPosition, SensorPosition);

        public long MinSensorX { get; } = (int) SensorPosition.X - MathUtils.ManhattanDistance(BeaconPosition, SensorPosition);

        public long MaxSensorX { get; } = (int) SensorPosition.X + MathUtils.ManhattanDistance(BeaconPosition, SensorPosition);

        public (int MinX, int MaxX)? GetMinMaxXForY(int y)
        {
            if (y >= MinSensorY && y <= MaxSensorY)
            {
                var delta = Math.Abs(y - (int) SensorPosition.Y);

                //var minX = (int) SensorPosition.X - (int) DistanceBetweenSensorAndBeacon - delta;
                var minX = (int) SensorPosition.X - (int) DistanceBetweenSensorAndBeacon + delta;
                var maxX = (int) SensorPosition.X + (int) DistanceBetweenSensorAndBeacon - delta;

                return (minX, maxX);
            }

            return null;
        }
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
