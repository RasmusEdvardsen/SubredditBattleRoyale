using System.Data;
using System.Text;
using Dapper;

namespace Backend.Functionality;

public static class DapperBulkExtensions
{
    public static async Task BulkInsertAsync<T>(this IDbConnection connection, IEnumerable<T> data)
    {
        if (data == null || !data.Any())
            return;

        string tableName = typeof(T).Name; // Use class name as table name
        var properties = typeof(T).GetProperties().Where(p => p.CanRead).ToList();

        var sql = new StringBuilder($"INSERT INTO {tableName} (");
        sql.Append(string.Join(", ", properties.Select(p => p.Name)));
        sql.Append(") VALUES ");

        var parameters = new DynamicParameters();
        int index = 0;

        foreach (var item in data)
        {
            var valuePlaceholders = properties.Select(p => $"@{p.Name}_{index}").ToList();
            sql.Append($"({string.Join(", ", valuePlaceholders)}),");

            foreach (var prop in properties)
            {
                parameters.Add($"@{prop.Name}_{index}", prop.GetValue(item));
            }
            index++;
        }

        sql.Length--; // Remove trailing comma
        await connection.ExecuteAsync(sql.ToString(), parameters);
    }
}
