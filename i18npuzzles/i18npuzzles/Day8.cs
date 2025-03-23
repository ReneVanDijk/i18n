using System.Buffers;
using System.Globalization;
using System.Text;

namespace i18npuzzles;

class Day8 : BaseDay
{
    public Day8() : base(Resource1.test_input_8, Resource1.input_8, "2")
    {

    }

    protected override string Solve(byte[] data)
    {
        var result = GetLines(data)
            .Select(RemoveDiacritics)
            .Select(s => s.ToLower())
            .Index().Count(Valid);


        return $"{result}";
    }

    bool Valid((int Index, string line) x)
    {
        var lengthValid = x.line.Length is >= 4 and <= 12;
        var containsDigits = x.line.Any(char.IsDigit);
        var containsVowel = x.line.Any(IsVowel);
        var containsConsonant = x.line.Any(IsConsonant);
        var lacksRecurrence = !ContainsRecurring(x.line);

        var valid =
            lengthValid &&
            containsDigits &&
            containsVowel &&
            containsConsonant &&
            lacksRecurrence;

        Console.WriteLine($"{x.Index}\t{valid}\t{x.line}");

        return valid;
    }

    bool IsVowel(char ch) => "aeiou".Contains(ch);

    bool IsConsonant(char ch) => "bcdfghjklmnpqrstvwxyz".Contains(ch);

    bool ContainsRecurring(string str)
    {
        str = str.ToLower();
        for (int i = 0; i < str.Length; i++)
        {
            if (str.Count(ch => ch == str[i]) > 1)
            {
                return true;
            }
        }
        return false;
    }

    string RemoveDiacritics(string str)
    {
        var chars =
            from c in str.Normalize(NormalizationForm.FormD).ToCharArray()
            let uc = CharUnicodeInfo.GetUnicodeCategory(c)
            where uc != UnicodeCategory.NonSpacingMark
            select c;

        var cleanStr = new string(chars.ToArray()).Normalize(NormalizationForm.FormC);

        return cleanStr;
    }
}

