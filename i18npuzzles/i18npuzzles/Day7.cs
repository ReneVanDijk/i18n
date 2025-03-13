using System.Buffers;

namespace i18npuzzles;

class Day7 : BaseDay
{
    public Day7() : base(Resource1.test_input_7, Resource1.input_7, "866")
    {

    }

    protected override string Solve(byte[] data)
    {
        var lines = GetLines(data).ToArray();

        var result = 0L;

        var halifax = TimeZoneInfo.FindSystemTimeZoneById("America/Halifax");
        var santiago = TimeZoneInfo.FindSystemTimeZoneById("America/Santiago"); ;

        Dictionary<int, DateTimeOffset> adjusted = new();

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            var dt = DateTimeOffset.Parse(line.Split('\t')[0]);

            var isHalifax = halifax.GetUtcOffset(dt.DateTime) == dt.Offset;
            var isSantiago = santiago.GetUtcOffset(dt.DateTime) == dt.Offset;

            var correct = int.Parse(line.Split('\t')[1]);
            var wrong = int.Parse(line.Split('\t')[2]);

            if (isHalifax)
            {
                adjusted[i] = ConvertTo(halifax, dt, correct, wrong);
            }
            else if (isSantiago)
            {
                adjusted[i] = ConvertTo(santiago, dt, correct, wrong);
            }
            else
            {
                adjusted[i] = ConvertTo(santiago, dt, correct, wrong);
            }
        }

        result = adjusted.Sum(kvp => (kvp.Key + 1) * kvp.Value.Hour);

        return $"{result}";
    }

    private static DateTimeOffset ConvertTo(TimeZoneInfo halifax, DateTimeOffset dt, int correct, int wrong)
    {
        var adjusted = dt.ToUniversalTime()
                                .Subtract(TimeSpan.FromMinutes(wrong))
                                .Add(TimeSpan.FromMinutes(correct));
        var res = TimeZoneInfo.ConvertTime(adjusted, halifax);
        return res;
    }
}

