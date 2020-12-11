using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace Microsoft.Data.Sqlite.Benchmarks
{
    // TODO: Batching?
    class Program
    {
        static void Main(string[] args)
        {
            var job = Job.Default
                //.WithCustomBuildConfiguration("Release50")
                //.WithNuGet("Microsoft.Data.Sqlite.Core", "5.0.2")
                //.WithNuGet("Microsoft.Data.Sqlite.Core", "3.1.11")
                .AsDefault();

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(
                args,
                DefaultConfig.Instance.AddJob(job));
        }
    }
}
