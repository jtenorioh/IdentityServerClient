using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace DarthSeldon.Services.Caching.Memory
{
    /// <summary>
    /// Memory Cache Helper
    /// </summary>
    /// <typeparam name="T">Generic.</typeparam>
    public class Memory<T>
    {
        #region Members

        /// <summary>
        /// Memory Cache
        /// </summary>
        private readonly MemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions { ExpirationScanFrequency = TimeSpan.FromHours(1) });

        /// <summary>
        /// Locks
        /// </summary>
        private readonly ConcurrentDictionary<object, SemaphoreSlim> _locks = new ConcurrentDictionary<object, SemaphoreSlim>();

        #endregion Members

        #region Methods

        /// <summary>
        /// Gets or creates entry.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="createItem">The create item delegate.</param>
        /// <returns>Generic</returns>
        public async Task<T> GetOrCreate(object key, Func<Task<T>> createItem)
        {
            if (!_memoryCache.TryGetValue(key, out T cacheEntry))// Look for cache key.
            {
                SemaphoreSlim mylock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
                //notify all threads trying to access this value to await creation
                await mylock.WaitAsync();
                try
                {
                    if (!_memoryCache.TryGetValue(key, out cacheEntry))
                    {
                        // Key not in cache, so get data, this is a delegate function
                        cacheEntry = await createItem();

                        _memoryCache.Set(key, cacheEntry);
                    }
                }
                finally
                {
                    mylock.Release();
                }
            }
            return cacheEntry;
        }

        /// <summary>
        /// Gets or creates entry.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="createItem">The create item delegate.</param>
        /// <param name="timeSpan">Expiration time span.</param>
        /// <returns>Generic</returns>
        public async Task<T> GetOrCreate(object key, Func<Task<T>> createItem, TimeSpan timeSpan)
        {
            if (!_memoryCache.TryGetValue(key, out T cacheEntry))// Look for cache key.
            {
                SemaphoreSlim mylock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
                //notify all threads trying to access this value to await creation
                await mylock.WaitAsync();
                try
                {
                    if (!_memoryCache.TryGetValue(key, out cacheEntry))
                    {
                        // Key not in cache, so get data, this is a delegate function
                        cacheEntry = await createItem();

                        //obtain absolute expiration relative to now in time span format
                        _memoryCache.Set(key, cacheEntry, timeSpan);
                    }
                }
                finally
                {
                    mylock.Release();
                }
            }
            return cacheEntry;
        }

        /// <summary>
        /// Deletes the entry.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>True or false if the entry was deleted</returns>
        public async Task<bool> DeleteEntry(object key)
        {
            SemaphoreSlim mylock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
            //notify all threads trying to access this value to await creation
            await mylock.WaitAsync();
            try
            {
                _memoryCache.Remove(key);
            }
            finally
            {
                mylock.Release();
            }

            return true;
        }

        /// <summary>
        /// Determines whether [is identifier exists].
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>
        ///   <c>true</c> if [is identifier exists] [the specified key]; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> DoesIdentifierExists(object key)
        {
            try
            {
                var cached = _memoryCache.Get(key);

                if (cached != null)
                    return true;

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion Methods
    }
}