using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

public class DapperContext
{
    private readonly IConfiguration _configuration;

    public DapperContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(                          
            _configuration.GetSection("ConnectionString").Value  
        );
    }
}
