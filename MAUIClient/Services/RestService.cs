using MAUIClient.Models.InvoiceAggregate;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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

        public async Task<List<Invoice>> RefreshDataAsync()
        {
            Items = new List<Invoice>();

            Uri apiUri = new Uri(string.Format(Constants.RestUrl, "api/Invoice"));

            try
            {
                HttpResponseMessage response = await _client.GetAsync(apiUri);
                //Checks if Status code is success (Status 200)
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Items = JsonSerializer.Deserialize<List<Invoice>>(content);   
                }

            }
            catch (Exception ex) 
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return Items;

        }
    }
}
