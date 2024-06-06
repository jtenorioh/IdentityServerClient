using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace DarthSeldon.Services.KeyVault.Abstractions
{
    /// <summary>
    /// Interface Key Vault
    /// </summary>
    public interface IKeyVault
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
        /// Gets the secret.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Secret</returns>
        Task<string> GetSecret(string name);

        /// <summary>
        /// Sets the secret.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>Tur or false if the secret was set</returns>
        Task<bool> SetSecret(string key, string value);

        /// <summary>
        /// Obtains the certificate.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>Certificate</returns>
        X509Certificate2 ObtainCertificate(string key);

        /// <summary>
        /// Obtains the certificate.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>Certificate</returns>
        Task<X509Certificate2> ObtainCertificateAsync(string key);

        /// <summary>
        /// Obtains the list of secrets.
        /// </summary>
        /// <returns>List of secrets</returns>
        IReadOnlyCollection<string> ObtainListOfSecrets();

        #endregion Methods
    }
}