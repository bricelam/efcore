using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace Microsoft.Data.Sqlite.Benchmarks
{
    public class Fortunes
    {
        private const string ConnectionString = "Data Source=hello_world;Cache=Shared;Mode=Memory";

        private SqliteConnection _connection;

        [GlobalSetup]
        public void Setup()
        {
            _connection = new SqliteConnection(ConnectionString);
            _connection.Open();
            
            using var command = _connection.CreateCommand();
            command.CommandText =
            @"
                CREATE TABLE fortune (
                    id INTEGER NOT NULL PRIMARY KEY,
                    message TEXT NOT NULL
                );

                INSERT INTO fortune (message)
                VALUES ('fortune: No such file or directory'),
                    ('A computer scientist is someone who fixes things that aren''t broken.'),
                    ('After enough decimal places, nobody gives a damn.'),
                    ('A bad random number generator: 1, 1, 1, 1, 1, 4.33e+67, 1, 1, 1'),
                    ('A computer program does what you tell it to do, not what you want it to do.'),
                    ('Emacs is a nice operating system, but I prefer UNIX. — Tom Christaensen'),
                    ('Any program that runs right is obsolete.'),
                    ('A list is only as strong as its weakest link. — Donald Knuth'),
                    ('Feature: A bug with seniority.'),
                    ('Computers make very fast, very accurate mistakes.'),
                    ('<script>alert(""This should not be displayed in a browser alert box."");</script>'),
                    ('フレームワークのベンチマーク');
            ";
            command.ExecuteNonQuery();
        }

        [Benchmark]
        public List<Fortune> Run()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT id, message FROM fortune";

            using var reader = command.ExecuteReader();

            var result = new List<Fortune>();
            while (reader.Read())
            {
                result.Add(
                    new Fortune
                    {
                        Id = reader.GetInt32(0),
                        Message = reader.GetString(1)
                    });
            }

            return result;
        }

        [GlobalCleanup]
        public void Cleanup()
            => _connection.Dispose();

        public class Fortune
        {
            public int Id { get; set; }
            public string Message { get; set; }
        }
    }
}
