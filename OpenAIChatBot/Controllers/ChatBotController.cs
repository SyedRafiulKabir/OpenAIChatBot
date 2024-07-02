using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
        private readonly MemoryManager _memoryManager;

        public ChatbotController()
        {
            var apiKey = ConfigurationManager.AppSettings["API_KEY"];
            _openAIService = new OpenAIService(apiKey);
            _memoryManager = new MemoryManager();
        }

        [HttpPost]
        [Route("getresponse")]
        public async Task<IHttpActionResult> GetResponse([FromBody] Message userMessage)
        {
            if (userMessage == null || string.IsNullOrEmpty(userMessage.content))
            {
                return BadRequest("Invalid input.");
            }

            var sessionId = Request.Headers.GetValues("Session-Id").FirstOrDefault();
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
            }

            // Retrieve previous messages from memory
            var messages = _memoryManager.GetMessages(sessionId);
            messages.Add(new Message { role = "user", content = userMessage.content });

            try
            {
                var response = await _openAIService.GetResponse(messages);

                // Add the assistant's response to the memory
                messages.Add(new Message { role = "assistant", content = response.choices[0].message.content });
                _memoryManager.SaveMessages(sessionId, messages);

                return Ok(new { sessionId = sessionId, response = response });
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
