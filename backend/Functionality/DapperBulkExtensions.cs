using System.Data;
using System.Text;
using Dapper;

namespace Backend.Functionality;

public static class DapperBulkExtensions
{
    public static async Task BulkInsertIfNotExistsAsync<T>(this IDbConnection connection, IEnumerable<T> data, string primaryKey = "Id")
    {
        if (data == null || !data.Any())
            return;

        string tableName = typeof(T).Name; // Use class name as table name
        var properties = typeof(T).GetProperties().Where(p => p.CanRead).ToList();
        
        var sql = new StringBuilder();
        var parameters = new DynamicParameters();
        int index = 0;

        foreach (var item in data)
        {
            // Build an "INSERT IF NOT EXISTS" query (Supports SQL Server, MySQL, PostgreSQL)
            sql.Append($"INSERT INTO {tableName} (");
            sql.Append(string.Join(", ", properties.Select(p => p.Name)));
            sql.Append(") SELECT ");
            sql.Append(string.Join(", ", properties.Select(p => $"@{p.Name}_{index}")));
            sql.Append($" WHERE NOT EXISTS (SELECT 1 FROM {tableName} WHERE {primaryKey} = @PrimaryKey_{index}); ");

            foreach (var prop in properties)
            {
                parameters.Add($"@{prop.Name}_{index}", prop.GetValue(item));
            }

            parameters.Add($"@PrimaryKey_{index}", properties.First(p => p.Name == primaryKey).GetValue(item));
            index++;
        }

        await connection.ExecuteAsync(sql.ToString(), parameters);
    }
}
