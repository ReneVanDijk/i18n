using System.Text;

namespace i18npuzzles;

class Day11 : BaseDay
{
    public Day11() : base(Resource1.test_input_11, Resource1.input_11, "19")
    {
    }

    private char[] _greekUpperCase = "ΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩ".ToCharArray();
    private char[] _greekLowerCase = "αβγδεζηθικλμνξοπρστυφχψω".ToCharArray();

    protected override string Solve(byte[] data)
    {
        var lines = GetLines(data).ToArray();

        var result = 0;

        foreach (var line in lines)
        {
            var rotations = 0;
            var mutable = line;
            bool found = EndsWithOddysseusessusi(mutable);
            while (!found && rotations <= _greekLowerCase.Length)
            {
                mutable = GreekCeasarCypher(mutable);
                var mutRunes = mutable.EnumerateRunes().ToArray();
                rotations++;
                found = EndsWithOddysseusessusi(mutable);
            }
            if (found)
            {
                result += rotations;
            }
        }

        return $"{result}";
    }

    private char[] fakeChars = ['ʼ', '=', '|', '+'];

    private string GreekCeasarCypher(string line)
    {
        StringBuilder sb = new();

        for (var i = 0; i < line.Length; i++)
        {
            var ch = line[i];

            if (char.IsWhiteSpace(ch) || char.IsPunctuation(ch) || fakeChars.Contains(ch))
            {
                sb.Append(ch);
                continue;
            }

            var alphabet = _greekUpperCase;
            if (char.IsLower(ch))
                alphabet = _greekLowerCase;

            if (ch == 'σ' || ch == 'ς')
            {
                sb.Append('τ');
            }
            else if (ch == 'ρ')
            {
                bool endOfWord = char.IsWhiteSpace(line[i + 1]) || char.IsPunctuation(line[i + 1]);
                if (endOfWord)
                    sb.Append('ς');
                else
                    sb.Append('σ');
            }
            else
            {
                var idx = Array.IndexOf(alphabet, ch) + 1;
                if (idx == alphabet.Length)
                    idx = 0;

                var newCh = alphabet[idx];

                sb.Append(newCh);
            }
        }

        return sb.ToString();
    }

    private bool EndsWithOddysseusessusi(string line) =>
        line.Contains("Οδυσσευς") ||
        line.Contains("Οδυσσεως") ||
        line.Contains("Οδυσσει") ||
        line.Contains("Οδυσσεα") ||
        line.Contains("Οδυσσευ");
}

