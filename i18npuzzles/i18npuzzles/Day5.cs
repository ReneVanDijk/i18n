using System.Drawing;
using System.Globalization;

namespace i18npuzzles;

class Day5 : BaseDay
{
    public Day5() : base(Resource1.test_input_5, Resource1.input_5, "2")
    {

    }

    protected override string Solve(byte[] data)
    {
        var lines = GetLines(data).ToList();

        var width = GetTextElements(lines[0]).Count;
        var height = lines.Count;

        Point current = new Point(0, 0);

        int result = 0;
        while (current.Y < height)
        {
            var curLine = lines[current.Y];
            var elements = GetTextElements(curLine);

            if (elements[current.X] == "💩")
                result++;

            if (current.X + 2 >= width)
            {
                current.X = (current.X + 2 - width);
            }
            else
            {
                current.X += 2;
            }

            current.Y += 1;
        }

        return $"{result}";
    }

    private List<string> GetTextElements(string curLine)
    {
        List<string> result = new();
        var enumerator = StringInfo.GetTextElementEnumerator(curLine);
        while (enumerator.MoveNext())
        {
            var element = enumerator.GetTextElement();
            result.Add(element);
        }

        return result;
    }
}

