namespace Scrapper.Common.Data
{
    public struct ConfigurationData
    {
        public string MySqlAddress { get; set; }
        public string UserId { get; set; }
        public string UserPwd { get; set; }
        public string DB { get; set; }
        public string OpenWeatherApiKey { get; set; }
        public string DiscoveryUri { get; set; }
        public string RegistrationUri { get; set; }
        public string[] ScrapperPrefixes { get; set; }
        
        
    }
}