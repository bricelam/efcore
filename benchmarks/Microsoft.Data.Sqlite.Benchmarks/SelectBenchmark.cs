using System.IO;
using BenchmarkDotNet.Attributes;

namespace Microsoft.Data.Sqlite.Benchmarks
{
    public class SelectBenchmark
    {
        const string ConnectionString = "Data Source=select.db";
        SqliteConnection _connection;
        SqliteCommand _command;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _connection = new SqliteConnection(ConnectionString);
            File.Delete(_connection.DataSource);
            _connection.Open();

            using (var command = _connection.CreateCommand())
            {
                command.CommandText = @"
                    CREATE TABLE test(value TEXT NOT NULL);
                    INSERT INTO test VALUES(hex(randomblob(512)))";
                command.ExecuteNonQuery();
            }

            _command = _connection.CreateCommand();
            _command.CommandText = "SELECT * FROM test";
            _command.Prepare();
        }

        [Benchmark(Baseline = true)]
        public string Baseline()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM test";
                    return (string)command.ExecuteScalar();
                }
            }
        }

        [Benchmark]
        public string ConnectionPool()
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM test";
                return (string)command.ExecuteScalar();
            }
        }

        [Benchmark]
        public string CommandCache()
        {
            return (string)_command.ExecuteScalar();
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _command.Dispose();
            _connection.Dispose();
        }
    }
}
