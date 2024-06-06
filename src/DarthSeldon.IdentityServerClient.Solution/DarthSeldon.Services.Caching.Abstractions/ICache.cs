using System;
using System.Threading.Tasks;

namespace DarthSeldon.Services.Caching.Abstractions
{
    /// <summary>
    /// Interface Cache
    /// </summary>
    public interface ICache
    {
        #region Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Deletes the entry.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <returns>True or false if the entry was deleted</returns>
        Task<bool> DeleteEntry(string identifier);

        /// <summary>
        /// Gets the value asynchronous.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <returns>Value</returns>
        Task<string> GetValueAsync(string identifier);

        /// <summary>
        /// Gets the value asynchronous.
        /// </summary>
        /// <typeparam name="T">Generic</typeparam>
        /// <param name="identifier">Identifier.</param>
        /// <returns>Generic</returns>
        Task<T> GetValueAsync<T>(string identifier);

        /// <summary>
        /// Sets the value asynchronous.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="valueToStore">Value to store.</param>
        /// <returns>True or false if the value was set</returns>
        Task<bool> SetValueAsync(string identifier, string valueToStore);

        /// <summary>
        /// Sets the value asynchronous.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="valueToStore">Value to store.</param>
        /// <param name="expiration">Expiration.</param>
        /// <returns>True or false if the value was set</returns>
        Task<bool> SetValueAsync(string identifier, string valueToStore, TimeSpan expiration);

        /// <summary>
        /// Sets the value asynchronous.
        /// </summary>
        /// <typeparam name="T">Generic</typeparam>
        /// <param name="identifier">Identifier.</param>
        /// <param name="objectToSerialize">Object to serialize.</param>
        /// <returns>True or false if the value was set</returns>
        Task<bool> SetValueAsync<T>(string identifier, T objectToSerialize);

        /// <summary>
        /// Sets the value asynchronous.
        /// </summary>
        /// <typeparam name="T">Generic</typeparam>
        /// <param name="identifier">Identifier.</param>
        /// <param name="objectToSerialize">Object to serialize.</param>
        /// <param name="expiration">Expiration.</param>
        /// <returns>True or false if the value was set</returns>
        Task<bool> SetValueAsync<T>(string identifier, T objectToSerialize, TimeSpan expiration);

        /// <summary>
        /// Does the identifier exists?
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <returns>True or false if the identifier exists</returns>
        Task<bool> DoesIdentifierExists(string identifier);

        #endregion Methods
    }
}