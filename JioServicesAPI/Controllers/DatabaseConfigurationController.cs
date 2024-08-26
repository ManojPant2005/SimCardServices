using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace JioServicesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseConfigurationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DatabaseConfigurationController> _logger;

        public DatabaseConfigurationController(IConfiguration configuration, ILogger<DatabaseConfigurationController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        [HttpPost]
        public async Task<IActionResult> ConfigureDatabase([FromBody] DatabaseConfig config)
        {
            try
            {
                using var connection = CreateConnection(config);

                var columns = config.Columns.Split(',').Select(c => c.Trim()).ToArray();
                var columnsSql = string.Join(", ", columns.Select(c => $"{c} NVARCHAR(500)")); // Adjust column type as needed

                var createTableQuery = $@"
        CREATE TABLE {config.TableName} (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            {columnsSql},
            CreatedOn DATETIME DEFAULT CURRENT_TIMESTAMP
        )";

                await connection.ExecuteAsync(createTableQuery);

                return Ok("Table created and stored procedures generated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while configuring the database.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        private IDbConnection CreateConnection(DatabaseConfig config)
        {
            return config.DatabaseType switch
            {
                "SQLServer" => new SqlConnection(config.ConnectionString),
                "MySQL" => new MySqlConnection(config.ConnectionString),
                "Oracle" => new OracleConnection(config.ConnectionString),
                "SQLite" => new SqliteConnection(config.ConnectionString),
                _ => throw new NotSupportedException($"Database type {config.DatabaseType} is not supported."),
            };
        }

        public class DatabaseConfig
        {
            public string DatabaseType { get; set; }
            public string ConnectionString { get; set; }
            public string TableName { get; set; }
            public string Columns { get; set; }
        }
    }
}
