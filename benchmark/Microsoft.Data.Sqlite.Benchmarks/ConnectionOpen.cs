using System.IO;
using BenchmarkDotNet.Attributes;

namespace Microsoft.Data.Sqlite.Benchmarks
{
    public class ConnectionOpen
    {
        const string Filename = "encrypted.db";
        const string ConnectionString = "Data Source=" + Filename + ";Password=key";

        readonly SqliteConnection _connection;
        readonly SqliteConnection _closedConnection;

        public ConnectionOpen()
        {
            File.Delete(Filename);
            _connection = new SqliteConnection(ConnectionString);
            _connection.Open();

            using var command = _connection.CreateCommand();
            command.CommandText = "PRAGMA journal_mode=wal;CREATE TABLE dual(dummy);INSERT INTO dual VALUES('X')";
            command.ExecuteNonQuery();

            _closedConnection = new SqliteConnection(ConnectionString);
        }

        [GlobalCleanup]
        public void Dispose()
        {
            _closedConnection.Dispose();
            _connection.Dispose();

            File.Delete(Filename);
        }

        [Benchmark(Baseline = true)]
        public object Baseline()
        {
            using var command = _connection.CreateCommand();
            command.CommandText = "SELECT 1 FROM dual";

            return command.ExecuteScalar();
        }

        [Benchmark]
        public object NewConnection()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1 FROM dual";

            return command.ExecuteScalar();
        }
    }
}
