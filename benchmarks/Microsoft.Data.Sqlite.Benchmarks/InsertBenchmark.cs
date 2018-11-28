using System;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace Microsoft.Data.Sqlite.Benchmarks
{
    public class InsertBenchmark
    {
        const string ConnectionString = "Data Source=insert.db";
        SqliteConnection _connection;
        SqliteCommand _command;
        SqliteParameter _parameter;
        Random _random = new Random();

        [GlobalSetup]
        public void GlobalSetup()
        {
            _connection = new SqliteConnection(ConnectionString);
            File.Delete(_connection.DataSource);
            _connection.Open();

            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE test(value TEXT NOT NULL)";
                command.ExecuteNonQuery();
            }

            _command = _connection.CreateCommand();
            _command.CommandText = "INSERT INTO test VALUES($value)";
            _parameter = _command.CreateParameter();
            _parameter.ParameterName = "$value";
            _command.Parameters.Add(_parameter);
            _command.Prepare();
        }

        [Benchmark(Baseline = true)]
        public int Baseline()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO test VALUES($value)";
                    command.Parameters.AddWithValue("$value", GetValue());

                    return command.ExecuteNonQuery();
                }
            }
        }

        [Benchmark]
        public int ConnectionPool()
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO test VALUES($value)";
                command.Parameters.AddWithValue("$value", GetValue());

                return command.ExecuteNonQuery();
            }
        }

        [Benchmark]
        public int CommandCache()
        {
            _parameter.Value = GetValue();

            return _command.ExecuteNonQuery();
        }

        public string GetValue()
        {
            var buffer = new byte[512];
            _random.NextBytes(buffer);

            var builder = new StringBuilder(buffer.Length * 2);

            for (int i = 0; i < buffer.Length; i++)
            {
                builder.Append(buffer[i].ToString("X2"));
            }

            return builder.ToString();
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _command.Dispose();
            _connection.Dispose();
        }
    }
}
