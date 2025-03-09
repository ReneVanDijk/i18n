namespace i18npuzzles;

class Day3 : BaseDay
{
    public Day3() : base(Resource1.i18nday3_example, Resource1.i18nday3, "2")
    {

    }

    protected override string Solve(byte[] data)
    {
        var result = GetLines(data).Count(line =>
                    line.Length >= 4 && line.Length <= 12 &&
                    line.Any(char.IsDigit) &&
                    line.Any(char.IsUpper) &&
                    line.Any(char.IsLower) &&
                    line.Any(ch => ch > 127));

        return $"{result}";
    }
}

