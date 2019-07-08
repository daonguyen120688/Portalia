using System;
using System.Runtime.Caching;

namespace Portalia.Core.Infrastructure
{
    public class Caching
    {
        /// <summary>
        /// Set cache for specific item by key.
        /// Cache data will be refreshed, if it is not evicted greater than 7 days.
        /// </summary>
        /// <param name="cacheKey">Unique key for a specific cache data.</param>
        /// <param name="data">Data that is cached.</param>
        /// <param name="cachedDay">Number of days that cache data will be refreshed.</param>
        public static void SetCache(string cacheKey,object data,Int16 cachedDay=7)
        {
            ObjectCache cache = MemoryCache.Default;
            cache.Remove(cacheKey);
            CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
            cacheItemPolicy.AbsoluteExpiration = DateTime.Now.AddDays(cachedDay);
            cache.Add(cacheKey, data, cacheItemPolicy);
        }

        /// <summary>
        /// Get cache data by key.
        /// If it has been not cached yet, system will perform caching data and return it.
        /// </summary>
        /// <typeparam name="T">Type of cache data.</typeparam>
        /// <param name="cacheKey">Unique key for a specific cache data.</param>
        /// <param name="callbackFunc">A callback method in order to specify cache data.</param>
        /// <returns>Cache data as T type</returns>
        public static T GetCache<T>(string cacheKey, Func<T> callbackFunc,Int16 cachedDay=7)
        {
            ObjectCache cache = MemoryCache.Default;
            if (cache.Contains(cacheKey))
                return (T)cache.Get(cacheKey);
            T result = callbackFunc();
            SetCache(cacheKey, result,cachedDay);
            return result;
        }
    }

    public class CacheKey
    {
        public const string DailyBasic= "DailyBasic";
        public const string Title = "Title";
        public const string Country = "Country";
        public const string Skill = "Skill";
        public const string Currency = "Currency";
        public const string ClientList = "ClientList";
    }
}
