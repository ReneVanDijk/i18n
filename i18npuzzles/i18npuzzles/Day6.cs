using System.Buffers;
using System.Collections.Immutable;
using System.Text;

namespace i18npuzzles;

class Day6 : BaseDay
{
    public Day6() : base(Resource1.test_input_6, Resource1.input_6, "50")
    {

    }

    protected override string Solve(byte[] data)
    {
        var lines = GetLines(data).ToArray();

        var splitIndex = -1;
        for (int i = 0; i < lines.Length; i++)
        {
            if ((i + 1) % 3 == 0)
            {
                lines[i] = Reencode(lines, i);
            }

            if ((i + 1) % 5 == 0)
            {
                lines[i] = Reencode(lines, i);
            }

            if (lines[i] == "")
            {
                splitIndex = i;
                break;
            }
        }

        var words = lines[..splitIndex];
        var crossword = lines[(splitIndex + 1)..];

        var result = 0;

        foreach (var cw in crossword)
        {
            var trimmed = cw.Trim();
            var idx = trimmed.LastIndexOfAny("abcdefghijklmnopqrstuvwxyz".Select(c => c).ToArray());

            var word = words.Index().Single(w =>  w.Item.Length == trimmed.Length && w.Item[idx] == trimmed[idx]);

            result += word.Index +1;
        }

        return $"{result}";
    }

    private static string Reencode(string[] lines, int i)
    {
        return Encoding.UTF8.GetString(Encoding.Convert(Encoding.UTF8, Encoding.Latin1, Encoding.UTF8.GetBytes(lines[i])));
    }
}

