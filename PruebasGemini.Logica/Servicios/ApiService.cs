using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PruebasGemini.Configurations;
using System.Text;


namespace PruebasGemini.Logica.Servicios
{
    public interface IApiService
    {
        Task<string> PostAsync(string endpoint, object data);
    }
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;

        public ApiService(HttpClient httpClient, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
        }

        public async Task<string> PostAsync(string endpoint, object data)
        {
            try
            {
                var requestContent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_apiSettings.BaseUrl}/{endpoint}?key={_apiSettings.ApiKey}", requestContent);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException httpRequestException)
            {
                // Manejo específico de excepciones HTTP
                throw new Exception($"Error al realizar la solicitud HTTP: {httpRequestException.Message}", httpRequestException);
            }
            catch (Exception ex)
            {
                // Manejo general de excepciones
                throw new Exception($"Error al realizar la solicitud: {ex.Message}", ex);
            }
        }
    }
}
