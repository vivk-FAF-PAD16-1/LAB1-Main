using System.Data.SqlClient;
using MySqlConnector;

namespace DBMaintenance.Service;

public class MaintenanceService
{
    private string[] _dbAddresses;

    private int _currentAddressId;

    public MaintenanceService(string[] dbAddresses)
    {
        _dbAddresses = dbAddresses;
    }

    public string GetCurrentSqlDatabaseAddress()
    {
        if (CheckCurrentAddress()) return _dbAddresses[_currentAddressId];
        
        _currentAddressId += 1;
        if (_dbAddresses.Length >= _currentAddressId) _currentAddressId = 0;
        
        return _dbAddresses[_currentAddressId];
    }

    private bool CheckCurrentAddress()
    {
        var sqlConnectionString = $"Server={_dbAddresses[_currentAddressId]};User ID=root;Password=rootpswdmaster1;" +
                                  $"Database=test";
        
        using MySqlConnection connection = new MySqlConnection(sqlConnectionString);
        
        connection.ConnectionString = sqlConnectionString;
        try
        {
            connection.Open();
            connection.Close();
            return true;
        }
        catch (SqlException)
        {
            return false;
        }
    }

    private bool IsServerConnected(string connectionString)
    {
        using SqlConnection connection = new SqlConnection();

        try
        {
            connection.Open();
            connection.Close();
            return true;
        }
        catch (SqlException)
        {
            return false;
        }
    }
}