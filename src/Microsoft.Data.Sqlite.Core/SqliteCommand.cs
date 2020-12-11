// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite.Properties;
using SQLitePCL;
using static SQLitePCL.raw;

namespace Microsoft.Data.Sqlite
{
    /// <summary>
    ///     Represents a SQL statement to be executed against a SQLite database.
    /// </summary>
    /// <seealso href="https://docs.microsoft.com/dotnet/standard/data/sqlite/batching">Batching</seealso>
    /// <seealso href="https://docs.microsoft.com/dotnet/standard/data/sqlite/database-errors">Database Errors</seealso>
    /// <seealso href="https://docs.microsoft.com/dotnet/standard/data/sqlite/async">Async Limitations</seealso>
    public class SqliteCommand : DbCommand
    {
        private SqliteParameterCollection? _parameters;

        private string _commandText = string.Empty;
        private int? _commandTimeout;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SqliteCommand" /> class.
        /// </summary>
        public SqliteCommand()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SqliteCommand" /> class.
        /// </summary>
        /// <param name="commandText">The SQL to execute against the database.</param>
        public SqliteCommand(string? commandText)
            => CommandText = commandText;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SqliteCommand" /> class.
        /// </summary>
        /// <param name="commandText">The SQL to execute against the database.</param>
        /// <param name="connection">The connection used by the command.</param>
        public SqliteCommand(string? commandText, SqliteConnection? connection)
            : this(commandText)
        {
            Connection = connection;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SqliteCommand" /> class.
        /// </summary>
        /// <param name="commandText">The SQL to execute against the database.</param>
        /// <param name="connection">The connection used by the command.</param>
        /// <param name="transaction">The transaction within which the command executes.</param>
        public SqliteCommand(string? commandText, SqliteConnection? connection, SqliteTransaction? transaction)
            : this(commandText, connection)
            => Transaction = transaction;

        /// <summary>
        ///     Gets or sets a value indicating how <see cref="CommandText" /> is interpreted. Only
        ///     <see cref="CommandType.Text" /> is supported.
        /// </summary>
        /// <value>A value indicating how <see cref="CommandText" /> is interpreted.</value>
        public override CommandType CommandType
        {
            get => CommandType.Text;
            set
            {
                if (value != CommandType.Text)
                {
                    throw new ArgumentException(Resources.InvalidCommandType(value));
                }
            }
        }

        /// <summary>
        ///     Gets or sets the SQL to execute against the database.
        /// </summary>
        /// <value>The SQL to execute against the database.</value>
        /// <seealso href="https://docs.microsoft.com/dotnet/standard/data/sqlite/batching">Batching</seealso>
        [AllowNull]
        public override string CommandText
        {
            get => _commandText;
            // TODO: Clear prepared
            set => _commandText = value ?? string.Empty;
        }

        /// <summary>
        ///     Gets or sets the connection used by the command.
        /// </summary>
        /// <value>The connection used by the command.</value>
        // TODO: Clear prepared
        public new virtual SqliteConnection? Connection { get; set; }

        /// <summary>
        ///     Gets or sets the connection used by the command. Must be a <see cref="SqliteConnection" />.
        /// </summary>
        /// <value>The connection used by the command.</value>
        protected override DbConnection? DbConnection
        {
            get => Connection;
            set => Connection = (SqliteConnection?)value;
        }

        /// <summary>
        ///     Gets or sets the transaction within which the command executes.
        /// </summary>
        /// <value>The transaction within which the command executes.</value>
        public new virtual SqliteTransaction? Transaction { get; set; }

        /// <summary>
        ///     Gets or sets the transaction within which the command executes. Must be a <see cref="SqliteTransaction" />.
        /// </summary>
        /// <value>The transaction within which the command executes.</value>
        protected override DbTransaction? DbTransaction
        {
            get => Transaction;
            set => Transaction = (SqliteTransaction?)value;
        }

        /// <summary>
        ///     Gets the collection of parameters used by the command.
        /// </summary>
        /// <value>The collection of parameters used by the command.</value>
        /// <seealso href="https://docs.microsoft.com/dotnet/standard/data/sqlite/parameters">Parameters</seealso>
        public new virtual SqliteParameterCollection Parameters
            => _parameters ??= new SqliteParameterCollection();

        /// <summary>
        ///     Gets the collection of parameters used by the command.
        /// </summary>
        /// <value>The collection of parameters used by the command.</value>
        protected override DbParameterCollection DbParameterCollection
            => Parameters;

        /// <summary>
        ///     Gets or sets the number of seconds to wait before terminating the attempt to execute the command.
        ///     Defaults to 30. A value of 0 means no timeout.
        /// </summary>
        /// <value>The number of seconds to wait before terminating the attempt to execute the command.</value>
        /// <remarks>
        ///     The timeout is used when the command is waiting to obtain a lock on the table.
        /// </remarks>
        /// <seealso href="https://docs.microsoft.com/dotnet/standard/data/sqlite/database-errors">Database Errors</seealso>
        public override int CommandTimeout
        {
            get => _commandTimeout ?? Connection?.DefaultTimeout ?? 30;
            set => _commandTimeout = value;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the command should be visible in an interface control.
        /// </summary>
        /// <value>A value indicating whether the command should be visible in an interface control.</value>
        public override bool DesignTimeVisible { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating how the results are applied to the row being updated.
        /// </summary>
        /// <value>A value indicating how the results are applied to the row being updated.</value>
        public override UpdateRowSource UpdatedRowSource { get; set; }

        /// <summary>
        ///     Releases any resources used by the connection and closes it.
        /// </summary>
        /// <param name="disposing">
        ///     <see langword="true" /> to release managed and unmanaged resources;
        ///     <see langword="false" /> to release only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            // TODO: Return prepared
            base.Dispose(disposing);
        }

        /// <summary>
        ///     Creates a new parameter.
        /// </summary>
        /// <returns>The new parameter.</returns>
        public new virtual SqliteParameter CreateParameter()
            => new();

        /// <summary>
        ///     Creates a new parameter.
        /// </summary>
        /// <returns>The new parameter.</returns>
        protected override DbParameter CreateDbParameter()
            => CreateParameter();

        /// <summary>
        ///     Creates a prepared version of the command on the database.
        /// </summary>
        public override void Prepare()
        {
            if (Connection?.State != ConnectionState.Open)
            {
                throw new InvalidOperationException(Resources.CallRequiresOpenConnection(nameof(Prepare)));
            }

            // TODO: Clear preapared
            // TODO: Ensure statements are cached. Eagerly reserve them for this command?
        }

        /// <summary>
        ///     Executes the <see cref="CommandText" /> against the database and returns a data reader.
        /// </summary>
        /// <returns>The data reader.</returns>
        /// <exception cref="SqliteException">A SQLite error occurs during execution.</exception>
        /// <seealso href="https://docs.microsoft.com/dotnet/standard/data/sqlite/database-errors">Database Errors</seealso>
        /// <seealso href="https://docs.microsoft.com/dotnet/standard/data/sqlite/batching">Batching</seealso>
        public new virtual SqliteDataReader ExecuteReader()
            => ExecuteReader(CommandBehavior.Default);

        /// <summary>
        ///     Executes the <see cref="CommandText" /> against the database and returns a data reader.
        /// </summary>
        /// <param name="behavior">A description of the results of the query and its effect on the database.</param>
        /// <returns>The data reader.</returns>
        /// <exception cref="SqliteException">A SQLite error occurs during execution.</exception>
        /// <seealso href="https://docs.microsoft.com/dotnet/standard/data/sqlite/database-errors">Database Errors</seealso>
        /// <seealso href="https://docs.microsoft.com/dotnet/standard/data/sqlite/batching">Batching</seealso>
        public new virtual SqliteDataReader ExecuteReader(CommandBehavior behavior)
        {
            if (Connection?.State != ConnectionState.Open)
            {
                throw new InvalidOperationException(Resources.CallRequiresOpenConnection(nameof(ExecuteReader)));
            }

            if (Transaction != Connection.Transaction)
            {
                throw new InvalidOperationException(
                    Transaction == null
                        ? Resources.TransactionRequired
                        : Resources.TransactionConnectionMismatch);
            }

            if (Connection.Transaction?.ExternalRollback == true)
            {
                throw new InvalidOperationException(Resources.TransactionCompleted);
            }

            var timer = new Stopwatch();
            var closeConnection = behavior.HasFlag(CommandBehavior.CloseConnection);

            // TODO: Parameters, Connection, and CommandTimeout might mutate while enumerating
            var dataReader = new SqliteDataReader(this, timer, GetStatements(timer), closeConnection);
            dataReader.NextResult();

            return dataReader;
        }

        private IEnumerable<SqlitePooledStatement> GetStatements(Stopwatch timer)
        {
            foreach (var stmt in PrepareAndEnumerateStatements(timer))
            {
                var boundParams = _parameters?.Bind(stmt.Value) ?? 0;

                var expectedParams = sqlite3_bind_parameter_count(stmt.Value);
                if (expectedParams != boundParams)
                {
                    var unboundParams = new List<string>();
                    for (var i = 1; i <= expectedParams; i++)
                    {
                        // TODO: Avoid conversion to string?
                        var name = sqlite3_bind_parameter_name(stmt.Value, i).utf8_to_string();

                        if (_parameters != null
                            && !_parameters.Cast<SqliteParameter>().Any(p => p.ParameterName == name))
                        {
                            unboundParams.Add(name);
                        }
                    }

                    if (sqlite3_libversion_number() < 3_028_000 || sqlite3_stmt_isexplain(stmt.Value) == 0)
                    {
                        throw new InvalidOperationException(Resources.MissingParameters(string.Join(", ", unboundParams)));
                    }
                }

                yield return stmt;
            }
        }

        /// <summary>
        ///     Executes the <see cref="CommandText" /> against the database and returns a data reader.
        /// </summary>
        /// <param name="behavior">A description of query's results and its effect on the database.</param>
        /// <returns>The data reader.</returns>
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
            => ExecuteReader(behavior);

        /// <summary>
        ///     Executes the <see cref="CommandText" /> asynchronously against the database and returns a data reader.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <seealso href="https://docs.microsoft.com/dotnet/standard/data/sqlite/async">Async Limitations</seealso>
        /// <seealso href="https://docs.microsoft.com/dotnet/standard/data/sqlite/batching">Batching</seealso>
        /// <seealso href="https://docs.microsoft.com/dotnet/standard/data/sqlite/database-errors">Database Errors</seealso>
        public new virtual Task<SqliteDataReader> ExecuteReaderAsync()
            => ExecuteReaderAsync(CommandBehavior.Default, CancellationToken.None);

        /// <summary>
        ///     Executes the <see cref="CommandText" /> asynchronously against the database and returns a data reader.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <seealso href="https://docs.microsoft.com/dotnet/standard/data/sqlite/async">Async Limitations</seealso>
        /// <seealso href="https://docs.microsoft.com/dotnet/standard/data/sqlite/batching">Batching</seealso>
        /// <seealso href="https://docs.microsoft.com/dotnet/standard/data/sqlite/database-errors">Database Errors</seealso>
        /// <exception cref="OperationCanceledException"> If the <see cref="CancellationToken"/> is canceled. </exception>
        public new virtual Task<SqliteDataReader> ExecuteReaderAsync(CancellationToken cancellationToken)
            => ExecuteReaderAsync(CommandBehavior.Default, cancellationToken);

        /// <summary>
        ///     Executes the <see cref="CommandText" /> asynchronously against the database and returns a data reader.
        /// </summary>
        /// <param name="behavior">A description of query's results and its effect on the database.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <seealso href="https://docs.microsoft.com/dotnet/standard/data/sqlite/async">Async Limitations</seealso>
        /// <seealso href="https://docs.microsoft.com/dotnet/standard/data/sqlite/batching">Batching</seealso>
        /// <seealso href="https://docs.microsoft.com/dotnet/standard/data/sqlite/database-errors">Database Errors</seealso>
        public new virtual Task<SqliteDataReader> ExecuteReaderAsync(CommandBehavior behavior)
            => ExecuteReaderAsync(behavior, CancellationToken.None);

        /// <summary>
        ///     Executes the <see cref="CommandText" /> asynchronously against the database and returns a data reader.
        /// </summary>
        /// <param name="behavior">A description of query's results and its effect on the database.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <seealso href="https://docs.microsoft.com/dotnet/standard/data/sqlite/async">Async Limitations</seealso>
        /// <seealso href="https://docs.microsoft.com/dotnet/standard/data/sqlite/batching">Batching</seealso>
        /// <seealso href="https://docs.microsoft.com/dotnet/standard/data/sqlite/database-errors">Database Errors</seealso>
        /// <exception cref="OperationCanceledException"> If the <see cref="CancellationToken"/> is canceled. </exception>
        public new virtual Task<SqliteDataReader> ExecuteReaderAsync(
            CommandBehavior behavior,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(ExecuteReader(behavior));
        }

        /// <summary>
        ///     Executes the <see cref="CommandText" /> asynchronously against the database and returns a data reader.
        /// </summary>
        /// <param name="behavior">A description of query's results and its effect on the database.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <seealso href="https://docs.microsoft.com/dotnet/standard/data/sqlite/async">Async Limitations</seealso>
        /// <exception cref="OperationCanceledException"> If the <see cref="CancellationToken"/> is canceled. </exception>
        protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(
            CommandBehavior behavior,
            CancellationToken cancellationToken)
            => await ExecuteReaderAsync(behavior, cancellationToken).ConfigureAwait(false);

        /// <summary>
        ///     Executes the <see cref="CommandText" /> against the database.
        /// </summary>
        /// <returns>The number of rows inserted, updated, or deleted. -1 for SELECT statements.</returns>
        /// <exception cref="SqliteException">A SQLite error occurs during execution.</exception>
        /// <seealso href="https://docs.microsoft.com/dotnet/standard/data/sqlite/database-errors">Database Errors</seealso>
        public override int ExecuteNonQuery()
        {
            if (Connection?.State != ConnectionState.Open)
            {
                throw new InvalidOperationException(Resources.CallRequiresOpenConnection(nameof(ExecuteNonQuery)));
            }

            var reader = ExecuteReader();
            reader.Dispose();

            return reader.RecordsAffected;
        }

        /// <summary>
        ///     Executes the <see cref="CommandText" /> against the database and returns the result.
        /// </summary>
        /// <returns>The first column of the first row of the results, or null if no results.</returns>
        /// <exception cref="SqliteException">A SQLite error occurs during execution.</exception>
        /// <seealso href="https://docs.microsoft.com/dotnet/standard/data/sqlite/database-errors">Database Errors</seealso>
        public override object? ExecuteScalar()
        {
            if (Connection?.State != ConnectionState.Open)
            {
                throw new InvalidOperationException(Resources.CallRequiresOpenConnection(nameof(ExecuteScalar)));
            }

            using var reader = ExecuteReader();
            return reader.Read()
                ? reader.GetValue(0)
                : null;
        }

        /// <summary>
        ///     Attempts to cancel the execution of the command. Does nothing.
        /// </summary>
        public override void Cancel()
        {
        }

        private IEnumerable<SqlitePooledStatement> PrepareAndEnumerateStatements(Stopwatch timer)
        {
            var length = Encoding.UTF8.GetByteCount(_commandText);
            var commandText = new byte[length + 1];
            Encoding.UTF8.GetBytes(_commandText, commandText);
            commandText[length] = 0;

            var tail = 0;
            do
            {
                var stmt = Connection!.StatementPool!.Get(
                    commandText.AsSpan(tail),
                    timer,
                    CommandTimeout * 1000L,
                    out var nextTail);
                tail = commandText.Length - nextTail.Length;

                // Statement was empty, white space, or a comment
                if (stmt.Value.IsInvalid)
                {
                    // TODO: Return to pool. Should this logic move?
                    if (tail < length)
                    {
                        continue;
                    }

                    break;
                }

                yield return stmt;
            }
            while (tail < length);
        }
    }
}
