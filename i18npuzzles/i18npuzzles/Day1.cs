using System.Text;

namespace i18npuzzles;

class Day1
{
    const int SMSByteMax = 160;
    const int TweetCharsMax = 140;

    public int Solve()
    {
        var result = 0;

        using (StringReader reader = new(Encoding.UTF8.GetString(Resource1.i18nday1)))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                var numChars = line.Length;
                var numBytes = Encoding.UTF8.GetBytes(line).Length;

                var validSMS = numBytes <= SMSByteMax;
                var validTweet = numChars <= TweetCharsMax;

                if (validSMS && validTweet)
                    result += 13;
                else if (validSMS)
                    result += 11;
                else if (validTweet)
                    result += 7;
            }
        }

        return result;
    }
}

