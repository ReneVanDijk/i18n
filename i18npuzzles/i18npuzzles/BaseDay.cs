using System.Text;

namespace i18npuzzles
{
    public abstract class BaseDay
    {
        private readonly byte[] _example;
        private readonly byte[] _actual;
        private readonly string _expected;

        protected BaseDay(byte[] example, byte[] actual, string expected)
        {
            _example = example;
            _actual = actual;
            _expected = expected;
        }

        public void Solve()
        {
            var exampleRes = Solve(_example);
            Console.WriteLine("Example: " + exampleRes);
            Console.WriteLine("Valid:   " + (exampleRes == _expected));
            Console.WriteLine("---");
            Console.WriteLine("Actual:  " + Solve(_actual));
            Console.WriteLine("---");
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
