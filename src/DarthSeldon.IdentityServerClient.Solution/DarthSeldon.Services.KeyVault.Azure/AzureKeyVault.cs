using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using DarthSeldon.Services.Caching.Abstractions;
using DarthSeldon.Services.KeyVault.Abstractions;
using DarthSeldon.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace DarthSeldon.Services.KeyVault.Azure
{
    /// <summary>
    /// Azure Key Vault Helper
    /// </summary>
    /// <seealso cref="DarthSeldon.Services.KeyVault.Abstractions.IKeyVault" />
    public class AzureKeyVault : IKeyVault
    {
        #region Members

        /// <summary>
        /// Settings
        /// </summary>
        private readonly AzureKeyVaultSettings _settings;

        /// <summary>
        /// Client secret credential
        /// </summary>
        private readonly ClientSecretCredential _clientSecretCredential;

        /// <summary>
        /// Secret Client
        /// </summary>
        private readonly SecretClient _client;

        /// <summary>
        /// Cache
        /// </summary>
        private readonly ICache _cache;

        /// <summary>
        /// Key vault name
        /// </summary>
        private readonly string _keyVaultName;

        #endregion Members

        #region Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name => _keyVaultName;

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureKeyVault" /> class.
        /// </summary>
        /// <param name="settings">Settings.</param>
        /// <param name="iCaches">Caches.</param>
        /// <exception cref="System.ArgumentException">The Azure Key Vault Settings are not initialized
        /// or
        /// The Azure Key Vault settings are not initialized correctly</exception>
        /// <exception cref="ArgumentException">The Azure Key Vault Settings are not initialized
        /// or
        /// The Azure Key Vault settings are not initialized correctly</exception>
        public AzureKeyVault(ISettings settings, IEnumerable<ICache> iCaches)
        {
            _settings = settings as AzureKeyVaultSettings;

            if (_settings == null)
                throw new ArgumentException("The Azure Key Vault Settings are not initialized");

            if (string.IsNullOrEmpty(_settings.Name))
                throw new ArgumentException("The Azure Key Vault settings are not initialized correctly, name is missing or is a duplicate from settings");

            _keyVaultName = _settings.Name;

            if (_settings.IsManagedServiceIdentity)
            {
                if (_settings.IsUseVisualStudioIdentity)
                    _client = new SecretClient(new Uri(_settings.AzureKeyVaultUri), new VisualStudioCredential());
                else
                    _client = new SecretClient(new Uri(_settings.AzureKeyVaultUri), new DefaultAzureCredential());
            }
            else
            {
                if (string.IsNullOrEmpty(_settings.TenantId) || string.IsNullOrEmpty(_settings.ClientId) || string.IsNullOrEmpty(_settings.ClientSecret) || string.IsNullOrEmpty(_settings.AzureKeyVaultUri))
                    throw new ArgumentException("The Azure Key Vault settings are not initialized correctly");

                _clientSecretCredential = new ClientSecretCredential(_settings.TenantId, _settings.ClientId, _settings.ClientSecret);
                _client = new SecretClient(new Uri(_settings.AzureKeyVaultUri), _clientSecretCredential);
            }

            if (_settings.IsUseCache)
            {
                _cache = iCaches.FirstOrDefault(x => x.Name == _settings.CacheSettingsName);

                if (_cache == null)
                    throw new ArgumentException($"The cache with name {_settings.CacheSettingsName} is not found");
            }
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the secret.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <returns>Secret</returns>
        /// <exception cref="System.ArgumentException">The secret key/name is required</exception>
        /// <exception cref="System.ApplicationException">The cache is not initialized</exception>
        public async Task<string> GetSecret(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("The secret key/name is required");

            if (!_settings.IsUseCache)
            {
                var result = await _client.GetSecretAsync(name);
                return result.Value.Value;
            }
            else
            {
                if (_cache != null)
                {
                    try
                    {
                        return await _cache.GetValueAsync(name);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            var result = await _client.GetSecretAsync(name);
                            await _cache.SetValueAsync(name, result.Value.Value);
                            return result.Value.Value;
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException($"The cache is not initialized. {ex.Message}");
                        }
                    }
                }

                throw new ApplicationException("The cache is not initialized");
            }
        }

        /// <summary>
        /// Sets the secret.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <returns>True or false if the secret was set</returns>
        /// <exception cref="System.ArgumentException">The secret key/name is required
        /// or
        /// The secret value is required</exception>
        /// <exception cref="System.ApplicationException">The cache is not initialized
        /// or</exception>
        /// <exception cref="ApplicationException"></exception>
        public async Task<bool> SetSecret(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The secret key/name is required");

            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("The secret value is required");

            var keyVaultSecret = new KeyVaultSecret(key, value);

            try
            {
                var result = await _client.SetSecretAsync(keyVaultSecret);

                if (_settings.IsUseCache)
                {
                    if (_cache == null)
                        throw new ApplicationException("The cache is not initialized");

                    return await _cache.SetValueAsync(key, value);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

            return true;
        }

        /// <summary>
        /// Obtains the certificate.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>Certificate</returns>
        /// <exception cref="System.ArgumentException">The certificate key/name is required</exception>
        /// <exception cref="System.ApplicationException">The cache is not initialized
        /// or</exception>
        /// <exception cref="ApplicationException"></exception>
        public X509Certificate2 ObtainCertificate(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The certificate key/name is required");

            try
            {
                if (!_settings.IsUseCache)
                {
                    var secret = _client.GetSecret(key);
                    return new X509Certificate2(Convert.FromBase64String(secret.Value.Value), (string)null, X509KeyStorageFlags.Exportable);
                }
                else
                {
                    if (_cache == null)
                        throw new ApplicationException("The cache is not initialized");

                    try
                    {
                        var cachedItem = _cache.GetValueAsync(key).Result;
                        return new X509Certificate2(Convert.FromBase64String(cachedItem), (string)null, X509KeyStorageFlags.Exportable);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            var secret = _client.GetSecret(key);
                            _ = _cache.SetValueAsync(key, secret.Value.Value).Result;
                            return new X509Certificate2(Convert.FromBase64String(secret.Value.Value), (string)null, X509KeyStorageFlags.Exportable);
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Obtains the certificate asynchronous.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>Certificate</returns>
        /// <exception cref="System.ArgumentException">The certificate key/name is required</exception>
        /// <exception cref="System.ApplicationException">The cache is not initialized
        /// or</exception>
        public async Task<X509Certificate2> ObtainCertificateAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The certificate key/name is required");

            try
            {
                if (!_settings.IsUseCache)
                {
                    var secret = await _client.GetSecretAsync(key);
                    return new X509Certificate2(Convert.FromBase64String(secret.Value.Value), (string)null, X509KeyStorageFlags.Exportable);
                }
                else
                {
                    if (_cache == null)
                        throw new ApplicationException("The cache is not initialized");

                    try
                    {
                        var cachedItem = await _cache.GetValueAsync(key);
                        return new X509Certificate2(Convert.FromBase64String(cachedItem), (string)null, X509KeyStorageFlags.Exportable);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            var secret = await _client.GetSecretAsync(key);
                            _ = await _cache.SetValueAsync(key, secret.Value.Value);
                            return new X509Certificate2(Convert.FromBase64String(secret.Value.Value), (string)null, X509KeyStorageFlags.Exportable);
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Deletes the cache entry.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>True or false if the cache entry was deleted</returns>
        /// <exception cref="System.ArgumentException">The key/name is required to delete a cached entry</exception>
        /// <exception cref="System.ApplicationException">The cache is not initialized</exception>
        /// <exception cref="ArgumentException">The key/name is required to delete a cached entry</exception>
        public async Task<bool> DeleteCacheEntry(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key/name is required to delete a cached entry");

            if (!_settings.IsUseCache)
            {
                return true;
            }

            if (_cache == null)
                throw new ApplicationException("The cache is not initialized");

            return await _cache.DeleteEntry(key);
        }

        /// <summary>
        /// Obtains the list of secrets.
        /// </summary>
        /// <returns>List of secrets</returns>
        public IReadOnlyCollection<string> ObtainListOfSecrets()
        {
            var secrets = new List<string>();

            var allSecrets = _client.GetPropertiesOfSecrets();

            foreach (var secretProperties in allSecrets)
                secrets.Add(secretProperties.Name);

            return secrets;
        }
    }

    #endregion Methods
}