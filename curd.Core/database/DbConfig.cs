using System.Text.Json;

namespace curd.Core.database;

public class DbConnectionConfig
{
    required public string Type { get; set; }
    public string? Path { get; set; }
}

public class DbConfig
{
    required public string Active { get; set; }
    required public Dictionary<string, DbConnectionConfig> Configs { get; set; }

    public static DbConfig GetActiveConfig()
    {
        string path = "../../../../curd.Core/config/config.json";
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Config file not found at: {path}");
        }

        var json = File.ReadAllText(path);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        DbConfig config = JsonSerializer.Deserialize<DbConfig>(json, options)
            ?? throw new InvalidOperationException("Failed to deserialize config.json");
        
        if (string.IsNullOrWhiteSpace(config.Active) || config.Configs == null || !config.Configs.ContainsKey(config.Active))
            throw new InvalidDataException(
                "Invalid or missing configuration for the active database.");
        
        return config;
    }

    public static IDatabase InitDatabase()
    {
        DbConfig config = GetActiveConfig();

        if (!config.Configs.TryGetValue(config.Active, out DbConnectionConfig? connectionConfig))
        {
            throw new KeyNotFoundException(
                $"No configuration found for active database '{config.Active}'");
        }

        string dbType = config.Configs[config.Active].Type;
        IDatabase database = connectionConfig.Type switch
        {
            "sqlite" => new SqliteDb(),
            _ => throw new NotSupportedException(
                $"Database type '{connectionConfig.Type}' not supported")
        };

        try
        {
            database.ConnectDatabase(config);
        }
        catch ( Exception ex )
        {
            throw new InvalidOperationException(
                $"Unable to connect to database '{config.Active}" +
                $"of type '{connectionConfig.Type}': {ex.Message}");
        }

        return database;
    }
}