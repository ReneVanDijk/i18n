using System.Data;
using System.Diagnostics;
using System.Text;

namespace i18npuzzles;

class Day10 : BaseDay
{
    public Day10() : base(Resource1.test_input_10, Resource1.input_10, "4")
    {
        var bcryptTest = BCrypt.Net.BCrypt.HashPassword("secret", "$2b$10$v3I80pwHtgxp2ampg4Opy.");
        Debug.Assert(bcryptTest == "$2b$10$v3I80pwHtgxp2ampg4Opy.hehc03wCR.JBZE6WHsrSQtxred57/PG");

        var compositions = Stuff([(1, new Rune('a')), (2, new Rune('b')), (3, new Rune('c'))]).ToArray();
        Debug.Assert(compositions.Length == 8);
    }

    protected override string Solve(byte[] data)
    {
        var lines = GetLines(data).ToArray();

        var salts = new List<string>();
        var loginAttempts = new List<string>();
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

        int result = 0;
        //foreach (var attempt in loginAttempts)
        Parallel.ForEach(loginAttempts.GroupBy(l => l), attempt =>
        {
            var username = attempt.Key.Split(" ")[0];
            var password = attempt.Key.Split(" ")[1];

            var salt = salts.Single(s => s.StartsWith(username)).Split(" ")[1][..29];
            var userpass = salts.Single(s => s.StartsWith(username)).Split(" ")[1];

            var norm = password.Normalize(NormalizationForm.FormC);
            var passRunes = password.EnumerateRunes().ToArray();
            var normRunes = norm.EnumerateRunes().ToArray();

            //Debug.Assert(passRunes.Length == norm.Length);
            var decomposed = Decomposed(norm).ToArray();
            var decomRunes = decomposed.Select(dec => dec.EnumerateRunes().ToArray());
            foreach (var option in decomposed)
            {
                var x = option.EnumerateRunes().ToArray();
                var hash = BCrypt.Net.BCrypt.HashPassword(option, salt);
                if (hash == userpass)
                {
                    //Console.WriteLine(attempt.Key);
                    Interlocked.Add(ref result, attempt.Count());
                    break;
                }
            }
        });

        return $"{result}";
    }

    private IEnumerable<string> Decomposed(string userpass)
    {
        var runes = userpass.EnumerateRunes().Index().Where(r => !r.Item.IsAscii).ToArray();

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

    static List<(int Index, Rune Item)[]> Stuff((int Index, Rune Item)[] runes)
    {
        return GenerateSubsets(runes).Select(x => x.ToArray()).ToList();
        //if (runes.Length == 1)
        //    yield return [runes.Single()];

        //for (int i = 0; i < runes.Length; i++)
        //{
        //    foreach (var x in Stuff(runes.Skip(i+1).ToArray()))
        //    {
        //        yield return [runes[i], .. x];
        //        yield return [.. x];
        //    }
        //}
    }

    static List<List<(int Index, Rune Item)>> GenerateSubsets((int Index, Rune Item)[] input)
    {
        List<List<(int Index, Rune Item)>> result = new List<List<(int Index, Rune Item)>>();
        int n = input.Length;

        for (int i = 0; i < (1 << n); i++) // Iterate through all bitmask possibilities
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
