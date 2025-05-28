using MAUIClient.Models.RequestHelpers;
using MAUIClient.Models.Auth;
using MAUIClient.Models.InvoiceAggregate;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net;

namespace MAUIClient.Services
{
    public class RestService : IRestService
    {
        HttpClient _client;
        JsonSerializerOptions _serializerOptions;
        

        public List<Invoice> Items { get; private set; }

        public RestService()
        {
#if DEBUG
            HttpClientHandler insecureHandler = GetInsecureHandler();
            _client = new HttpClient(insecureHandler);
#else
            _client = new HttpClient();
#endif
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true

            };
        }
        private HttpClientHandler GetInsecureHandler()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert != null && cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };
            return handler;
        }

        /// <summary>
        /// Authenticates user and returns user data with JWT token
        /// </summary>
        public async Task<User> LoginAsync(string username, string password)
        {
            var loginDto = new Login
            {
                Username = username,
                Password = password
            };

            var endpoint = "api/account/login";

            Uri uri = new Uri(string.Format(Constants.RestUrl, endpoint));

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                var response = await _client.PostAsJsonAsync(uri, loginDto);

                response.EnsureSuccessStatusCode();

             
                var content = await response.Content.ReadAsStringAsync();
                var userDto = JsonSerializer.Deserialize<User>(content, _serializerOptions);

                if (userDto == null)
                    throw new InvalidDataException("Received invalid user data from server");

                // Store token in secure storage for future requests
                if (string.IsNullOrEmpty(userDto.Token))
                {
                    throw new InvalidOperationException("Authentication token is missing");
                }

                await SecureStorage.SetAsync("jwt_token", userDto.Token);

                var storedToken = await SecureStorage.GetAsync("jwt_token");
                if (storedToken != userDto.Token)
                    throw new Exception("Failed to securely store authentication token");


                return userDto;
     
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP Error during authentication: {ex.Message}");
                throw new AuthenticationException("Failed to connect to authentication service", ex);
            }
            catch (TaskCanceledException)
            {
                Debug.WriteLine("Authentication request timed out");
                throw new AuthenticationException("Authentication request timed out");
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JSON Parsing Error: {ex.Message}");
                throw new AuthenticationException("Failed to process server response", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected authentication error: {ex.Message}");
                throw new AuthenticationException("Authentication failed", ex);
            }
        }

        /// <summary>
        /// Updates the HttpClient with JWT token from secure storage
        /// </summary>
        private async Task ApplyJwtTokenAsync()
        {
            string token = await SecureStorage.GetAsync("jwt_token");

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer ", token);
            }
            else
            {
                // Remove authorization header if no token is available
                _client.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<PagedResponse<Invoice>> RefreshDataAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string orderBy = "orderDate")
        {
            var endpoint = $"api/invoice?OrderBy={orderBy}"; // Ignore pagination params
            Uri apiUri = new Uri(string.Format(Constants.RestUrl, endpoint));

            try
            {
                var token = await SecureStorage.GetAsync("jwt_token");
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _client.GetAsync(apiUri);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var invoices = JsonSerializer.Deserialize<List<Invoice>>(jsonResponse,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                        ?? new List<Invoice>();

                    return new PagedResponse<Invoice>
                    {
                        Items = invoices,
                        MetaData = new MetaData
                        {
                            TotalCount = invoices.Count,
                            CurrentPage = 1,
                            PageSize = invoices.Count,
                            TotalPages = 1
                        }
                    };
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await Shell.Current.GoToAsync("//LoginPage");
                    return new PagedResponse<Invoice> { Items = new List<Invoice>() };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: {ex.Message}");
            }

            return new PagedResponse<Invoice> { Items = new List<Invoice>() };

        }

    }
}
