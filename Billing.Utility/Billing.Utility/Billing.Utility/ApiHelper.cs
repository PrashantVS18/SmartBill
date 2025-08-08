using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

/// <summary>
/// Static utility class for making HTTP API calls with retry logic, logging, and Bearer token support.
/// </summary>
/// 

namespace Billing.Utility
{
    public static class ApiHelper
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly int MaxRetryAttempts = 3;
        private static readonly TimeSpan BaseDelay = TimeSpan.FromSeconds(1);

        static ApiHelper()
        {
            // Configure HttpClient with reasonable defaults
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "ApiHelper/1.0");
        }

        /// <summary>
        /// Performs a GET request and deserializes the response to type T.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response to</typeparam>
        /// <param name="url">The base URL for the request</param>
        /// <param name="queryParams">Optional query parameters to append to the URL</param>
        /// <param name="bearerToken">Optional Bearer token for authorization</param>
        /// <returns>Deserialized response of type T</returns>
        public static async Task<T> GetAsync<T>(string url, Dictionary<string, string>? queryParams = null, string? bearerToken = null)
        {
            var requestUrl = BuildUrlWithQueryParams(url, queryParams);

            return await ExecuteWithRetryAsync<T>(async () =>
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                AddBearerTokenIfProvided(request, bearerToken);

                LogRequest(request);

                using var response = await _httpClient.SendAsync(request);
                LogResponse(response);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return DeserializeJson<T>(content);
            });
        }

        /// <summary>
        /// Performs a POST request with optional body and deserializes the response to type T.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response to</typeparam>
        /// <param name="url">The base URL for the request</param>
        /// <param name="body">Optional request body object to serialize as JSON</param>
        /// <param name="queryParams">Optional query parameters to append to the URL</param>
        /// <param name="bearerToken">Optional Bearer token for authorization</param>
        /// <returns>Deserialized response of type T</returns>
        public static async Task<T> PostAsync<T>(string url, object? body = null, Dictionary<string, string>? queryParams = null, string? bearerToken = null)
        {
            var requestUrl = BuildUrlWithQueryParams(url, queryParams);

            return await ExecuteWithRetryAsync<T>(async () =>
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
                AddBearerTokenIfProvided(request, bearerToken);

                if (body != null)
                {
                    var jsonContent = SerializeToJson(body);
                    request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                }

                LogRequest(request);

                using var response = await _httpClient.SendAsync(request);
                LogResponse(response);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return DeserializeJson<T>(content);
            });
        }

        /// <summary>
        /// Performs a PUT request with optional body and deserializes the response to type T.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response to</typeparam>
        /// <param name="url">The base URL for the request</param>
        /// <param name="body">Optional request body object to serialize as JSON</param>
        /// <param name="queryParams">Optional query parameters to append to the URL</param>
        /// <param name="bearerToken">Optional Bearer token for authorization</param>
        /// <returns>Deserialized response of type T</returns>
        public static async Task<T> PutAsync<T>(string url, object? body = null, Dictionary<string, string>? queryParams = null, string? bearerToken = null)
        {
            var requestUrl = BuildUrlWithQueryParams(url, queryParams);

            return await ExecuteWithRetryAsync<T>(async () =>
            {
                using var request = new HttpRequestMessage(HttpMethod.Put, requestUrl);
                AddBearerTokenIfProvided(request, bearerToken);

                if (body != null)
                {
                    var jsonContent = SerializeToJson(body);
                    request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                }

                LogRequest(request);

                using var response = await _httpClient.SendAsync(request);
                LogResponse(response);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return DeserializeJson<T>(content);
            });
        }

        /// <summary>
        /// Performs a DELETE request and deserializes the response to type T.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response to</typeparam>
        /// <param name="url">The base URL for the request</param>
        /// <param name="queryParams">Optional query parameters to append to the URL</param>
        /// <param name="bearerToken">Optional Bearer token for authorization</param>
        /// <returns>Deserialized response of type T</returns>
        public static async Task<T> DeleteAsync<T>(string url, Dictionary<string, string>? queryParams = null, string? bearerToken = null)
        {
            var requestUrl = BuildUrlWithQueryParams(url, queryParams);

            return await ExecuteWithRetryAsync<T>(async () =>
            {
                using var request = new HttpRequestMessage(HttpMethod.Delete, requestUrl);
                AddBearerTokenIfProvided(request, bearerToken);

                LogRequest(request);

                using var response = await _httpClient.SendAsync(request);
                LogResponse(response);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return DeserializeJson<T>(content);
            });
        }

        #region Private Helper Methods

        /// <summary>
        /// Executes an HTTP operation with exponential backoff retry logic.
        /// </summary>
        private static async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation)
        {
            var attempt = 0;

            while (true)
            {
                try
                {
                    return await operation();
                }
                catch (Exception ex) when (ShouldRetry(ex, attempt))
                {
                    attempt++;
                    var delay = TimeSpan.FromMilliseconds(BaseDelay.TotalMilliseconds * Math.Pow(2, attempt - 1));

                    LogRetryAttempt(attempt, delay, ex);
                    await Task.Delay(delay);
                }
            }
        }

        /// <summary>
        /// Determines if an exception should trigger a retry.
        /// </summary>
        private static bool ShouldRetry(Exception ex, int currentAttempt)
        {
            if (currentAttempt >= MaxRetryAttempts)
                return false;

            return ex switch
            {
                HttpRequestException => true,
                TaskCanceledException => true,
                HttpListenerException => true,
                _ when ex.Message.Contains("timeout") => true,
                _ => false
            };
        }

        /// <summary>
        /// Builds a URL with query parameters appended.
        /// </summary>
        private static string BuildUrlWithQueryParams(string baseUrl, Dictionary<string, string>? queryParams)
        {
            if (queryParams == null || queryParams.Count == 0)
                return baseUrl;

            var uriBuilder = new UriBuilder(baseUrl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            foreach (var param in queryParams)
            {
                query[param.Key] = param.Value;
            }

            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }

        /// <summary>
        /// Adds Bearer token to request headers if provided.
        /// </summary>
        private static void AddBearerTokenIfProvided(HttpRequestMessage request, string? bearerToken)
        {
            if (!string.IsNullOrWhiteSpace(bearerToken))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
            }
        }

        /// <summary>
        /// Serializes an object to JSON string using System.Text.Json.
        /// </summary>
        private static string SerializeToJson(object obj)
        {
            try
            {
                return JsonSerializer.Serialize(obj, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to serialize request body to JSON: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Deserializes JSON string to specified type using System.Text.Json.
        /// </summary>
        private static T DeserializeJson<T>(string json)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json))
                {
                    if (typeof(T) == typeof(string))
                        return (T)(object)string.Empty;

                    throw new InvalidOperationException("Cannot deserialize empty response to non-string type.");
                }

                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }) ?? throw new InvalidOperationException($"Deserialization resulted in null for type {typeof(T).Name}");
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to deserialize JSON response to type {typeof(T).Name}: {ex.Message}", ex);
            }
        }

        #endregion

        #region Logging Methods (Stub Implementation)

        /// <summary>
        /// Logs the outgoing HTTP request. Replace with your preferred logging implementation.
        /// </summary>
        /// <param name="request">The HTTP request message</param>
        private static void LogRequest(HttpRequestMessage request)
        {
            // TODO: Replace with your logging framework (NLog, Serilog, ILogger, etc.)
            Console.WriteLine($"[REQUEST] {request.Method} {request.RequestUri}");

            if (request.Content != null)
            {
                // Note: Reading content here for logging purposes
                // In production, you might want to log headers instead to avoid performance impact
                Console.WriteLine($"[REQUEST HEADERS] {request.Headers}");
            }
        }

        /// <summary>
        /// Logs the HTTP response. Replace with your preferred logging implementation.
        /// </summary>
        /// <param name="response">The HTTP response message</param>
        private static void LogResponse(HttpResponseMessage response)
        {
            // TODO: Replace with your logging framework (NLog, Serilog, ILogger, etc.)
            Console.WriteLine($"[RESPONSE] {(int)response.StatusCode} {response.StatusCode} - {response.RequestMessage?.RequestUri}");
            Console.WriteLine($"[RESPONSE HEADERS] {response.Headers}");
        }

        /// <summary>
        /// Logs retry attempts. Replace with your preferred logging implementation.
        /// </summary>
        /// <param name="attempt">Current retry attempt number</param>
        /// <param name="delay">Delay before retry</param>
        /// <param name="exception">The exception that triggered the retry</param>
        private static void LogRetryAttempt(int attempt, TimeSpan delay, Exception exception)
        {
            // TODO: Replace with your logging framework (NLog, Serilog, ILogger, etc.)
            Console.WriteLine($"[RETRY] Attempt {attempt}/{MaxRetryAttempts} failed. Retrying in {delay.TotalSeconds}s. Error: {exception.Message}");
        }

        #endregion
    }
}
