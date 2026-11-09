using System.Net.Http.Json;
using System.Text.Json;

namespace RaceDay.Mvc.Services
{
    public class RaceDayApiClient
    {
        private readonly IHttpClientFactory _factory;
        private readonly IHttpContextAccessor _accessor;

        public RaceDayApiClient(
            IHttpClientFactory factory,
            IHttpContextAccessor accessor)
        {
            _factory = factory;
            _accessor = accessor;
        }

        private HttpClient CreateClient()
        {
            return _factory.CreateClient("RaceDayApi");
        }

        private HttpRequestMessage CreateRequest(
            HttpMethod method, string url)
        {
            var request = new HttpRequestMessage(method, url);

            // Get the API session cookie saved during login.
            string? apiCookie = _accessor.HttpContext?
                .Session.GetString("ApiSessionCookie");

            if (!string.IsNullOrWhiteSpace(apiCookie))
            {
                request.Headers.TryAddWithoutValidation(
                    "Cookie", apiCookie);
            }

            return request;
        }

        public async Task<HttpResponseMessage> SendAsync(
            HttpMethod method, string url, object? body = null)
        {
            var client = CreateClient();

            using var request = CreateRequest(method, url);

            if (body != null)
            {
                request.Content = JsonContent.Create(body);
            }

            return await client.SendAsync(request);
        }

        public async Task<T?> ReadAsync<T>(
            HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                return default;
            }

            string json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<T>(json, options);
        }

        public static string? ExtractSessionCookie(
            HttpResponseMessage response)
        {
            if (!response.Headers.TryGetValues(
                "Set-Cookie", out var values))
            {
                return null;
            }

            string? cookie = values.FirstOrDefault(v =>
                v.StartsWith(
                    ".AspNetCore.Session=",
                    StringComparison.OrdinalIgnoreCase));

            return cookie?.Split(';', 2)[0];
        }
    }
}