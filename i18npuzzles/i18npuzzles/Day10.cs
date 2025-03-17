using System.Diagnostics;
using System.Text;

namespace i18npuzzles;

class Day10 : BaseDay
{
    public Day10() : base(Resource1.test_input_10, Resource1.input_10, "4")
    {
        var bcryptTest = BCrypt.Net.BCrypt.HashPassword("secret", "$2b$10$v3I80pwHtgxp2ampg4Opy.");
        Debug.Assert(bcryptTest == "$2b$10$v3I80pwHtgxp2ampg4Opy.hehc03wCR.JBZE6WHsrSQtxred57/PG");
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
        Parallel.ForEach(loginAttempts, attempt =>
        {
            var username = attempt.Split(" ")[0];
            var password = attempt.Split(" ")[1];

            var salt = salts.Single(s => s.StartsWith(username)).Split(" ")[1][..29];
            var userpass = salts.Single(s => s.StartsWith(username)).Split(" ")[1];

            var decomposed = Decomposed(password.Normalize(NormalizationForm.FormC)).ToArray();
            foreach (var option in decomposed)
            {
                var x = option.EnumerateRunes().ToArray();
                var hash = BCrypt.Net.BCrypt.HashPassword(option, salt);
                if (hash == userpass)
                {
                    //Console.WriteLine(attempt);
                    Interlocked.Increment(ref result);
                }
            }
        });

        return $"{result}";
    }

    private IEnumerable<string> Decomposed(string userpass)
    {
        yield return userpass;

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

    static IEnumerable<(int Index, Rune Item)[]> Stuff((int Index, Rune Item)[] runes)
    {
        for (int i = 0; i < runes.Length; i++)
        {
            for (int j = i; j < runes.Length; j++)
            {
                yield return runes[i..(j + 1)];
            }
        }
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
