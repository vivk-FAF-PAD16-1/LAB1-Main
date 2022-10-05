namespace Scrapper.Common.Data
{
    public class EndpointData
    {
        public string Endpoint { get; set; }
        public string DestinationUri { get; set; }
        
        public bool IsValid =>
            Endpoint != null &&
            DestinationUri != null;
    }
}