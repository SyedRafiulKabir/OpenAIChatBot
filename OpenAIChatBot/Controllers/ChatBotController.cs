using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using OpenAIChatBot.Models;

namespace OpenAIChatBot.Controllers
{
    [RoutePrefix("api/chatbot")]
    public class ChatbotController : ApiController
    {
        private readonly OpenAIService _openAIService;

        public ChatbotController()
        {
            var apiKey = ConfigurationManager.AppSettings["API_KEY"];
            _openAIService = new OpenAIService(apiKey);
        }

        [HttpPost]
        [Route("getresponse")]
        public async Task<IHttpActionResult> GetResponse([FromBody] Message userMessage)
        {
            if (userMessage == null || string.IsNullOrEmpty(userMessage.content))
            {
                return BadRequest("Invalid input.");
            }

            // Prepare the messages list for the OpenAI API request
            var messages = new List<Message>
    {
        new Message
        {
            role = "user",
            content = userMessage.content
        }
    };

            try
            {
                var response = await _openAIService.GetResponse(messages);
                return Ok(response);
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("429"))
            {
                var content = new StringContent("API quota exceeded. Please try again later.");
                var message = new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    Content = content
                };

                return ResponseMessage(message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}

