using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Gateway.Common;
using Gateway.Common.Data;

namespace Gateway.Router
{
	public class GatewayRouter : IRouter
	{
		private readonly string _discoveryUri;

		private readonly HttpClient _httpClient;

		private const string Get = "GET";
		
        public GatewayRouter(string discoveryUri)
        {
	        _discoveryUri = discoveryUri.TrimWeb();

	        _httpClient = new HttpClient();
        }
        
		public async void Route(HttpListenerRequest request, HttpListenerResponse response)
		{
			var absolutePath = request.Url.AbsolutePath;
			var discoveryFullUri = _discoveryUri + absolutePath;
			
			var discoveryRequest = new HttpRequestMessage(new HttpMethod(Get), discoveryFullUri);
			var discoveryResponse = await _httpClient.SendAsync(discoveryRequest);
			var discoveryContent = HttpUtilities.ReadResponseBody(discoveryResponse);

			var uriData = JsonSerializer.Deserialize<UriData>(discoveryContent,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			var content = HttpUtilities.ReadRequestBody(request);

			var serviceRequest = new HttpRequestMessage(new HttpMethod(request.HttpMethod), uriData.Uri);
			serviceRequest.Content = new StringContent(content, Encoding.UTF8, request.ContentType);

			var serviceResponse = await _httpClient.SendAsync(serviceRequest);
			var serviceContent = HttpUtilities.ReadResponseBody(serviceResponse);

			HttpUtilities.SendResponseMessage(response, serviceContent, (int)serviceResponse.StatusCode);
			
			response.Close();
		}
	}
}