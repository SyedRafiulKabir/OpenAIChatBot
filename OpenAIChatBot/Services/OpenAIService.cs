using Newtonsoft.Json;
using OpenAIChatBot.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;

public class OpenAIService
{
    private readonly string _apiKey;

    public OpenAIService(string apiKey)
    {
        _apiKey = apiKey;
    }

    public async Task<ChatMessage> GetResponse(List<Message> messages)
    {
        using (var client = new HttpClient())
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");

            request.Headers.Add("Authorization", $"Bearer {_apiKey}");
            request.Headers.Add("Accept", "application/json");

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = messages
            };
            var jsonBody = JsonConvert.SerializeObject(requestBody);
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var chatResponse = JsonConvert.DeserializeObject<ChatMessage>(responseString);
                    return chatResponse;
                }
                else if (response.StatusCode == HttpStatusCode.RequestTimeout) // Handle 429 specifically
                {
                    throw new HttpRequestException($"Failed with status code {response.StatusCode}. Response: {await response.Content.ReadAsStringAsync()}");
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Failed with status code {response.StatusCode}. Response: {errorResponse}");
                }
            }
            catch (HttpRequestException ex)
            {
                // Log exception details
                Console.WriteLine($"HttpRequestException: {ex.Message}");
                throw; // Rethrow to propagate the exception
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"Exception: {ex.Message}");
                throw; // Rethrow to propagate the exception
            }
        }
    }
}
