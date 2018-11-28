using BenchmarkDotNet.Running;

namespace Microsoft.Data.Sqlite.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
            => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}
