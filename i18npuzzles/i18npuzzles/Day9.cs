namespace i18npuzzles;

class Day9 : BaseDay
{
    public Day9() : base(Resource1.test_input_9, Resource1.input_9, "Margot Peter")
    {

    }

    private DateOnly NineEleven = new DateOnly(2001, 9, 11);

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

        Dictionary<string, List<string>> datesByName = new();

        foreach (var line in lines)
        {
            var date = line.Split(":")[0];
            var namesOnLine = line.Split(":")[1].Trim();

            foreach (var name in namesOnLine.Split(",").Select(s => s.Trim()))
            {
                var key = name;

                if (datesByName.ContainsKey(key))
                {
                    datesByName[key].Add(date);
                }
                else
                {
                    datesByName.Add(key, [date]);
                }
            }
        }

        List<string> wroteInDiaryOnNineEleven = [];

        foreach (var (name, dates) in datesByName)
        {
            var formatsUsed = formats.Where(format => dates.All(date => DateOnly.TryParseExact(date, format, null, System.Globalization.DateTimeStyles.None, out _)));

            foreach (var format in formatsUsed)
            {
                var formattedDates = dates.Select(date => DateOnly.ParseExact(date, format, null));

                if (formattedDates.Contains(NineEleven))
                    wroteInDiaryOnNineEleven.Add(name);
            }
        }

        string result = string.Join(" ", wroteInDiaryOnNineEleven.OrderBy(s => s).ToArray());
        return $"{result}";
    }
}


