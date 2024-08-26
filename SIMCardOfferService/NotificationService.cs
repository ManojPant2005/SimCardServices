using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Data;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Data.Sqlite;
using Microsoft.Data.SqlClient;

namespace SIMCardOfferService
{
    public class NotificationService : BackgroundService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly string _connectionString;
        private readonly string _tableName;
        private readonly string _databaseType;

        public NotificationService(ILogger<NotificationService> logger, string connectionString, string tableName, string databaseType)
        {
            _logger = logger;
            _connectionString = connectionString;
            _tableName = tableName;
            _databaseType = databaseType;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Sending offer message at: {time}", DateTimeOffset.Now);
                await SendOfferMessageAsync("Sample Offer Message", stoppingToken); // Modify as needed
                await Task.Delay(60000, stoppingToken); // Run every minute
            }
        }

        private async Task SendOfferMessageAsync(string messageText, CancellationToken cancellationToken)
        {
            try
            {
                using var connection = CreateConnection();
                var query = $"INSERT INTO {_tableName} (MessageText, SentOn) VALUES (@MessageText, CURRENT_TIMESTAMP)";
                using var command = CreateCommand(query, connection);
                var parameter = CreateParameter("@MessageText", messageText);
                command.Parameters.Add(parameter);

                await OpenConnectionAsync(connection, cancellationToken);

                switch (command)
                {
                    case SqlCommand sqlCommand:
                        await sqlCommand.ExecuteNonQueryAsync(cancellationToken);
                        break;
                    case MySqlCommand mySqlCommand:
                        await mySqlCommand.ExecuteNonQueryAsync(cancellationToken);
                        break;
                    case OracleCommand oracleCommand:
                        await oracleCommand.ExecuteNonQueryAsync(cancellationToken);
                        break;
                    case SqliteCommand sqliteCommand:
                        await sqliteCommand.ExecuteNonQueryAsync(cancellationToken);
                        break;
                    default:
                        throw new NotSupportedException($"Command type {command.GetType().Name} is not supported.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending offer message.");
            }
        }

        private async Task OpenConnectionAsync(IDbConnection connection, CancellationToken cancellationToken)
        {
            switch (connection)
            {
                case SqlConnection sqlConnection:
                    await sqlConnection.OpenAsync(cancellationToken);
                    break;
                case MySqlConnection mySqlConnection:
                    await mySqlConnection.OpenAsync(cancellationToken);
                    break;
                case OracleConnection oracleConnection:
                    await oracleConnection.OpenAsync(cancellationToken);
                    break;
                case SqliteConnection sqliteConnection:
                    await sqliteConnection.OpenAsync(cancellationToken);
                    break;
                default:
                    throw new NotSupportedException($"Connection type {connection.GetType().Name} is not supported.");
            }
        }

        private IDbConnection CreateConnection()
        {
            return _databaseType switch
            {
                "SQLServer" => new SqlConnection(_connectionString),
                "MySQL" => new MySqlConnection(_connectionString),
                "Oracle" => new OracleConnection(_connectionString),
                "SQLite" => new SqliteConnection(_connectionString),
                _ => throw new NotSupportedException($"Database type {_databaseType} is not supported."),
            };
        }

        private IDbCommand CreateCommand(string query, IDbConnection connection)
        {
            return _databaseType switch
            {
                "SQLServer" => new SqlCommand(query, (SqlConnection)connection),
                "MySQL" => new MySqlCommand(query, (MySqlConnection)connection),
                "Oracle" => new OracleCommand(query, (OracleConnection)connection),
                "SQLite" => new SqliteCommand(query, (SqliteConnection)connection),
                _ => throw new NotSupportedException($"Database type {_databaseType} is not supported."),
            };
        }

        private IDbDataParameter CreateParameter(string name, object value)
        {
            return _databaseType switch
            {
                "SQLServer" => new SqlParameter(name, value),
                "MySQL" => new MySqlParameter(name, value),
                "Oracle" => new OracleParameter(name, value),
                "SQLite" => new SqliteParameter(name, value),
                _ => throw new NotSupportedException($"Database type {_databaseType} is not supported."),
            };
        }
    }
}
