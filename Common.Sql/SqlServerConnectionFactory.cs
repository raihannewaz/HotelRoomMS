using System.Data;
using Ardalis.GuardClauses;
using Common.Abstractions.EFCoreConnection;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Common.Sql;

public class SqlServerConnectionFactory : IDbConnectionCreator
{
    private readonly SqlServerOptions _options;
    private IDbConnection? _connection;

    public SqlServerConnectionFactory(IOptions<SqlServerOptions> options)
    {
        _options = Guard.Against.Null(options.Value, nameof(SqlServerOptions));
        Guard.Against.NullOrEmpty(
            _options.ConnectionString,
            nameof(_options.ConnectionString),
            "ConnectionString can't be empty or null.");
    }

    public IDbConnection GetOrCreateConnection()
    {
        if (_connection is null || _connection.State != ConnectionState.Open)
        {
            _connection = new SqlConnection(_options.ConnectionString);
            _connection.Open();
        }

        return _connection;
    }

    public void Dispose()
    {
        if (_connection is { State: ConnectionState.Open })
            _connection.Dispose();
    }
}
