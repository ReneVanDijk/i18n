namespace i18npuzzles;

class Day3 : BaseDay
{
    public Day3() : base(Resource1.i18nday3_example, Resource1.i18nday3, "2")
    {

    }

    protected override string Solve(byte[] data)
    {
        var result = 0;

        var lines = GetLines(data);

        result = lines.Count(LineValid);

        return $"{result}";
    }

    private static bool LineValid(string line) =>
                    line.Length >= 4 && line.Length <= 12 &&
                    line.Any(ch => char.IsDigit(ch)) &&
                    line.Any(ch => char.IsUpper(ch)) &&
                    line.Any(ch => char.IsLower(ch)) &&
                    line.Any(ch => ch > 127);
}

