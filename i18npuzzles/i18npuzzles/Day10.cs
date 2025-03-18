using System.Data;
using System.Text;

namespace i18npuzzles;

class Day10 : BaseDay
{
    public Day10() : base(Resource1.test_input_10, Resource1.input_10, "4")
    {
    }

    protected override string Solve(byte[] data)
    {
        var lines = GetLines(data).ToArray();
        List<string> salts, loginAttempts;
        Parse(lines, out salts, out loginAttempts);

        int result = 0;
        Parallel.ForEach(loginAttempts.GroupBy(l => l), attempt =>
        {
            var username = attempt.Key.Split(" ")[0];
            var password = attempt.Key.Split(" ")[1];

            var userpass = salts.Single(s => s.StartsWith(username)).Split(" ")[1];
            var salt = userpass[..29];

            var compositions = AllCompositions(password.Normalize(NormalizationForm.FormC)).ToArray();
            foreach (var composition in compositions)
            {
                var hash = BCrypt.Net.BCrypt.HashPassword(composition, salt);
                if (hash == userpass)
                {
                    Interlocked.Add(ref result, attempt.Count());
                    break;
                }
            }
        });

        return $"{result}";
    }

    private static void Parse(string[] lines, out List<string> salts, out List<string> loginAttempts)
    {
        salts = new List<string>();
        loginAttempts = new List<string>();
        var flipped = false;
        foreach (var l in lines)
        {
            if (!flipped)
            {
                if (l == "")
                {
                    flipped = true;
                }
                else
                {
                    salts.Add(l);
                }
            }
            else
            {
                loginAttempts.Add(l);
            }
        }
    }

    private IEnumerable<string> AllCompositions(string userpass)
    {
        var runes = userpass.EnumerateRunes()
            .Index()
            .Where(r => !r.Item.IsAscii).ToArray();

        var asdfasfd = Stuff(runes).ToArray();

        foreach (var runez in asdfasfd)
        {
            StringBuilder sb = new();

            foreach (var ridx in userpass.EnumerateRunes().Index())
            {
                if (runez.Any(r => r.Index == ridx.Index))
                {
                    foreach (var r in Decompose(ridx.Item))
                    {
                        sb.Append(r);
                    }
                }
                else
                {
                    sb.Append(ridx.Item);
                }
            }

            yield return sb.ToString();
        }
    }

    static List<(int Index, Rune Item)[]> Stuff((int Index, Rune Item)[] runes) =>
        TotallyNotChatGpt(runes).Select(x => x.ToArray()).ToList();

    static List<List<(int Index, Rune Item)>> TotallyNotChatGpt((int Index, Rune Item)[] input)
    {
        List<List<(int Index, Rune Item)>> result = new List<List<(int Index, Rune Item)>>();
        int n = input.Length;

        for (int i = 0; i < (1 << n); i++)
        {
            List<(int Index, Rune Item)> subset = new List<(int Index, Rune Item)>();
            for (int j = 0; j < n; j++)
            {
                if ((i & (1 << j)) != 0)
                {
                    subset.Add(input[j]);
                }
            }
            result.Add(subset);
        }

        return result;
    }

    static IEnumerable<Rune> Decompose(Rune rune)
    {
        string decomposed = rune.ToString().Normalize(NormalizationForm.FormD);

        foreach (char c in decomposed)
        {
            yield return new Rune(c);
        }
    }
}
