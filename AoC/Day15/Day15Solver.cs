namespace AoC.Day15;

public partial class Day15Solver : ISolver
{
    public string DayName => "Beacon Exclusion Zone";

    //const char SensorChar = 'S';
    //const char BeaconChar = 'B';
    //const char NoBeaconChar = '#';

    public long? SolvePart1(PuzzleInput input)
    {
        var sensorZones = ParseSensorZones(input);
        var targetY = IsExample(sensorZones) ? 10 : 2000000;

        //var map = new Dictionary<Point, char>();
        var sensorsOrBeacons = new HashSet<Vector2>();

        var minimumsOnTargetY = new List<int>();
        var maximumsOnTargetY = new List<int>();

        foreach (var sensorZone in sensorZones)
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
            //map[sensorReport.SensorPosition] = SensorChar;
            //map[sensorReport.BeaconPosition] = BeaconChar;

            sensorsOrBeacons.Add(sensorZone.SensorPosition);
            sensorsOrBeacons.Add(sensorZone.BeaconPosition);

            var bounds = sensorZone.GetMinMaxXForY(targetY);
            if (bounds != null)
            {
                minimumsOnTargetY.Add(bounds.Value.MinX);
                maximumsOnTargetY.Add(bounds.Value.MaxX);
            }
            //PlotSensorReportCoverage(sensorReport, map, targetY);
        }

        var numSensorsOrBeaconsOnLine = sensorsOrBeacons.Count(p => (int) p.Y == targetY);

        return maximumsOnTargetY.Max() - minimumsOnTargetY.Min() + 1 - numSensorsOrBeaconsOnLine;

        //return map.Count(p => (int) p.Key.Y == targetY && p.Value == NoBeaconChar);
    }

    /*
     * Where there are >1 sensors for that y
     * Work out whether there is a gap in X
     * Can easily get MinX and MaxX of each sensor
     * Can easily get MinX and MaxX of the whole line
     *
     *
     * Latest thinking:
     * Sensor builds a diamond
     * Get all of the intersection points, and build diamonds of those too
     *
     * Latest thinking:
     * Every sensor gives us 4 outer points
     * Get all of the intersection points, and build diamonds of those too
     */
    public long? SolvePart2(PuzzleInput input)
    {
        var sensorZones = ParseSensorZones(input);
        var boundsSize = IsExample(sensorZones) ? 20 : 4000000;
        var inclusionBoundingBox = new BoundingBox(new Vector2(0, 0), new Vector2(boundsSize, boundsSize));

        //var inclusionBounds = new[] {inclusionBoundingBox}.ToList();

        //Console.WriteLine(string.Join(Environment.NewLine, sensorReports.Select(x => x.DistanceBetweenSensorAndBeacon)));

        //if (sensorReports.Length != 14)
        //{
        //    throw new InvalidOperationException("Early exit of actual puzzle input");
        //}

        //var count = 0;
        //foreach (var scan in sensorReports)
        //{
        //    var newInclusionBounds = new List<BoundingBox>();

        //    foreach (var inclusionBound in inclusionBounds)
        //    {
        //        newInclusionBounds.AddRange(inclusionBound.Exclude(scan.GetExclusionBox()));
        //    }

        //    inclusionBounds = newInclusionBounds;


        //    ////var map = new Dictionary<Point, char>();

        //    ////foreach (var (b, i) in inclusionBounds.Select((b, i) => (b, i)))
        //    ////{
        //    ////    foreach (var p in b.GetPointsWithinBounds())
        //    ////    {
        //    ////        map.Add(p, i.ToString()[0]);
        //    ////    }
        //    ////}

        //    //////foreach (var p in sensorReports[1].GetExclusionBox().GetPointsWithinBounds())
        //    //////{
        //    //////    map[p] = 'B';
        //    //////}

        //    ////map[new Point(14, 11)] = 'X';
        //    ////map[new Point(0, 0)] = 'T';
        //    ////map[new Point(20, 20)] = 'B';

        //    ////map.ToStringGrid(x => x.Key.AsVector(), x => x.Value, ' ').RenderGridToConsole();


        //    count++;
        //    //if (count == 2)
        //    //{
        //    //    break;
        //    //}

        //    //foreach (var exclusionBound in scan.GetExclusionBounds())
        //    //{
        //    //    inclusionBounds =
        //    //}


        //}

        //static int GetAreaInc(Vector2 lowerBounds, Vector2 upperBounds)
        //{
        //    var size = upperBounds - lowerBounds + Vector2.One;
        //    return Math.Abs((int)size.X * (int)size.Y);
        //}

        // The point must be within our main bounds
        // Assuming that there is only one point, the point must be right next to the edge of a scan zone
        // The point will not be inside any other scan zone if the manhattan distance is greater than the size of scan zone
        foreach (var sensorZone in sensorZones)
        {
            var distressBeacon = sensorZone.TryFindDistressBeacon(inclusionBoundingBox, sensorZones);

            if (distressBeacon != null)
            {
                //throw new InvalidOperationException($"Point found: {distressBeacon}");

                Console.WriteLine($"Distress beacon located: {distressBeacon}");

                // Its tuning frequency can be found by multiplying its x coordinate by 4000000 and then adding its y coordinate.
                return (long) distressBeacon.Value.X * 4000000 + (long) distressBeacon.Value.Y;
            }
        }

        throw new InvalidOperationException("Unexpected: distress beacon not found!");

        //return inclusionBounds.Count; //.Sum(x => GetAreaInc(x.Min.AsVector(), x.Max.AsVector()));

        //// One of the points must be not inside any of the scan zones
        //foreach (var inclusionBound in inclusionBounds)
        //{
        //    foreach (var candidatePoint in inclusionBound.GetPointsWithinBounds())
        //    {
        //        if (sensorReports.All(s => !s.Contains(candidatePoint)))
        //        {
        //            //throw new InvalidOperationException($"Point found: {candidatePoint}");

        //            // its tuning frequency, which can be found by multiplying its x coordinate by 4000000 and then adding its y coordinate.
        //            return (candidatePoint.X * 4000000) + candidatePoint.Y;
        //        }
        //    }
        //}

        //return inclusionBounds.Count;

        //var map = new Dictionary<Point, char>();

        //foreach (var report in sensorReports)
        //{
        //    foreach (var bounds in report.GetExclusionBounds())
        //    {
        //        foreach (var point in bounds.GetPoints())
        //        {
        //            map[point] = '#';
        //        }
        //    }

        //    map[report.SensorPosition] = 'S';

        //    //foreach (var diamondCoord in report.GetDiamondCoords().Select((v, i) =>(v, i)))
        //    //{
        //    //    map[diamondCoord.v] = diamondCoord.i switch
        //    //    {
        //    //        0 => '^',
        //    //        1 => '>',
        //    //        2 => 'v',
        //    //        3 => '<'
        //    //    };
        //    //}

        //    //var tr = new Vector2(report.MaxSensorX, report.MinSensorY);
        //    //var br = new Vector2(report.MaxSensorX, report.MaxSensorY);
        //    //var bl = new Vector2(report.MinSensorX, report.MaxSensorY);
        //    //var tl = new Vector2(report.MinSensorX, report.MinSensorY);

        //    //// Fill the square
        //    //for (var x = (int)tl.X; x < (int)tr.X; x++)
        //    //{
        //    //    for (var y = (int)tl.Y; y < (int)bl.Y; y++)
        //    //    {
        //    //        map[new Point(x, y)] = '#';
        //    //    }
        //    //}

        //}

        //map[new Point(14, 11)] = 'X';

        //map.ToStringGrid(x => x.Key.AsVector(), x => x.Value, ' ').RenderGridToConsole();

        //return -1;
    }

    //public readonly record struct Point(int X, int Y)
    //{
    //    public Vector2 AsVector() => new(X, Y);
    //}

    /// <summary>
    /// Note that the bounds (min and max) are inclusive.
    /// </summary>
    public record BoundingBox(Vector2 Min, Vector2 Max)
    {
        //public Vector2 GetSizeExclusive() => Max.AsVector() - Min.AsVector();
        //public Vector2 GetSizeInclusive() => Max.AsVector() - Min.AsVector() + Vector2.One;

        //public bool Contains(BoundingBox other) => Contains(other.Min) && Contains(other.Max);

        //public bool Contains(Point position) =>
        //    position.X >= Min.X && position.X <= Max.X &&
        //    position.Y >= Min.Y && position.Y <= Max.Y;

        public bool Contains(Vector2 position) =>
            position.X >= Min.X && position.X <= Max.X &&
            position.Y >= Min.Y && position.Y <= Max.Y;

        //public IEnumerable<Point> GetPointsWithinBounds()
        //{
        //    for (var x = Min.X; x <= Max.X; x++)
        //    {
        //        for (var y = Min.Y; y <= Max.Y; y++)
        //        {
        //            yield return new Point(x, y);
        //        }
        //    }
        //}

        //public static bool Intersect(BoundingBox boxA, BoundingBox boxB, out BoundingBox intersection)
        //{
        //    var xA = Math.Max(boxA.Min.X, boxB.Min.X);
        //    var yA = Math.Max(boxA.Min.Y, boxB.Min.Y);

        //    var xB = Math.Min(boxA.Max.X, boxB.Max.X);
        //    var yB = Math.Min(boxA.Max.Y, boxB.Max.Y);

        //    var intersectionArea = Math.Abs(Math.Max(xB - xA, 0) * Math.Max(yB - yA, 0));

        //    intersection = new BoundingBox(new Point(xA, yA), new Point(xB, yB));

        //    return intersectionArea > 0;
        //}

        //public BoundingBox[] Exclude(BoundingBox exclude)
        //{
        //    //// When they equal, that means there is nothing left of this, everything was excluded
        //    //if (this == exclude)
        //    //{
        //    //    return Array.Empty<BoundingBox>();
        //    //}
        //    // Otherwise, there is the single intersection box (the thing to exclude), and some other boxes around it (the things to include)
        //    // Work out all of the boxes, and any whose area is 0, exclude them


        //    Intersect(this, exclude, out var intersection);

        //    if (!Contains(intersection))
        //    {
        //        return new[] { this };
        //    }

        //    // There will be single intersection box (the thing to exclude), and some other boxes around it (the things to include)
        //    // Note that some (or all) the other boxes could essentially have an area of zero because the intersection lies on an edge, so exclude them.
        //    var surroundingBoxes = new BoundingBox[]
        //    {
        //        new(Min, new Point(Max.X, intersection.Min.Y - 1)), // top box
        //        new(new Point(Min.X, intersection.Max.Y + 1), Max), // bottom box
        //        new(new Point(Min.X, intersection.Min.Y), new Point(intersection.Min.X - 1, intersection.Max.Y)), // left box
        //        new(new Point(intersection.Max.X + 1, intersection.Min.Y), new Point(Max.X, intersection.Max.Y)) // right box
        //    };

        //    //return exceptions.Where(x => x.GetAreaExclusive() > 0).ToArray();
        //    return surroundingBoxes.Where(Contains).ToArray();
        //}

        //public int GetAreaExclusive()
        //{
        //	var size = Max.AsVector() - Min.AsVector();
        //	return Math.Abs((int)size.X * (int)size.Y);
        //}
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

        //public IEnumerable<BoundingBox> GetExclusionBounds()
        //{
        //    var halfDist = DistanceBetweenSensorAndBeacon / 2;

        //    yield return new BoundingBox(
        //        new Point(SensorPosition.X - halfDist, SensorPosition.Y - halfDist),
        //        new Point(SensorPosition.X + halfDist, SensorPosition.Y + halfDist));
        //}

        //public BoundingBox GetExclusionBox()
        //{
        //    var halfDist = ZoneSize / 2;

        //    return new BoundingBox(
        //        new Point(SensorPosition.X - halfDist, SensorPosition.Y - halfDist),
        //        new Point(SensorPosition.X + halfDist, SensorPosition.Y + halfDist));
        //}

        //public IEnumerable<Vector2> GetDiamondCoords()
        //{
        //    yield return new Vector2(SensorPosition.X, MinSensorY);
        //    yield return new Vector2(MaxSensorX, SensorPosition.Y);
        //    yield return new Vector2(SensorPosition.X, MaxSensorY);
        //    yield return new Vector2(MinSensorX, SensorPosition.Y);
        //}

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

        //public (int MinY, int MaxY)? GetMinMaxYForX(int x)
        //{
        //    if (x >= MinSensorX && x <= MaxSensorX)
        //    {
        //        var delta = Math.Abs(x - SensorPosition.X);

        //        var minY = SensorPosition.Y - ZoneSize + delta;
        //        var maxY = SensorPosition.Y + ZoneSize - delta;

        //        return (minY, maxY);
        //    }

        //    return null;
        //}

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

            //if (inclusionBoundingBox.Contains(current) && sensorZones.All(z => !z.Contains(current)))
            //{
            //    return current;
            //}

            foreach (var target in path.Skip(1))
            {
                var dir = Vector2.Clamp(target - current, new(-1, -1), new(1, 1)); // Move diagonally

                while (current != target)
                {
                    current += dir;

                    //if (inclusionBoundingBox.Contains(current) && sensorZones.All(z => !z.Contains(current)))
                    //{
                    //    return current;
                    //}

                    if (IsDistressBeacon(current))
                    {
                        return current;
                    }
                }
            }

            return null;
        }

        public bool Contains(Vector2 point) => MathUtils.ManhattanDistance(SensorPosition, point) <= ZoneSize;

        //public bool Contains(Point p)
        //{
        //    //var distance = MathUtils.ManhattanDistance(SensorPosition.AsVector(), p.AsVector());

        //    //return distance <= ZoneSize;

        //    var minMaxX = GetMinMaxXForY(p.Y);

        //    if (minMaxX != null &&
        //        p.X >= minMaxX.Value.MinX &&
        //        p.X <= minMaxX.Value.MaxX)
        //    {
        //        var minMaxY = GetMinMaxYForX(p.X);

        //        if (minMaxY != null &&
        //            p.Y >= minMaxY.Value.MinY &&
        //            p.Y <= minMaxY.Value.MaxY)
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}
    }

    static bool IsExample(SensorZone[] sensorZones) => sensorZones.Length == 14;

    static SensorZone[] ParseSensorZones(string input) =>
        ParseLineRegex.Matches(input).Select(match => new SensorZone(
            new Vector2(int.Parse(match.Groups["sX"].Value), int.Parse(match.Groups["sY"].Value)),
            new Vector2(int.Parse(match.Groups["bX"].Value), int.Parse(match.Groups["bY"].Value)))).ToArray();

    // Sensor at x=(?<sX>-?\d+), y=(?<sY>-?\d+): closest beacon is at x=(?<bX>-?\d+), y=(?<bY>-?\d+)
    private static readonly Regex ParseLineRegex = BuildParseLineRegex();

    [GeneratedRegex(@"Sensor at x=(?<sX>-?\d+), y=(?<sY>-?\d+): closest beacon is at x=(?<bX>-?\d+), y=(?<bY>-?\d+)", RegexOptions.Compiled)]
    private static partial Regex BuildParseLineRegex();

    ///// <summary>
    ///// Note that bounds are inclusive.
    ///// </summary>
    //public record BoundingBox(Point Min, Point Max)
    //{
    //    public Vector2 SizeExclusive { get; } = Max.AsVector() - Min.AsVector();
    //    public Vector2 SizeInclusive { get; } = Max.AsVector() - Min.AsVector() + Vector2.One;

    //    public bool Contains(Point position) =>
    //        position.X >= Min.X && position.X <= Max.X &&
    //        position.Y >= Min.Y && position.Y <= Max.Y;

    //    public IEnumerable<Point> GetPointsWithinBounds()
    //    {
    //        for (var x = Min.X; x <= Max.X; x++)
    //        {
    //            for (var y = Min.Y; y <= Max.Y; y++)
    //            {
    //                yield return new Point(x, y);
    //            }
    //        }
    //    }

    //    public static bool Intersect(BoundingBox boxA, BoundingBox boxB, out BoundingBox intersection)
    //    {
    //        var xA = Math.Max(boxA.Min.X, boxB.Min.X);
    //        var yA = Math.Max(boxA.Min.Y, boxB.Min.Y);

    //        var xB = Math.Min(boxA.Max.X, boxB.Max.X);
    //        var yB = Math.Min(boxA.Max.Y, boxB.Max.Y);

    //        var intersectionArea = Math.Abs(Math.Max(xB - xA, 0) * Math.Max(yB - yA, 0));

    //        intersection = new BoundingBox(new Point(xA, yA), new Point(xB, yB));

    //        return intersectionArea > 0;
    //    }

    //    public IEnumerable<BoundingBox> Exclude(BoundingBox exclude)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //static void PlotSensorReportCoverage(SensorScan sensorReport, Dictionary<Vector2, char> map, int? targetY)
    //{
    //    var width = sensorReport.DistanceBetweenSensorAndBeacon * 2 + 1;

    //    var sensorX = (int) sensorReport.SensorPosition.X;
    //    var sensorY = (int) sensorReport.SensorPosition.Y;
    //    var relativeY = 0;

    //    while (width > 0)
    //    {
    //        var northY = sensorY - relativeY;
    //        var southY = sensorY + relativeY;

    //        if (northY == targetY || targetY == null)
    //        {
    //            PlotPath(
    //                new Vector2(sensorX - (int)(width / 2f), northY),
    //                new Vector2(sensorX + (int)(width / 2f), northY),
    //                map);
    //        }

    //        if (southY == targetY || targetY == null)
    //        {
    //            PlotPath(
    //                new Vector2(sensorX - (int)(width / 2f), southY),
    //                new Vector2(sensorX + (int)(width / 2f), southY),
    //                map);
    //        }

    //        // new Vector2(SandSource.X - (int) (floorWidth / 2f), floorY),
    //        // new Vector2(SandSource.X + (int)(floorWidth / 2f), floorY)

    //        width -= 2;
    //        relativeY++;
    //    }
    //}

    //static void PlotPath(Vector2 left, Vector2 right, Dictionary<Vector2, char> map)
    //{
    //    if (right.X < left.X)
    //    {
    //        throw new InvalidOperationException("Invalid state");
    //    }

    //    var current = left;
    //    var dir = new Vector2(1, 0); // : Vector2.Normalize(right - left);
    //    var target = right + dir;
    //    while (current != target)
    //    {
    //        map.TryAdd(current, NoBeaconChar);
    //        current += dir;
    //    }
    //}

    // Line intersect
    //public static Vector2 Intersect(Vector2 line1V1, Vector2 line1V2, Vector2 line2V1, Vector2 line2V2)
    //{
    //    //Line1
    //    float A1 = line1V2.Y - line1V1.Y;
    //    float B1 = line1V1.X - line1V2.X;
    //    float C1 = A1 * line1V1.X + B1 * line1V1.Y;

    //    //Line2
    //    float A2 = line2V2.Y - line2V1.Y;
    //    float B2 = line2V1.X - line2V2.X;
    //    float C2 = A2 * line2V1.X + B2 * line2V1.Y;

    //    float det = A1 * B2 - A2 * B1;
    //    if (det == 0)
    //    {
    //        throw new InvalidOperationException("Unxpected parallel lines");
    //    }
    //    else
    //    {
    //        float x = (B2 * C1 - B1 * C2) / det;
    //        float y = (A1 * C2 - A2 * C1) / det;
    //        return new Vector2(x, y);
    //    }
    //}

    //public long? SolvePart2MatrixNo(PuzzleInput input)
    //{
    //    var sensorReports = ParseSensorScans(input);
    //    if (sensorReports.Length != 14)
    //    {
    //        throw new InvalidOperationException("Early exit of actual puzzle input");
    //    }

    //    var rotationMatrix = Matrix3x2.CreateRotation(45.DegreesToRadians());

    //    var map = new Dictionary<Vector2, char>();

    //    foreach (var report in sensorReports)
    //    {
    //        var squareCoords = report.GetDiamondCoords().Select(diamondCoord => Vector2.Transform(diamondCoord, rotationMatrix)).ToArray();

    //        //var tr = squareCoords[0].Round();
    //        //var br = squareCoords[1].Round();
    //        //var bl = squareCoords[2].Round();
    //        //var tl = squareCoords[3].Round();

    //        var tr = new Vector2((int) squareCoords[0].X, (int) squareCoords[0].Y);
    //        var br = new Vector2((int) squareCoords[1].X, (int) squareCoords[1].Y);
    //        var bl = new Vector2((int) squareCoords[2].X, (int) squareCoords[2].Y);
    //        var tl = new Vector2((int) squareCoords[3].X, (int) squareCoords[3].Y);

    //        // Fill the square
    //        for (var x = (int)tl.X; x < (int)tr.X; x++)
    //        {
    //            for (var y = (int)tl.Y; y < (int)bl.Y; y++)
    //            {
    //                map[new Vector2(x, y)] = '#';
    //            }
    //        }

    //        //map[squareCoords[0].Round()] = '1'; // TR
    //        //map[squareCoords[1].Round()] = '2'; // BR
    //        //map[squareCoords[2].Round()] = '3'; // BL
    //        //map[squareCoords[3].Round()] = '4'; // TL
    //        //break;
    //    }

    //    map.ToStringGrid(x => x.Key, x => x.Value, '.').RenderGridToConsole();

    //    return -1;
    //}

    //public long? SolvePart2Nope(PuzzleInput input)
    //{
    //    var sensorReports = ParseSensorScans(input);
    //    var target = sensorReports.Length < 30 ? 20 : 4000000;

    //    var minY = sensorReports.Min(x => x.MinSensorY);
    //    var maxY = sensorReports.Max(x => x.MaxSensorY);
    //    Console.WriteLine(new { minY, maxY });

    //    var minX = sensorReports.Min(x => x.MinSensorX);
    //    var maxX = sensorReports.Max(x => x.MaxSensorX);
    //    Console.WriteLine(new { minX, maxX });

    //    var count = 0;

    //    for (var i = 0; i <= target; i++)
    //    {
    //        //count += sensorReports.Count(x => x.MinSensorY >= y && x.MaxSensorY <= y);
    //        var sensorsOnThisLine = sensorReports.Count(x => i >= x.MinSensorX && i <= x.MaxSensorX &&
    //                                                         i >= x.MinSensorY && i <= x.MaxSensorY);

    //        if (sensorsOnThisLine > 1)
    //        {
    //            count++;
    //        }
    //    }

    //    Console.WriteLine(new { count });

    //    foreach (var sensorReport in sensorReports)
    //    {
    //        Console.WriteLine(sensorReport.DistanceBetweenSensorAndBeacon);
    //    }

    //    return null;
    //}

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

    //public record SensorReport(Vector2 SensorPosition, Vector2 BeaconPosition)
    //{
    //    public long DistanceBetweenSensorAndBeacon { get; } = MathUtils.ManhattanDistance(BeaconPosition, SensorPosition);

    //    public long MinSensorY { get; } = (int) SensorPosition.Y - MathUtils.ManhattanDistance(BeaconPosition, SensorPosition);

    //    public long MaxSensorY { get; } = (int) SensorPosition.Y + MathUtils.ManhattanDistance(BeaconPosition, SensorPosition);

    //    public long MinSensorX { get; } = (int) SensorPosition.X - MathUtils.ManhattanDistance(BeaconPosition, SensorPosition);

    //    public long MaxSensorX { get; } = (int) SensorPosition.X + MathUtils.ManhattanDistance(BeaconPosition, SensorPosition);

    //    public IEnumerable<Vector2> GetDiamondCoords()
    //    {
    //        yield return new Vector2(SensorPosition.X, MinSensorY);

    //        yield return new Vector2(MaxSensorX, SensorPosition.Y);

    //        yield return new Vector2(SensorPosition.X, MaxSensorY);

    //        yield return new Vector2(MinSensorX, SensorPosition.Y);
    //    }

    //    public (int MinX, int MaxX)? GetMinMaxXForY(int y)
    //    {
    //        if (y >= MinSensorY && y <= MaxSensorY)
    //        {
    //            var delta = Math.Abs(y - (int) SensorPosition.Y);

    //            //var minX = (int) SensorPosition.X - (int) DistanceBetweenSensorAndBeacon - delta;
    //            var minX = (int) SensorPosition.X - (int) DistanceBetweenSensorAndBeacon + delta;
    //            var maxX = (int) SensorPosition.X + (int) DistanceBetweenSensorAndBeacon - delta;

    //            return (minX, maxX);
    //        }

    //        return null;
    //    }
    //}

    //static SensorReport[] ParseSensorReports(string input) =>
    //    ParseLineRegex.Matches(input).Select(match => new SensorReport(
    //        new Vector2(int.Parse(match.Groups["sX"].Value), int.Parse(match.Groups["sY"].Value)),
    //        new Vector2(int.Parse(match.Groups["bX"].Value), int.Parse(match.Groups["bY"].Value)))).ToArray();
}
