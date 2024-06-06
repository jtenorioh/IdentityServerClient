using DarthSeldon.Services.Caching.Abstractions;
using DarthSeldon.Settings.Abstractions;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace DarthSeldon.Services.Caching.Memory
{
    /// <summary>
    /// In Memory Cache
    /// </summary>
    /// <seealso cref="DarthSeldon.Services.Caching.Abstractions.ICache" />
    public class InMemoryCache : ICache
    {
        #region Members

        /// <summary>
        /// In Memory Settings
        /// </summary>
        private readonly InMemoryCacheSettings _settings;

        /// <summary>
        /// Memory cache
        /// </summary>
        private static readonly Memory<string> _memoryCache = new Memory<string>();

        /// <summary>
        /// Cache name
        /// </summary>
        private string _cacheName;

        #endregion Members

        #region Properties

        /// <summary>
        /// Gets the name of the cache.
        /// </summary>
        /// <value>
        /// The name of the cache.
        /// </value>
        public string Name { get => _cacheName; }

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryCache" /> class.
        /// </summary>
        /// <param name="settings">Settings.</param>
        public InMemoryCache(ISettings settings)
        {
            _settings = settings as InMemoryCacheSettings;
            _cacheName = _settings.Name;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Deletes the entry.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <returns>True or false if the entry was deleted</returns>
        public async Task<bool> DeleteEntry(string identifier)
        {
            return await _memoryCache.DeleteEntry(identifier);
        }

        /// <summary>
        /// Gets the value asynchronous.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <returns>Value</returns>
        public async Task<string> GetValueAsync(string identifier)
        {
            return await _memoryCache.GetOrCreate(identifier, () =>
            {
                throw new ApplicationException($"No value found for key: {identifier}");
            });
        }

        /// <summary>
        /// Gets the value asynchronous.
        /// </summary>
        /// <typeparam name="T">Generic</typeparam>
        /// <param name="identifier">Identifier.</param>
        /// <returns>Generic</returns>
        public async Task<T> GetValueAsync<T>(string identifier)
        {
            var cached = await _memoryCache.GetOrCreate(identifier, () =>
            {
                throw new ApplicationException($"No value found for key: {identifier}");
            });

            return JsonConvert.DeserializeObject<T>(cached);
        }

        /// <summary>
        /// Sets the value asynchronous.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="valueToStore">Value to store.</param>
        /// <returns>True or false if the value was set</returns>
        public async Task<bool> SetValueAsync(string identifier, string valueToStore)
        {
            if (_settings.IsUseExpiration)
                _ = await _memoryCache.GetOrCreate(identifier, () =>
                {
                    return Task.FromResult(valueToStore);
                });
            else
                _ = await _memoryCache.GetOrCreate(identifier, () =>
                {
                    return Task.FromResult(valueToStore);
                }, timeSpan: TimeSpan.FromDays(_settings.DefaultExpirationInDays));

            return true;
        }

        /// <summary>
        /// Sets the value asynchronous.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="valueToStore">Value to store.</param>
        /// <param name="expiration">Expiration.</param>
        /// <returns>True or false if the value was set</returns>
        public async Task<bool> SetValueAsync(string identifier, string valueToStore, TimeSpan expiration)
        {
            _ = await _memoryCache.GetOrCreate(identifier, () =>
            {
                return Task.FromResult(valueToStore);
            }, expiration);

            return true;
        }

        /// <summary>
        /// Sets the value asynchronous.
        /// </summary>
        /// <typeparam name="T">Generic</typeparam>
        /// <param name="identifier">Identifier.</param>
        /// <param name="objectToSerialize">Object to serialize.</param>
        /// <returns>True or false if the value was set</returns>
        public async Task<bool> SetValueAsync<T>(string identifier, T objectToSerialize)
        {
            var serialized = JsonConvert.SerializeObject(objectToSerialize);

            if (_settings.IsUseExpiration)
                _ = await SetValueAsync(identifier, serialized);
            else
                _ = await SetValueAsync(identifier, serialized, TimeSpan.FromDays(_settings.DefaultExpirationInDays));

            return true;
        }

        /// <summary>
        /// Sets the value asynchronous.
        /// </summary>
        /// <typeparam name="T">Generic</typeparam>
        /// <param name="identifier">Identifier.</param>
        /// <param name="objectToSerialize">Object to serialize.</param>
        /// <param name="expiration">Expiration.</param>
        /// <returns>True or false if the value was set</returns>
        public async Task<bool> SetValueAsync<T>(string identifier, T objectToSerialize, TimeSpan expiration)
        {
            var serialized = JsonConvert.SerializeObject(objectToSerialize);

            _ = await SetValueAsync(identifier, serialized, expiration);

            return true;
        }

        /// <summary>
        /// Determines whether [is identifier exists].
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <returns>True or false if the identifier exists</returns>
        public async Task<bool> DoesIdentifierExists(string identifier)
        {
            return await _memoryCache.DoesIdentifierExists(identifier);
        }

        #endregion Methods
    }
}