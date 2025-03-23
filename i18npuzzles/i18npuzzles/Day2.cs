using System.Globalization;
using System.Text;

namespace i18npuzzles;

class Day2 : BaseDay
{
    public Day2() : base(Resource1.i18nday2_example, Resource1.i18nday2, "2019-06-05T12:15:00+00:00")
    {

    }

    protected override string Solve(byte[] data)
    {
        Dictionary<DateTimeOffset, int> offSets = new();

        using (StringReader reader = new(Encoding.UTF8.GetString(data)))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                var x = DateTimeOffset.Parse(line);
                var key = x.ToUniversalTime();
                if (offSets.ContainsKey(key))
                {
                    offSets[key] += 1;
                }
                else
                    offSets.Add(key, 1);
            }
        }

        return offSets.Single(kvp => kvp.Value == 4).Key.ToString("yyyy-MM-ddTHH:mm:sszzz");
    }
}

