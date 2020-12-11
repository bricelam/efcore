using BenchmarkDotNet.Attributes;

namespace Microsoft.Data.Sqlite.Benchmarks
{
    public class DataReaderGet
    {
        readonly SqliteConnection _connection;
        readonly SqliteCommand _command;

        public DataReaderGet()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            _command = _connection.CreateCommand();
            _command.CommandText = "SELECT 1, 1.0, '1', x'01'";
        }

        [GlobalCleanup]
        public void Dispose()
        {
            _command.Dispose();
            _connection.Dispose();
        }

        [Benchmark]
        public long Integer()
        {
            using var reader = _command.ExecuteReader();
            reader.Read();

            return reader.GetFieldValue<long>(0);
        }

        [Benchmark]
        public double Real()
        {
            using var reader = _command.ExecuteReader();
            reader.Read();

            return reader.GetFieldValue<double>(1);
        }

        [Benchmark]
        public string Text()
        {
            using var reader = _command.ExecuteReader();
            reader.Read();

            return reader.GetFieldValue<string>(2);
        }

        [Benchmark]
        public byte[] Blob()
        {
            using var reader = _command.ExecuteReader();
            reader.Read();

            return reader.GetFieldValue<byte[]>(3);
        }
    }
}
