using DarthSeldon.Services.Caching.Abstractions;
using DarthSeldon.Services.KeyVault.Abstractions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DarthSeldon.IdentityServerClient
{
    /// <summary>
    /// Identity Server Client
    /// </summary>
    public class IdentityServerClient
    {
        #region Members

        /// <summary>
        /// Identity Server Client Settings
        /// </summary>
        private readonly IdentityServerClientSettings _settings;

        /// <summary>
        /// Caches
        /// </summary>
        private readonly IEnumerable<ICache> _caches;

        /// <summary>
        /// Key Vaults
        /// </summary>
        private readonly IEnumerable<IKeyVault> _keyVaults;

        #endregion Members

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityServerClient" /> class.
        /// </summary>
        /// <param name="settings">Settings.</param>
        /// <param name="caches">Caches.</param>
        /// <param name="keyVaults">Key vaults.</param>
        public IdentityServerClient(IdentityServerClientSettings settings, IEnumerable<ICache> caches, IEnumerable<IKeyVault> keyVaults)
        {
            _settings = settings;
            _caches = caches;
            _keyVaults = keyVaults;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Obtains the authorization token signed JWT asynchronous.
        /// </summary>
        /// <returns>Authorization Token</returns>
        /// <exception cref="ApplicationException">$"Failed status code: {responseMessage.StatusCode} Message : {await responseMessage.Content.ReadAsStringAsync()}</exception>
        public async Task<AuthorizationToken> ObtainAuthorizationTokenSignedJWTAsync()
        {
            var utility = new JwtBearerTokenUtility(_settings, _caches, _keyVaults);
            var signedJwt = await utility.GetSignedJwtAsync();

            HttpResponseMessage responseMessage = null;
            using (HttpClient client = new HttpClient())
            {
                var url = $"{_settings.AuthorizationServer}{_settings.SuffixTokenEndpoint}";

                HttpRequestMessage tokenRequest =
                    new HttpRequestMessage(HttpMethod.Post, url);
                HttpContent httpContent = new FormUrlEncodedContent(
                    new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "client_credentials"),
                        new KeyValuePair<string, string>("client_id", _settings.ClientId),
                        new KeyValuePair<string, string>("client_assertion_type", _settings.ClientAssertionType),
                        new KeyValuePair<string, string>("client_assertion", signedJwt),
                        new KeyValuePair<string, string>("scope", _settings.Scope)
                    });
                tokenRequest.Content = httpContent;

                responseMessage = await client.SendAsync(tokenRequest);

                if (!responseMessage.IsSuccessStatusCode)
                {
                    throw new ApplicationException($"Failed status code: {responseMessage.StatusCode} Message : {await responseMessage.Content.ReadAsStringAsync()}");
                }
            }
            var plainToken = await responseMessage.Content.ReadAsStringAsync();

            JObject token = JObject.Parse(plainToken);

            int seconds = int.Parse(token.GetValue("expires_in").ToString());

            var authorizationToken = new AuthorizationToken
            {
                AccessToken = token.GetValue("access_token").ToString(),
                ExpiresInDateTime = DateTime.Now.Add(new TimeSpan(0, 0, 0, seconds)),
                TokenType = token.GetValue("token_type").ToString()
            };

            return authorizationToken;
        }

        /// <summary>
        /// Deletes the cached entry.
        /// </summary>
        /// <returns>True if the value was deleted or false if it failed</returns>
        /// <exception cref="ArgumentException">The key/name is required to delete a cached entry</exception>
        public async Task<bool> DeleteCachedEntry()
        {
            if (string.IsNullOrEmpty(_settings.KeyVaultCertificateName))
            {
                throw new ArgumentException("The key/name is required to delete a cached entry");
            }

            var utility = new JwtBearerTokenUtility(_settings);

            return await utility.DeleteCachedEntry();
        }

        #endregion Methods
    }
}