namespace i18npuzzles
{
    public abstract class BaseDay
    {
        public void Solve()
        {
            Console.WriteLine("Example: " + Solve(Resource1.i18nday2_example));
            Console.WriteLine("Actual: " + Solve(Resource1.i18nday2));
        }

        protected abstract string Solve(byte[] data);
    }
}
