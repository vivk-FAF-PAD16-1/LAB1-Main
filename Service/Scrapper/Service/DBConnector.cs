namespace Scrapper.Service;

public class DBConnector
{
    private string _user;
    private string _pwd;
    private string _db;

    private HttpClient _httpClient;
    
    
    
    public DBConnector(string dbMaintenanceUri, string user, string pwd, string db)
    {
        _user = user;
        _pwd = pwd;
        _db = db;
        
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(dbMaintenanceUri);
    }
    
    public string GenerateConnectionString()
    {
        var dbAddress = 
         _httpClient.GetStringAsync("/").Result;
        
        var sqlConnectionString =
            $"Server={dbAddress};User ID={_user};Password={_pwd};" +
            $"Database={_db}";

        return sqlConnectionString;
    }
}