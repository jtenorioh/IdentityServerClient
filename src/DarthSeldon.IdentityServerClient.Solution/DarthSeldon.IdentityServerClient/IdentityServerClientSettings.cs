using DarthSeldon.Settings.Abstractions;

namespace DarthSeldon.IdentityServerClient
{
    /// <summary>
    /// Identity Server Client Settings
    /// </summary>
    /// <seealso cref="DarthSeldon.Settings.Abstractions.ISettings" />
    public class IdentityServerClientSettings : ISettings
    {
        #region Properties

        /// <summary>
        /// Gets or sets the authorization server
        /// </summary>
        /// <value>
        /// AuthorizationServer
        /// </value>
        public string AuthorizationServer { get; set; }

        /// <summary>
        /// Gets or sets the suffix token endpoint.
        /// </summary>
        /// <value>
        /// The suffix token endpoint.
        /// </value>
        public string SuffixTokenEndpoint { get; set; } = "/connect/token";

        /// <summary>
        /// Gets or sets the client id
        /// </summary>
        /// <value>
        /// ClientId
        /// </value>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret
        /// </summary>
        /// <value>
        /// ClientSecret
        /// </value>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the scope
        /// </summary>
        /// <value>
        /// Scope
        /// </value>
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// Name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// Type.
        /// </value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the audience.
        /// </summary>
        /// <value>
        /// The audience.
        /// </value>
        public string Audience { get; set; }

        /// <summary>
        /// Gets or sets the type of the client assertion. Default value urn:ietf:params:oauth:client-assertion-type:jwt-bearer
        /// </summary>
        /// <value>
        /// The type of the client assertion.
        /// </value>
        public string ClientAssertionType { get; set; } = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";

        /// <summary>
        /// Gets or sets the JWT expire in minutes. Default value 5
        /// </summary>
        /// <value>
        /// The JWT expire in minutes.
        /// </value>
        public int JWTExpireInMinutes { get; set; } = 5;

        /// <summary>
        /// Gets or sets the JWT signing algo. Default value RS512
        /// </summary>
        /// <value>
        /// The JWT signing algo.
        /// </value>
        public string JWTSigningAlgo { get; set; } = "RS512";

        /// <summary>
        /// Gets or sets the name of the key vault certificate.
        /// </summary>
        /// <value>
        /// The name of the key vault certificate.
        /// </value>
        public string KeyVaultCertificateName { get; set; }

        /// <summary>
        /// Gets or sets the name of the key vault settings.
        /// </summary>
        /// <value>
        /// The name of the key vault settings.
        /// </value>
        public string KeyVaultSettingsName { get; set; }

        #endregion Properties
    }
}