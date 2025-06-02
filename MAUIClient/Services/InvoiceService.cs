using MAUIClient.DTO;
using MAUIClient.Models.InvoiceAggregate;
using MAUIClient.Models.RequestHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MAUIClient.Services
{
    public class InvoiceService:IInvoiceService
    {
        IRestService _restService;
        HttpClient _client;
        JsonSerializerOptions _serializerOptions;

        public InvoiceService(IRestService service)
        {
            _restService = service;
        }

        public Task<PagedResponse<Invoice>> GetInvoicesAsync()
        {
            return _restService.RefreshDataAsync();
        }

        public List<InvoiceItem> InvoiceItems { get; set; } = new();


        public async Task<string> CreateInvoiceAsync(CreateInvoiceDto invoiceDto)
        {
            var endpoint = $"api/invoice";
            Uri apiUri = new Uri(string.Format(Constants.RestUrl, endpoint));

            try
            {
                // Gets JWT Token
                var token = await SecureStorage.GetAsync("jwt_token");
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                //Convert opject to JSON
                var json = JsonSerializer.Serialize(invoiceDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(apiUri, content);

                //Checks if its not successfull
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();

                    throw new Exception($"Couldn't save Invoice: {error}");
                }

                var invoiceId = await response.Content.ReadAsStringAsync();

                return invoiceId;


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: {ex.Message}");
                return "";
            }
        }

        public async Task<Invoice> UpdateInvoiceAsync(string invoiceid,UpdateInvoiceDto invoiceDto)
        {
            var endpoint = $"api/invoice/{invoiceid}";
            Uri apiUri = new Uri(string.Format(Constants.RestUrl, endpoint));

            try
            {
                // Gets JWT Token
                var token = await SecureStorage.GetAsync("jwt_token");
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                //Convert opject to JSON
                var json = JsonSerializer.Serialize(invoiceDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PutAsync(apiUri, content);

                //Checks if its not successfull
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();

                    throw new Exception($"Couldn't save Invoice: {error}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var invoice = JsonSerializer.Deserialize<Invoice>(responseContent);

                return invoice!;


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: {ex.Message}");

                return new Invoice { };
            }

        }
    }
}
