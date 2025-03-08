using System.Globalization;
using System.Text;

namespace i18npuzzles;

class Day2 : BaseDay
{
    protected override string Solve(byte[] data)
    {
        var result = 0;

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

        return offSets.Single(kvp => kvp.Value == 4).Key.ToString("s", CultureInfo.InvariantCulture);
    }
}

