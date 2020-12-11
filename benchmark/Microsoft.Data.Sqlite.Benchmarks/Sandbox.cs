using System;
using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using SQLitePCL;
using static SQLitePCL.raw;

namespace Microsoft.Data.Sqlite.Benchmarks
{
    public class Sandbox
    {
        const int Iterations = 2;
        static readonly Random _random = new Random();
        const string Sql = "INSERT INTO data VALUES(?)";

        sqlite3 _db;

        [GlobalSetup]
        public void Init()
        {
            Batteries_V2.Init();

            var rc = sqlite3_open(":memory:", out _db);
            Debug.Assert(rc == SQLITE_OK);

            rc = sqlite3_exec(_db, "CREATE TABLE data(value)");
            Debug.Assert(rc == SQLITE_OK);

            rc = sqlite3_exec(_db, "BEGIN");
            Debug.Assert(rc == SQLITE_OK);
        }

        [Benchmark(Baseline = true)]
        public void Baseline()
        {
            for (int i = 0; i < Iterations; i++)
            {
                var rc = sqlite3_prepare_v3(_db, Sql, 0, out var stmt);
                Debug.Assert(rc == SQLITE_OK);

                rc = sqlite3_bind_double(stmt, 0, _random.NextDouble());
                Debug.Assert(rc == SQLITE_OK);

                rc = sqlite3_step(stmt);
                Debug.Assert(rc == SQLITE_DONE);

                rc = sqlite3_finalize(stmt);
                Debug.Assert(rc == SQLITE_OK);
            }
        }

        [Benchmark]
        public void Prototype1()
        {
            var rc = sqlite3_prepare_v3(_db, Sql, 0, out var stmt);
            Debug.Assert(rc == SQLITE_OK);

            for (int i = 0; i < Iterations; i++)
            {
                rc = sqlite3_bind_double(stmt, 0, _random.NextDouble());
                Debug.Assert(rc == SQLITE_OK);

                rc = sqlite3_step(stmt);
                Debug.Assert(rc == SQLITE_DONE);

                rc = sqlite3_reset(stmt);
                Debug.Assert(rc == SQLITE_OK);
            }

            rc = sqlite3_finalize(stmt);
            Debug.Assert(rc == SQLITE_OK);
        }

        //[Benchmark]
        //public void Prototype2()
        //{
        //    int rc;

        //    for (int i = 0; i < 2; i++)
        //    {
        //        rc = sqlite3_prepare_v3(_db, Sql, 0, out var stmt1);
        //        Debug.Assert(rc == SQLITE_OK);

        //        rc = sqlite3_bind_double(stmt1, 0, _random.NextDouble());
        //        Debug.Assert(rc == SQLITE_OK);

        //        rc = sqlite3_step(stmt1);
        //        Debug.Assert(rc == SQLITE_DONE);

        //        rc = sqlite3_finalize(stmt1);
        //        Debug.Assert(rc == SQLITE_OK);
        //    }

        //    /////////////////////////

        //    rc = sqlite3_prepare_v3(_db, Sql, 0, out var stmt);
        //    Debug.Assert(rc == SQLITE_OK);

        //    rc = sqlite3_bind_double(stmt, 0, _random.NextDouble());
        //    Debug.Assert(rc == SQLITE_OK);

        //    rc = sqlite3_step(stmt);
        //    Debug.Assert(rc == SQLITE_DONE);

        //    rc = sqlite3_reset(stmt);
        //    Debug.Assert(rc == SQLITE_OK);

        //    for (int i = 0; i < Iterations - 3; i++)
        //    {
        //        rc = sqlite3_bind_double(stmt, 0, _random.NextDouble());
        //        Debug.Assert(rc == SQLITE_OK);

        //        rc = sqlite3_step(stmt);
        //        Debug.Assert(rc == SQLITE_DONE);

        //        rc = sqlite3_reset(stmt);
        //        Debug.Assert(rc == SQLITE_OK);
        //    }

        //    rc = sqlite3_finalize(stmt);
        //    Debug.Assert(rc == SQLITE_OK);
        //}

        [GlobalCleanup]
        public void Dispose()
        {

            var rc = sqlite3_exec(_db, "COMMIT");
            Debug.Assert(rc == SQLITE_OK);

            rc = sqlite3_close(_db);
            Debug.Assert(rc == SQLITE_OK);
        }
    }
}
