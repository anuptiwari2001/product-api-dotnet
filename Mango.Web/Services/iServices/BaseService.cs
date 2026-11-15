using System.Net.Http;
using System.Text;
using System.Text.Json;
using Mango.Web.Models;

namespace Mango.Web.Services.iServices
{
    public class BaseService(IHttpClientFactory httpClient) : IBaseService
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
        public ResponseDto ResponseModel { get; set; } = new();
        public IHttpClientFactory HttpClient { get; set; } = httpClient;

        public async Task<T> SendAsync<T>(ApiRequest apiRequest) where T : ResponseDto, new()
        {
            try
            {
                var client = HttpClient.CreateClient("MangoAPI");
                using var message = new HttpRequestMessage
                {
                    RequestUri = new Uri(apiRequest.Url),
                    Method = apiRequest.ApiType switch
                    {
                        SD.ApiType.POST => HttpMethod.Post,
                        SD.ApiType.PUT => HttpMethod.Put,
                        SD.ApiType.DELETE => HttpMethod.Delete,
                        _ => HttpMethod.Get
                    }
                };
                message.Headers.Accept.ParseAdd("application/json");
                if (apiRequest.Data is not null)
                {
                    message.Content = new StringContent(JsonSerializer.Serialize(apiRequest.Data, JsonOptions), Encoding.UTF8, "application/json");
                }

                using var apiResponse = await client.SendAsync(message);
                var apiContent = await apiResponse.Content.ReadAsStringAsync();
                var response = string.IsNullOrWhiteSpace(apiContent)
                    ? CreateFailureResponse<T>("The Product API returned an empty response.")
                    : JsonSerializer.Deserialize<T>(apiContent, JsonOptions)
                        ?? CreateFailureResponse<T>("The Product API returned an empty response.");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    response.IsSuccess = false;
                    if (response.ErrorMessages is not { Count: > 0 } && string.IsNullOrWhiteSpace(response.DisplayMessage))
                    {
                        response.DisplayMessage = $"The Product API returned HTTP {(int)apiResponse.StatusCode} ({apiResponse.ReasonPhrase}).";
                    }
                }

                ResponseModel = response;
                return response;
            }
            catch (HttpRequestException ex)
            {
                return SetFailureResponse<T>($"Unable to reach the Product API: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                return SetFailureResponse<T>($"The Product API request timed out or was canceled: {ex.Message}");
            }
            catch (UriFormatException ex)
            {
                return SetFailureResponse<T>($"The Product API URL is invalid: {ex.Message}");
            }
            catch (JsonException ex)
            {
                return SetFailureResponse<T>($"The Product API returned an invalid response: {ex.Message}");
            }
        }

        private T SetFailureResponse<T>(string message) where T : ResponseDto, new()
        {
            var response = CreateFailureResponse<T>(message);
            ResponseModel = response;
            return response;
        }

        private static T CreateFailureResponse<T>(string message) where T : ResponseDto, new()
        {
            return new T
            {
                IsSuccess = false,
                DisplayMessage = "Product API request failed.",
                ErrorMessages = [message]
            };
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
