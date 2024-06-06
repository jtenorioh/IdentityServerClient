using DarthSeldon.Services.Caching.Abstractions;
using DarthSeldon.Services.KeyVault.Abstractions;
using Jose;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DarthSeldon.IdentityServerClient
{
    /// <summary>
    /// JWT Bearer Token Utility
    /// </summary>
    internal class JwtBearerTokenUtility
    {
        #region Members

        /// <summary>
        /// Settings
        /// </summary>
        private readonly IdentityServerClientSettings _settings;

        /// <summary>
        /// Caches
        /// </summary>
        private readonly IEnumerable<ICache> _caches;

        /// <summary>
        /// Key vaults
        /// </summary>
        private readonly IEnumerable<IKeyVault> _keyVaults;

        #endregion Members

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtBearerTokenUtility" /> class.
        /// </summary>
        /// <param name="settings">Settings.</param>
        public JwtBearerTokenUtility(IdentityServerClientSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtBearerTokenUtility"/> class.
        /// </summary>
        /// <param name="settings">Settings.</param>
        /// <param name="caches">Caches.</param>
        /// <param name="keyVaults">Key vaults.</param>
        public JwtBearerTokenUtility(IdentityServerClientSettings settings, IEnumerable<ICache> caches, IEnumerable<IKeyVault> keyVaults)
        {
            _settings = settings;
            _caches = caches;
            _keyVaults = keyVaults;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the signed JWT asynchronous.
        /// </summary>
        /// <returns>Signed JWT</returns>
        public async Task<string> GetSignedJwtAsync()
        {
            var jwtClaimset = new JwtClaimset
            {
                iss = _settings.ClientId,
                sub = _settings.ClientId,
                jti = Guid.NewGuid().ToString(),
                aud = _settings.Audience
            };

            // create a header & encode it
            var header = new { alg = _settings.JWTSigningAlgo };
            var headerSerialized = JsonConvert.SerializeObject(header);
            var headerEncoded = EncodeData(headerSerialized);

            // create a claimset & encode it
            var claimset = jwtClaimset;
            claimset.iat = EpochTime.GetIntDate(DateTime.Now);
            claimset.exp = EpochTime.GetIntDate(DateTime.Now.AddMinutes(_settings.JWTExpireInMinutes));
            var claimsetSerialized = JsonConvert.SerializeObject(claimset);
            var claimsetEncoded = EncodeData(claimsetSerialized);

            // Construct signature
            var dataToSign = $"{headerEncoded}.{claimsetEncoded}";
            var dataToSignBytes = new ASCIIEncoding().GetBytes(dataToSign);

            var keyVault = _keyVaults.FirstOrDefault(kv => string.Equals(kv.Name, _settings.KeyVaultSettingsName, StringComparison.CurrentCultureIgnoreCase));

            var cert = await keyVault.ObtainCertificateAsync(_settings.KeyVaultCertificateName);

            // get cert
            //var cert = await keyvault.ObtainCertificateAsync(_settings.AzureCertificateName);
            var privateKey = cert.GetRSAPrivateKey();
            var publicKey = cert.GetRSAPublicKey();

            // Hash, sign, url encode  data
            var signedData = privateKey.SignData(dataToSignBytes, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
            var jwtSignature = Base64Url.Encode(signedData);

            // Verify the data using the signature
            var sigIsValid = publicKey.VerifyData(dataToSignBytes, signedData, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);

            //return jwt format
            return $"{headerEncoded}.{claimsetEncoded}.{jwtSignature}";
        }

        /// <summary>
        /// Deletes the cached entry.
        /// </summary>
        /// <returns>True if deleted the entry, false if failed</returns>
        /// <exception cref="ArgumentException">The key/name is required to delete a cached entry</exception>
        public async Task<bool> DeleteCachedEntry()
        {
            throw new NotImplementedException();                      
        }

        /// <summary>
        /// Encodes the data.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <returns>Encoded data</returns>
        private string EncodeData(string data)
        {
            var dataBytes = Encoding.UTF8.GetBytes(data);
            var dataEncoded = Base64Url.Encode(dataBytes);

            return dataEncoded;
        }

        #endregion Methods
    }
}