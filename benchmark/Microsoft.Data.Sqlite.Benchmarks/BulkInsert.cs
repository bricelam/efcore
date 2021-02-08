using System;
using BenchmarkDotNet.Attributes;

namespace Microsoft.Data.Sqlite.Benchmarks
{
    public class BulkInsert : IDisposable
    {
        const string ConnectionString = "Data Source=BulkInsert;Cache=Shared;Mode=Memory";

        static readonly Random _random = new Random();

        readonly SqliteConnection _connection = new SqliteConnection(ConnectionString);
        SqliteTransaction _transaction;

        [GlobalSetup]
        public void Init()
        {
            _connection.Open();

            var command = _connection.CreateCommand();
            command.CommandText = "CREATE TABLE data(value)";
            command.ExecuteNonQuery();

            _transaction = _connection.BeginTransaction();
        }

        [Benchmark]
        public void Insert()
        {
            var command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO data VALUES($value)";
            command.Parameters.AddWithValue("$value", _random.NextDouble());
            command.ExecuteNonQuery();
        }

        [GlobalCleanup]
        public void Dispose()
        {
            _transaction.Dispose();
            _connection.Dispose();
        }
    }
}
