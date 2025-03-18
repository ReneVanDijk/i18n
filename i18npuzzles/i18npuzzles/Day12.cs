using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace i18npuzzles;

class Day12 : BaseDay
{
    public Day12() : base(Resource1.test_input_12, Resource1.input_12, "1885816494308838")
    {
    }

    protected override string Solve(byte[] data)
    {
        var linez = GetLines(data).ToArray();
        var lines = linez.Select(line => (
            phoneNumber: long.Parse(line.Split(":")[1]),
            lastName: line.Split(":")[0].Split(',')[0],
            firstName: line.Split(":")[0].Split(',')[1])
        ).ToArray();

        var english = lines.OrderBy(line => line.lastName, new EnglishComparer()).ToArray();
        var swedish = lines.OrderBy(line => line.lastName, new SwedishComparer()).ToArray();
        var dutch = lines.OrderBy(line => line.lastName, new DutchComparer()).ToArray();

        var englishMiddle = english[english.Length / 2].phoneNumber;
        var swedishMiddle = swedish[swedish.Length / 2].phoneNumber;
        var dutchMiddle = dutch[dutch.Length / 2].phoneNumber;

        var result = new long[] { englishMiddle, swedishMiddle, dutchMiddle }
            .Aggregate((a, b) => a * b);

        return $"{result}";
    }

    private abstract class CustomComparer
    {
        protected readonly CompareInfo _compareInfo;

        protected CustomComparer(string culture)
        {
            _compareInfo = new CultureInfo(culture).CompareInfo;
        }

        protected string? EnglishRule(string? str)
        {
            if (str == null) return str;
            StringBuilder sb = new StringBuilder();

            foreach (var ch in str)
            {
                if (char.IsLetter(ch))
                    sb.Append(ch);
            }
            return sb.ToString();
        }
    }
    private class EnglishComparer : CustomComparer, IComparer<string>
    {
        public EnglishComparer() : base("en-US")
        {

        }

        public int Compare(string? x, string? y) =>
            _compareInfo.Compare(EnglishRule(x), EnglishRule(y));
    }



    private class SwedishComparer : CustomComparer, IComparer<string>
    {
        public SwedishComparer() : base("sv-SE")
        {
        }

        public int Compare(string? x, string? y) =>
            _compareInfo.Compare(EnglishRule(x), EnglishRule(y));
    }

    private class DutchComparer : CustomComparer, IComparer<string>
    {
        public DutchComparer() : base("nl-NL")
        {
        }

        public int Compare(string? x, string? y)
        {
            return _compareInfo.Compare(RemoveInfixes(EnglishRule(x)), RemoveInfixes(EnglishRule(y)));
        }

        private string? RemoveInfixes(string? tehStr)
        {
            if (tehStr == null) return tehStr;
            for (int i = 0; i < tehStr.Length; i++)
                if (char.IsUpper(tehStr[i]))
                    return tehStr[i..];
            throw new UnreachableException();
        }
    }
}

