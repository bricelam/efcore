using BenchmarkDotNet.Attributes;

namespace Microsoft.Data.Sqlite.Benchmarks
{
    public class CommandExecute
    {
        readonly SqliteConnection _connection;
        readonly SqliteCommand _command;

        public CommandExecute()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            _command = _connection.CreateCommand();
            _command.CommandText = "SELECT 1";
            _command.Prepare();
        }

        [GlobalCleanup]
        public void Dispose()
        {
            _command.Dispose();
            _connection.Dispose();
        }

        [Benchmark(Baseline = true)]
        public object Baseline()
        {
            return _command.ExecuteScalar();
        }

        [Benchmark]
        public object NewCommand()
        {
            using var command = _connection.CreateCommand();
            command.CommandText = "SELECT 1";

            return command.ExecuteScalar();
        }

        [Benchmark]
        public object NewConnection()
        {
            using var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";

            return command.ExecuteScalar();
        }
    }
}
