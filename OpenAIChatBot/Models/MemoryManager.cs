using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using OpenAIChatBot.Models;

public class MemoryManager
{
    private readonly MemoryCache _cache;

    public MemoryManager()
    {
        _cache = MemoryCache.Default;
    }

    public List<Message> GetMessages(string sessionId)
    {
        if (_cache.Contains(sessionId))
        {
            return (List<Message>)_cache.Get(sessionId);
        }
        return new List<Message>();
    }

    public void SaveMessages(string sessionId, List<Message> messages)
    {
        var cacheItemPolicy = new CacheItemPolicy
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30) // Set expiration time as needed
        };
        _cache.Set(sessionId, messages, cacheItemPolicy);
    }
}
