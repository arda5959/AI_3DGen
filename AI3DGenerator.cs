using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace AI_3DGen
{
    public class AI3DGenerator
    {
        private readonly string _apiEndpoint;
        private readonly HttpClient _httpClient;

        public AI3DGenerator(string apiEndpoint = "https://api.ai3dgenerator.com")
        {
            _apiEndpoint = apiEndpoint;
            _httpClient = new HttpClient();
        }

        public async Task<AI3DModelResponse> Generate3DModelAsync(string prompt)
        {
            try
            {
                // Prepare the request
                var request = new AI3DModelRequest
                {
                    Prompt = prompt ?? throw new ArgumentNullException(nameof(prompt)),
                    Style = "realistic",
                    Resolution = "high",
                    Format = "obj"
                };

                // Serialize the request
                var content = new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8,
                    "application/json"
                );

                // Send the request
                var response = await _httpClient.PostAsync($"{_apiEndpoint}/generate", content);
                response.EnsureSuccessStatusCode();

                // Read and deserialize the response
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<AI3DModelResponse>(responseContent) 
                    ?? throw new InvalidOperationException("Failed to deserialize response");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating 3D model: {ex.Message}", ex);
            }
        }

        public class AI3DModelRequest
        {
            public required string Prompt { get; set; }
            public required string Style { get; set; }
            public required string Resolution { get; set; }
            public required string Format { get; set; }
        }

        public class AI3DModelResponse
        {
            public required string ModelUrl { get; set; }
            public required string Status { get; set; }
            public required string ErrorMessage { get; set; }
            public required Dictionary<string, string> Metadata { get; set; }
        }
    }
}
