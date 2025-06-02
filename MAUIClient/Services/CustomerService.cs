using MAUIClient.DTO.Customer;
using MAUIClient.Models;
using MAUIClient.Models.RequestHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MAUIClient.Services
{
    public class CustomerService:ICustomerService
    {
        HttpClient _client;
        IRestService _restService;

        public CustomerService(IRestService service)
        {
            _restService = service;
        }

        public async Task<PagedResponse<Customer>> SearchCustomersAsync(
            string searchTerm,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var endpoint = $"api/customer?SearchTerm={Uri.EscapeDataString(searchTerm)}";

            Uri uri = new Uri(string.Format(Constants.RestUrl, endpoint));

            try
            {
                var token = await SecureStorage.GetAsync("jwt_token");
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _client.GetAsync(uri);
                
                if(response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();  
                    var customers = JsonSerializer.Deserialize<List<Customer>>(jsonResponse,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                        ?? new List<Customer>();

                    return new PagedResponse<Customer>
                    {
                        Items = customers,
                        MetaData = new MetaData
                        {
                            TotalCount = customers.Count,
                            CurrentPage = 1,
                            PageSize = customers.Count,
                            TotalPages = 1
                        }
                    };
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await Shell.Current.GoToAsync("//LoginPage");
                    return new PagedResponse<Customer> { Items = new List<Customer>() };
                }
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: {ex.Message}");
            }

            return new PagedResponse<Customer> { Items = new List<Customer>() };
        }
    }
}
