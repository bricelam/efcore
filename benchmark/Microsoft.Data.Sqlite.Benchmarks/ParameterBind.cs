using BenchmarkDotNet.Attributes;

namespace Microsoft.Data.Sqlite.Benchmarks
{
    public class ParameterBind
    {
        readonly SqliteConnection _connection;
        readonly SqliteCommand _command;
        readonly SqliteParameter _parameter;

        public ParameterBind()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            _command = _connection.CreateCommand();
            _command.CommandText = "SELECT $v";
            _command.Prepare();

            _parameter = _command.CreateParameter();
            _parameter.ParameterName = "$v";
            _command.Parameters.Add(_parameter);
        }

        [GlobalCleanup]
        public void Dispose()
        {
            _command.Dispose();
            _connection.Dispose();
        }

        [Benchmark]
        public void Integer()
        {
            _parameter.Value = 1L;

            _command.ExecuteNonQuery();
        }

        [Benchmark]
        public void Real()
        {
            _parameter.Value = 1.0;

            _command.ExecuteNonQuery();
        }

        [Benchmark]
        public void Text()
        {
            _parameter.Value = "1";

            _command.ExecuteNonQuery();
        }

        [Benchmark]
        public void Blob()
        {
            _parameter.Value = new byte[] { 1 };

            _command.ExecuteNonQuery();
        }
    }
}
