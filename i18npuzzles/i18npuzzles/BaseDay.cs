﻿using System.Diagnostics;
using System.Text;

namespace i18npuzzles
{
    public abstract class BaseDay
    {
        private readonly byte[] _example;
        private byte[] _actual;
        private readonly string _answerForExample;

        protected BaseDay(byte[] example, byte[] actual, string answerForExample)
        {
            _example = example;
            _actual = actual;
            _answerForExample = answerForExample;
        }

        public void Solve()
        {
            var sw = Stopwatch.StartNew();
            var exampleResult = Solve(_example);
            Console.WriteLine("Example: " + exampleResult);
            Console.WriteLine("Valid:   " + (exampleResult == _answerForExample));
            Console.WriteLine("---");
            Console.WriteLine("Actual:  " + Solve(_actual));
            Console.WriteLine("---");
            sw.Stop();

            Console.WriteLine($"Total duration: {sw.Elapsed}");
        }

        protected abstract string Solve(byte[] data);

        protected IEnumerable<string> GetLines(byte[] data)
        {
            using (StringReader reader = new(Encoding.UTF8.GetString(data)))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}
