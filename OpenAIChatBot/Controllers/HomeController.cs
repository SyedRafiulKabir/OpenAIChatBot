using System.Web.Mvc;
using OpenAIChatBot.Models;
using System.Collections.Generic;

namespace OpenAIChatBot.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            // Assuming you want to pass an empty list of ChatMessage to the view initially
            var chatMessages = new List<ChatMessage>();
            return View(chatMessages);
        }
    }
}
