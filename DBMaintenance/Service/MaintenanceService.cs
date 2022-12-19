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
        while (!CheckCurrentAddress())
        {
            _currentAddressId += 1;
            if (_dbAddresses.Length <= _currentAddressId) _currentAddressId = 0;
        }
        
        return _dbAddresses[_currentAddressId];
    }

    private bool CheckCurrentAddress()
    {
        var sqlConnectionString = $"Server={_dbAddresses[_currentAddressId]};Port=3306;User ID=root;Password=root;" +
                                  $"Database=mysql";
        using MySqlConnection connection = new MySqlConnection(sqlConnectionString);
        
        connection.ConnectionString = sqlConnectionString;
        try
        {
            connection.Open();
            connection.Close();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }
}