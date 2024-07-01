using System;
using System.Collections.Generic;

namespace OpenAIChatBot.Models
{
    public class ChatMessage
    {
        public string id { get; set; } =string.Empty;
        public string @object { get; set; } = string.Empty;
        public long created { get; set; } = 0;
        public string model { get; set; } = "gpt-3.5-turbo";
        public List<Choice> choices { get; set; }
        public Usage usage { get; set; }
        public object system_fingerprint { get; set; } = 0;
    }

    public class Choice
    {
        public int index { get; set; } =int.MaxValue;
        public Message message { get; set; }
        public object logprobs { get; set; } = 0;
        public string finish_reason { get; set; } = string.Empty;
    }

    public class Message
    {
        public string role { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
    }

    public class Usage
    {
        public int prompt_tokens { get; set; } = 0;
        public int completion_tokens { get; set; } = 0;
        public int total_tokens { get; set; } = 0;
    }
}
