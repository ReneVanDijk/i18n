namespace i18npuzzles;

class Day9 : BaseDay
{
    public Day9() : base(Resource1.test_input_9, Resource1.input_9, "Margot Peter")
    {

    }

    protected override string Solve(byte[] data)
    {
        var lines = GetLines(data);

        var formats = new string[]
        {
            "yy-MM-dd",
            "yy-dd-MM",
            "dd-MM-yy",
            "MM-dd-yy"
        };

        Dictionary<string, List<string>> byName = new();

        foreach (var line in lines)
        {
            var numbers = line.Split(":")[0].Split("-");

            var namesOnLine = line.Split(":")[1].Trim();

            foreach (var name in namesOnLine.Split(",").Select(s => s.Trim()))
            {
                var key = name;

                if (byName.ContainsKey(key))
                {
                    byName[key].Add(line.Split(":")[0]);
                }
                else
                {
                    byName.Add(key, [line.Split(":")[0]]);
                }
            }
        }

        List<string> names = new List<string>();

        foreach (var name in byName)
        {
            var dates = name.Value;

            var formatsUsed = formats.Where(format => dates.All(date => DateTime.TryParseExact(date, format, null, System.Globalization.DateTimeStyles.None, out _)));

            foreach (var format in formatsUsed)
            {
                var formattedDates = dates.Select(date => DateTime.ParseExact(date, format, null));

                if (formattedDates.Any(f => f.Year == 2001 && f.Month == 9 && f.Day == 11))
                    names.Add(name.Key);
            }
        }

        string result = string.Join(" ", names.OrderBy(s => s).ToArray());
        return $"{result}";
    }
}


