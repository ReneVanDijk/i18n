namespace i18npuzzles;

class Day4 : BaseDay
{
    public Day4() : base(Resource1._4_test_input, Resource1._4_input, "3143")
    {

    }

    protected override string Solve(byte[] data)
    {
        var lines = GetLines(data).ToList();

        var totalMinutes = 0;

        for (int i = 0; i < lines.Count; i += 3)
        {
            var from = lines[i][10..42].Trim();
            var to = lines[i + 1][10..42].Trim();

            var tzFrom = TimeZoneInfo.FindSystemTimeZoneById(from);
            var tzTo = TimeZoneInfo.FindSystemTimeZoneById(to);

            var dtFrom = DateTime.Parse(lines[i][42..]);
            var dtTo = DateTime.Parse(lines[i + 1][42..]);

            var dtFromOffset = new DateTimeOffset(dtFrom, tzFrom.GetUtcOffset(dtFrom));
            var dtToOffset = new DateTimeOffset(dtTo, tzTo.GetUtcOffset(dtTo));

            var delta = (int)(dtToOffset - dtFromOffset).TotalMinutes;

            totalMinutes += delta;

        }

        return $"{totalMinutes}";
    }
}

