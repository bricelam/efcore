using BenchmarkDotNet.Running;

namespace Microsoft.Data.Sqlite.Benchmarks
{
    internal class Program
    {
        private static void Main(string[] args)
            => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly)
                .Run(args);
    }
}
